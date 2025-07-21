using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
using System;

public class DaoService : MonoBehaviour
{
    public static DaoService Instance { get; private set; }

    private DatabaseReference dbRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                    Debug.Log("Firebase đã sẵn sàng");
                }
                else
                {
                    Debug.LogError("Firebase chưa sẵn sàng: " + dependencyStatus);
                }
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgressForUser(int map_num, string uid, string displayName, int newDistance, bool completed)
    {
        GetHighestDistanceOfUser(map_num, uid, currentDistance =>
        {
            var data = new Dictionary<string, object>
            {
            { "name", displayName },
            { "completed", completed }
            };

            if (newDistance > currentDistance)
            {
                data["distance"] = newDistance;
            }

            dbRef.Child("leaderboard")
                 .Child(map_num.ToString())
                 .Child(uid)
                 .UpdateChildrenAsync(data)
                 .ContinueWithOnMainThread(task =>
                 {
                     if (task.IsFaulted)
                     {
                         Debug.LogError("Lỗi khi lưu: " + task.Exception);
                     }
                     else
                     {
                         Debug.Log($"Đã lưu thành công. Distance mới {(newDistance > currentDistance ? $"được cập nhật: {newDistance}" : "không thay đổi")}. Completed: {completed}");
                     }
                 });
        });
    }



    public void GetHighestDistanceOfUser(int map_num, string uid, Action<int> onResult)
    {
        dbRef.Child("leaderboard")
             .Child(map_num.ToString())
             .Child(uid)
             .Child("distance")
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {
                 int distance = 0;
                 if (task.IsFaulted)
                 {
                     Debug.LogError("Lỗi khi lấy dữ liệu: " + task.Exception);
                 }
                 else if (task.Result.Exists)
                 {
                     distance = Convert.ToInt32(task.Result.Value);
                     Debug.Log("Đã lấy thành công distance: " + distance);
                 }
                 onResult?.Invoke(distance);
             });
    }

    public void GetCompletedStatusOfUserInMap(int map_num, string uid, Action<bool> onResult)
    {
        dbRef.Child("leaderboard")
             .Child(map_num.ToString())
             .Child(uid)
             .Child("completed")
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.LogError("Lỗi khi lấy dữ liệu: " + task.Exception);
                     onResult(false);
                 }
                 else if (task.Result.Exists)
                 {
                     try
                     {
                         bool completed = Convert.ToBoolean(task.Result.Value);
                         Debug.Log("Field 'completed' tồn tại: " + completed);
                         onResult(completed);
                     }
                     catch (Exception ex)
                     {
                         Debug.LogError("Không thể convert completed: " + ex.Message);
                         onResult(false);
                     }
                 }
                 else
                 {
                     Debug.Log("Field 'completed' không tồn tại.");
                     onResult(false);
                 }
             });
    }



    public void GetLatestScoreOfUser(string uid, Action<int> onResult)
    {
        dbRef.Child("user-info").Child(uid).Child("score").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                int latest_score = 0;
                if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi lấy score: " + task.Exception);
                }
                else if (task.Result.Exists)
                {
                    latest_score = Convert.ToInt32(task.Result.Value);
                }

                onResult?.Invoke(latest_score);
            });
    }

    public void UpdateLatestScoreOfAUser(string uid, int plus_score)
    {
        GetLatestScoreOfUser(uid, currentScore =>
        {
            int updatedScore = currentScore + plus_score;

            dbRef.Child("user-info").Child(uid).Child("score").SetValueAsync(updatedScore)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Lỗi khi cập nhật điểm: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log($"Đã cập nhật điểm của {uid}: {updatedScore}");
                    }
                });
        });
    }

    public void GetMusicAndSFXVolumeSettings(string uid, Action<Dictionary<string, float>> onResult)
    {
        dbRef.Child("user-info").Child(uid).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                Dictionary<string, float> resultData = new Dictionary<string, float>();

                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting volume settings: " + task.Exception);
                }
                else if (task.Result.Exists)
                {
                    var dataSnapshot = task.Result;

                    float musicVol = 0.7f;
                    float sfxVol = 0.7f;

                    if (dataSnapshot.HasChild("music-volume"))
                    {
                        float.TryParse(dataSnapshot.Child("music-volume").Value.ToString(), out musicVol);
                    }

                    if (dataSnapshot.HasChild("sfx-volume"))
                    {
                        float.TryParse(dataSnapshot.Child("sfx-volume").Value.ToString(), out sfxVol);
                    }

                    resultData["music-volume"] = musicVol;
                    resultData["sfx-volume"] = sfxVol;
                }
                else
                {
                    resultData["music-volume"] = 0.7f;
                    resultData["sfx-volume"] = 0.7f;
                }

                onResult?.Invoke(resultData);
            });
    }

    public void SaveMusicAndSFXVolumeSettings(string uid, float musicVolume, float sfxVolume, Action<bool> onResult)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "music-volume", musicVolume },
            { "sfx-volume", sfxVolume }
        };

        dbRef.Child("user-info").Child(uid).UpdateChildrenAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Volume settings saved successfully.");
                    onResult?.Invoke(true);
                }
                else
                {
                    Debug.LogError("Failed to save volume settings: " + task.Exception);
                    onResult?.Invoke(false);
                }
            });
    }



}

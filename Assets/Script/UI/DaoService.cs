using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
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

            // Khởi tạo Firebase
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
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

    // SAVE
    public void SaveProgressForUser(int map_num, string uid, string displayName, int distance)
    {
        var data = new Dictionary<string, object>
        {
            { "name", displayName },
            { "distance", distance }
        };

        dbRef.Child("leaderboard")
             .Child(map_num.ToString())
             .Child(uid)
             .SetValueAsync(data)
             .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Đã lưu");
            else
                Debug.LogError("Lỗi khi lưu: " + task.Exception);
        });
    }



}

using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using Firebase.Database;
using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;


    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;


        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase đã được kết nối thành công!");
            }
            else
            {
                Debug.LogError("❌ Firebase lỗi: " + status);
            }
        });
    }

    public void LoginOrRegister(string email, string password, string displayName, Action<FirebaseUser> onSuccess = null)
    {
        if (auth == null) return;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var user = task.Result.User;
                Debug.Log("✅ Đăng nhập: " + user.Email);

                if (!string.IsNullOrEmpty(displayName))
                {
                    user.UpdateUserProfileAsync(new UserProfile { DisplayName = displayName })
                    .ContinueWithOnMainThread(_ =>
                    {
                        onSuccess?.Invoke(user);
                    });
                }
                return;
            }

            // Nếu lỗi là do chưa có tài khoản → đăng ký
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(registerTask =>
            {
                if (registerTask.IsCompletedSuccessfully)
                {
                    var newUser = registerTask.Result.User;
                    Debug.Log("✅ Đăng ký: " + newUser.Email);

                    newUser.UpdateUserProfileAsync(new UserProfile { DisplayName = displayName })
                        .ContinueWithOnMainThread(_ =>
                        {
                            onSuccess?.Invoke(newUser);
                        });
                }
                else
                {
                    Debug.LogError("❌ Lỗi đăng ký: " + registerTask.Exception?.Message);
                }
            });
        });
    }


    public void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("👋 Đã đăng xuất.");
        }
    }

    public FirebaseUser GetUser()
    {
        return auth?.CurrentUser;
    }

    public bool IsSignedIn()
    {
        return auth != null && auth.CurrentUser != null;
    }

    public void SaveScore(int score, long completeTime, String mapName)
    {
        if (!IsSignedIn()) return;

        FirebaseUser user = GetUser();
        string uid = user.UserId;
        string name = user.DisplayName ?? user.Email;

        Dictionary<string, object> entry = new Dictionary<string, object>
        {
            { "score", score },
            { "complete_time", completeTime },
            { "display_name", name }
        };

        dbRef.Child("leaderboard")
        .Child(mapName)
             .Child(uid)
             .SetValueAsync(entry)
             .ContinueWithOnMainThread(task =>
             {
                 if (task.IsCompletedSuccessfully)
                     Debug.Log("✅ Ghi điểm thành công!");
                 else
                     Debug.LogError("❌ Ghi điểm thất bại: " + task.Exception?.Message);
             });
    }


    // public ScoreData GetScore(String mapName)
    // {
        
    // }
}

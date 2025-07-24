using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using Firebase.Database;
using System.Collections.Generic;

public class AuthService : MonoBehaviour
{
    public static AuthService Instance { get; private set; }

    private FirebaseAuth auth;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

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
                onSuccess?.Invoke(user);
                return;
            }

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

}

using UnityEngine;
using TMPro; // Nếu bạn dùng TMP_InputField
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField displayNameInput;

    [Header("Buttons")]
    public GameObject signInOrUpButton;
    public GameObject signOutButton;

    [Header("Text")]
    public Text displayNameText;

    [Header("Manager")]
    public UserManager userManager;


    public void Open()
    {
        gameObject.SetActive(true);
        SetInputsAndButton();
        SetDisplayName();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleLoginOrRegister()
    {
        userManager.LoginOrRegister(emailInput.text, passwordInput.text, displayNameInput.text, (user) =>
        {
            Debug.Log("✅ Đăng nhập/đăng ký hoàn tất");
            SetInputsAndButton();
        });
    }

    public void SetInputsAndButton()
    {
        bool isSignedIn = userManager.IsSignedIn();

        emailInput.interactable = !isSignedIn;
        passwordInput.interactable = !isSignedIn;
        displayNameInput.interactable = !isSignedIn;

        if (signInOrUpButton != null) signInOrUpButton.SetActive(!isSignedIn);
        if (signOutButton != null) signOutButton.SetActive(isSignedIn);

        if (isSignedIn)
        {
            var user = userManager.GetUser();
            displayNameInput.text = user?.DisplayName ?? "";
            emailInput.text = user?.Email ?? "";
            passwordInput.text = "********";
        }
        else
        {
            emailInput.text = "";
            passwordInput.text = "";
            displayNameInput.text = "";
        }
    }

    public void HandleLogout()
    {
        userManager.Logout();
        SetInputsAndButton();
    }

    public void SetDisplayName()
    {
        var user = userManager.GetUser();
        if (user != null)
        {
            displayNameText.text = user.DisplayName ?? user.Email;
        }
        else
        {
            displayNameText.text = "<ANONYMOUS>";
        }
    }

}

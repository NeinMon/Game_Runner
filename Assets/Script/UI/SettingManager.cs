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
    public Button signInOrUpButton;
    public Button signOutButton;

    [Header("Text")]
    public Text displayNameText;

    [Header("Manager")]
    private AuthService authService => AuthService.Instance;

    public void Start()
    {
        signInOrUpButton.onClick.AddListener(HandleLoginOrRegister);
        signOutButton.onClick.AddListener(HandleLogout);
        SetInputsAndButton();
        SetDisplayName();
    }


    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleLoginOrRegister()
    {
        authService.LoginOrRegister(emailInput.text, passwordInput.text, displayNameInput.text, (user) =>
        {
            SetInputsAndButton();
            SetDisplayName();
        });
    }



    public void HandleLogout()
    {
        authService.Logout();
        SetInputsAndButton();
        SetDisplayName();
    }

    public void SetDisplayName()
    {
        var user = authService.GetUser();
        if (user != null)
        {
            displayNameText.text = user.DisplayName ?? "<ANONYMOUS>";
            Debug.Log("DISPLAY NAME OF THIS GUY: " + user.DisplayName);
        }
        else
        {
            displayNameText.text = "<ANONYMOUS>";
        }
    }
    public void SetInputsAndButton()
    {
        bool isSignedIn = authService.IsSignedIn();
        emailInput.text = "";
        passwordInput.text = "";
        displayNameInput.text = "";
        emailInput.gameObject.SetActive(!isSignedIn);
        passwordInput.gameObject.SetActive(!isSignedIn);
        displayNameInput.gameObject.SetActive(!isSignedIn);

        if (signInOrUpButton != null) signInOrUpButton.gameObject.SetActive(!isSignedIn);
        if (signOutButton != null) signOutButton.gameObject.SetActive(isSignedIn);
    }
}

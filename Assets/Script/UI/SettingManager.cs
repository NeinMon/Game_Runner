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

    [Header("Music & SFX Slider")]
    public Slider musicSlider;
    public Slider sfxSlider;



    public void Start()
    {
        signInOrUpButton.onClick.AddListener(HandleLoginOrRegister);
        signOutButton.onClick.AddListener(HandleLogout);
        SetInputsAndButton();
        SetDisplayName();
        SoundManager.Instance.LoadMusicVolumeAndSFXVolume(() =>
        {
            SetMusicVolumeAndSFXVolume();
        });
    }


    public void Open()
    {
        gameObject.SetActive(true);
        SoundManager.Instance.LoadMusicVolumeAndSFXVolume(() =>
        {
            SetMusicVolumeAndSFXVolume();
        });
    }

    public void Close()
    {
        SoundManager.Instance.SaveMusicVolumeAndSFXVolume();
        gameObject.SetActive(false);
    }

    public void HandleLoginOrRegister()
    {
        AuthService.Instance.LoginOrRegister(emailInput.text, passwordInput.text, displayNameInput.text, (user) =>
        {
            SetInputsAndButton();
            SetDisplayName();
            SoundManager.Instance.LoadMusicVolumeAndSFXVolume(() =>
            {
                SetMusicVolumeAndSFXVolume();
            });
        });
    }

    public void HandleLogout()
    {
        AuthService.Instance.Logout();
        SoundManager.Instance.SaveMusicVolumeAndSFXVolume();
        SetInputsAndButton();
        SetDisplayName();
    }

    public void SetDisplayName()
    {
        var user = AuthService.Instance.GetUser();
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
        bool isSignedIn = AuthService.Instance.IsSignedIn();
        emailInput.text = "";
        passwordInput.text = "";
        displayNameInput.text = "";
        emailInput.gameObject.SetActive(!isSignedIn);
        passwordInput.gameObject.SetActive(!isSignedIn);
        displayNameInput.gameObject.SetActive(!isSignedIn);

        if (signInOrUpButton != null) signInOrUpButton.gameObject.SetActive(!isSignedIn);
        if (signOutButton != null) signOutButton.gameObject.SetActive(isSignedIn);
    }



    public void MusicVolume()
    {
        float musicVolume = musicSlider.value;
        SoundManager.Instance.MusicVolume(musicVolume);
    }

    public void SFXVolume()
    {
        float sfxVolume = sfxSlider.value;
        SoundManager.Instance.SFXVolume(sfxVolume);
    }

    private void SetMusicVolumeAndSFXVolume()
    {
        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSFXVolume();
    }
}

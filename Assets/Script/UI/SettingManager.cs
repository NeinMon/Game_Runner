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

    [Header("Music & SFX Slider")]
    public Slider musicSlider;
    public Slider sfxSlider;

    public void Start()
    {
        signInOrUpButton.onClick.AddListener(HandleLoginOrRegister);
        signOutButton.onClick.AddListener(HandleLogout);
        SetInputsAndButton();
        SetDisplayName();
        LoadMusicVolumeAndSFXVolume();
    }


    public void Open()
    {
        gameObject.SetActive(true);
        LoadMusicVolumeAndSFXVolume();
    }

    public void Close()
    {
        SaveMusicVolumeAndSFXVolume();
        gameObject.SetActive(false);
    }

    public void HandleLoginOrRegister()
    {
        authService.LoginOrRegister(emailInput.text, passwordInput.text, displayNameInput.text, (user) =>
        {
            SetInputsAndButton();
            SetDisplayName();
        });
        LoadMusicVolumeAndSFXVolume();
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

    private void LoadMusicVolumeAndSFXVolume()
    {
        string uid = authService.GetUser()?.UserId;
        if (string.IsNullOrEmpty(uid)) return;

        DaoService.Instance.GetMusicAndSFXVolumeSettings(uid, data =>
        {
            if (data != null)
            {
                float musicVol = data.ContainsKey("music-volume") ? data["music-volume"] : 0.7f;
                float sfxVol = data.ContainsKey("sfx-volume") ? data["sfx-volume"] : 0.7f;

                Debug.Log("Music Volume: " + musicVol);
                Debug.Log("SFX Volume: " + sfxVol);

                musicSlider.value = musicVol;
                sfxSlider.value = sfxVol;

                SoundManager.Instance.MusicVolume(musicVol);
                SoundManager.Instance.SFXVolume(sfxVol);
            }
        });
    }

    private void SaveMusicVolumeAndSFXVolume()
    {
        string uid = authService.GetUser()?.UserId;

        float musicVolume = musicSlider.value;
        float sfxVolume = sfxSlider.value;

        if (string.IsNullOrEmpty(uid)) return;
        DaoService.Instance.SaveMusicAndSFXVolumeSettings(uid, musicVolume, sfxVolume, success =>
        {
            Debug.Log("SUCCESS STATUS: " + success.ToString());
        });
    }


    public void MusicVolume()
    {
        float musicVolume = musicSlider.value;
        SoundManager.Instance.MusicVolume(musicVolume);
    }

    public void SFXVolume()
    {
        float sfxVolume = sfxSlider.value;
        Debug.Log(sfxVolume);
        SoundManager.Instance.SFXVolume(sfxVolume);
    }
}

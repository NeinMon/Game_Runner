using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{


    public GameObject closeBtn;

    // [Header("Audio")]
    // public AudioMixer audioMixer;
    // public Slider themeVolumeSlider;
    // public Slider effectVolumeSlider;

    // private const string THEME_KEY = "ThemeVolume";
    // private const string EFFECT_KEY = "EffectVolume";

    void Start()
    {
        // // Load saved values or set defaults
        // float savedTheme = PlayerPrefs.GetFloat(THEME_KEY, 0.75f);
        // float savedEffect = PlayerPrefs.GetFloat(EFFECT_KEY, 0.75f);

        // themeVolumeSlider.value = savedTheme;
        // effectVolumeSlider.value = savedEffect;

        // SetThemeVolume(savedTheme);
        // SetEffectVolume(savedEffect);


        if (closeBtn != null)
        {
            Button btn = closeBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(Close);
            }
        }
    }


    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // public void SetThemeVolume(float volume)
    // {
    //     audioMixer.SetFloat("ThemeVolume", Mathf.Log10(volume) * 20);
    //     PlayerPrefs.SetFloat(THEME_KEY, volume);
    // }

    // public void SetEffectVolume(float volume)
    // {
    //     audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
    //     PlayerPrefs.SetFloat(EFFECT_KEY, volume);
    // }
}

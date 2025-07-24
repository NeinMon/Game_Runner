using UnityEngine;
using System;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }


    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    private float sfxVolume = 0.7f;
    private float musicVolume = 0.7f;

    public AudioClip musicClip;
    public AudioClip coinClip;
    public AudioClip powerUpClip;
    public AudioClip pressBtnClip;
    public AudioClip freezeClip;
    public AudioClip collideClip;



    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        LoadMusicVolumeAndSFXVolume(() =>
        {
            musicAudioSource.clip = musicClip;
            musicAudioSource.Play();
        });
    }

    public void MusicVolume(float newVolume)
    {
        musicVolume = newVolume;
        musicAudioSource.volume = newVolume;
    }
    public void SFXVolume(float newVolume)
    {
        sfxVolume = newVolume;
        sfxAudioSource.volume = newVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void PlayCoinSFX()
    {
        sfxAudioSource.PlayOneShot(coinClip);
    }

    public void PlayPowerUpSFX()
    {
        sfxAudioSource.PlayOneShot(powerUpClip);
    }

    public void PlayFreezeSFX()
    {
        sfxAudioSource.PlayOneShot(freezeClip);
    }

    public void PlayCollideSFX()
    {
        sfxAudioSource.PlayOneShot(collideClip);
    }

    public void PlayPressBtnSFX()
    {
        sfxAudioSource.PlayOneShot(pressBtnClip);
    }

    public void LoadMusicVolumeAndSFXVolume(Action onCompleted = null)
    {
        Debug.Log("Hi HERE");
        string uid = AuthService.Instance.GetUser()?.UserId;
        if (string.IsNullOrEmpty(uid))
        {
            onCompleted.Invoke();
            return;
        }

        DaoService.Instance.GetMusicAndSFXVolumeSettings(uid, data =>
        {
            if (data != null)
            {
                float musicVol = data.ContainsKey("music-volume") ? data["music-volume"] : musicVolume;
                float sfxVol = data.ContainsKey("sfx-volume") ? data["sfx-volume"] : sfxVolume;

                MusicVolume(musicVol);
                SFXVolume(sfxVol);
            }

            onCompleted.Invoke();
        });
    }


    public void SaveMusicVolumeAndSFXVolume()
    {
        string uid = AuthService.Instance.GetUser()?.UserId;

        if (string.IsNullOrEmpty(uid)) return;
        DaoService.Instance.SaveMusicAndSFXVolumeSettings(uid, musicVolume, sfxVolume, success =>
        {
            Debug.Log("SUCCESS STATUS: " + success.ToString());
        });
    }
}

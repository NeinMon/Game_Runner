using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }


    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    public float sfxVolume = 0.7f;
    public float musicVolume = 0.7f;

    public AudioClip musicClip;
    public AudioClip coinClip;
    public AudioClip powerUpClip;
    public AudioClip pressBtnClip;

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
        musicAudioSource.clip = musicClip;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.Play();
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

    public void PlayCoinSFX()
    {
        sfxAudioSource.clip = coinClip;
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.PlayOneShot(coinClip);
    }

    public void PlayPowerUpSFX()
    {
        sfxAudioSource.clip = powerUpClip;
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.PlayOneShot(powerUpClip);
    }

    public void PlayPressBtnSFX()
    {
        sfxAudioSource.clip = pressBtnClip;
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.PlayOneShot(pressBtnClip);
    }



}

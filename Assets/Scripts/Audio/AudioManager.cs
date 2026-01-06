using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadVolume();
    }

    public void SetMasterVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(MIXER_MASTER, volume);

        PlayerPrefs.SetFloat(MIXER_MASTER, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(MIXER_MUSIC, volume);

        PlayerPrefs.SetFloat(MIXER_MUSIC, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(MIXER_SFX, volume);

        PlayerPrefs.SetFloat(MIXER_SFX, value);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        float master = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
        float music = PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
        float sfx = PlayerPrefs.GetFloat(MIXER_SFX, 1f);

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(MIXER_SFX, 1f);
}

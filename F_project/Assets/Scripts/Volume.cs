using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{

    [SerializeField] AudioMixer master;
    [SerializeField] private Slider sfxAudio, musicAudio;
    public bool isMute;
    public string musicSavedValue = "musicValue";  // Clave para guardar el volumen de la música en PlayerPrefs
    public string sfxSavedValue = "sfxValue";

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadSoundPreferences();
        }

        else
        {
            MusicVolumeControl();
            SFXVolumeControl();
        }

    }
    // Controla el volumen de la música
    public void MusicVolumeControl()
    {
        float volume = musicAudio.value;

        // Evita Log10(0) usando un mínimo de 0.0001f
        float adjustedVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;

        master.SetFloat("Music Volume", adjustedVolume);

        PlayerPrefs.SetFloat("musicVolume", volume);
    }


    // Controla el volumen de los efectos de sonido
    public void SFXVolumeControl()
    {
        float volume = sfxAudio.value;

        float adjustedVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        master.SetFloat("SFX Volume", adjustedVolume);

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // Silencia el sonido 
    public void MuteAll()
    {
        isMute = !isMute; // Alterna entre silencio y sonido
        if (isMute)
        {
            master.SetFloat("Master Volume", -80f); // Silencia todo el audio
        }
        else
        {
            master.SetFloat("Master Volume", 0f); // Restaura el volumen
        }
    }

    public void SaveSoundPreferences(float levelMusic, float levelSFX)
    {

        PlayerPrefs.SetFloat(musicSavedValue, levelMusic);
        PlayerPrefs.SetFloat(sfxSavedValue, levelSFX);
    }

    // Carga las preferencias de volumen guardadas
    public void LoadSoundPreferences()
    {
        musicAudio.value = PlayerPrefs.GetFloat("musicVolume");
        sfxAudio.value = PlayerPrefs.GetFloat("SFXVolume");

        MusicVolumeControl();
        SFXVolumeControl();
    }
}

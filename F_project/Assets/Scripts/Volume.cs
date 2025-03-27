using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private AudioMixer master;
    [SerializeField] private Slider sfxAudio, musicAudio, masterAudio;
    public GameObject muteCheck; // Imagen del mute

    public bool isMute;

    private const string musicPref = "musicVolume";
    private const string sfxPref = "SFXVolume";
    private const string masterPref = "masterAudio";

    void Start()
    {
        // Solo carga los valores al inicio de la escena si es necesario
        LoadVolumePreferences();
    }

    // Método que debes llamar al abrir el panel de sonido
    public void LoadVolumePreferences()
    {
        // Obtiene lo que esté guardado o asigna 0.5f por defecto
        float music = PlayerPrefs.GetFloat(musicPref, 0.5f);
        float sfx = PlayerPrefs.GetFloat(sfxPref, 0.5f);
        float masterVol = PlayerPrefs.GetFloat(masterPref, 0.5f);

        // Asigna valores a los sliders sin bajar el volumen
        musicAudio.value = music;
        sfxAudio.value = sfx;
        masterAudio.value = masterVol;

        // Aplica valores cargados
        ApplyVolume("Music Volume", music);
        ApplyVolume("SFX Volume", sfx);
        ApplyVolume("Master Volume", masterVol);
    }

    // Método genérico para aplicar volumen
    private void ApplyVolume(string parameterName, float value)
    {
        float adjustedVolume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        master.SetFloat(parameterName, adjustedVolume);
    }

    public void MasterVolumeControl()
    {
        float value = masterAudio.value;
        ApplyVolume("Master Volume", value);
        PlayerPrefs.SetFloat(masterPref, value);
    }

    public void MusicVolumeControl()
    {
        float value = musicAudio.value;
        ApplyVolume("Music Volume", value);
        PlayerPrefs.SetFloat(musicPref, value);
    }

    public void SFXVolumeControl()
    {
        float value = sfxAudio.value;
        ApplyVolume("SFX Volume", value);
        PlayerPrefs.SetFloat(sfxPref, value);
    }

    // Alterna entre mute y volumen normal
    public void MuteAll()
    {
        isMute = !isMute;
        if (isMute)
        {
            master.SetFloat("Master Volume", -80f); // Silencia todo
            muteCheck.SetActive(true);
        }
        else
        {
            // Vuelve a aplicar el volumen guardado
            MasterVolumeControl();
            muteCheck.SetActive(false);
        }
    }
}

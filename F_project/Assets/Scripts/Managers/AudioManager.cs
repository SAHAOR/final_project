using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioMixer master;
    [SerializeField] private AudioSource sfxAudio, musicAudio;
    [SerializeField] private Volume volumeController; // Ahora se asigna en el Inspector
    public Sound[] musicSounds, sfxSounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Si la referencia no está asignada en el Inspector, buscar automáticamente
        if (volumeController == null)
        {
            volumeController = FindFirstObjectByType<Volume>();
        }

        if (volumeController != null)
        {
            volumeController.LoadVolumePreferences(); // Cargar volumen guardado
        }
        else
        {
            Debug.LogWarning("No se encontró el script Volume en la escena.");
        }

        PlayMusic("example menu theme 2"); // Reproduce la música del menú con el volumen correcto
    }



    public void PlaySfx(string name)
    {
        Sound sfx = Array.Find(sfxSounds, x => x.nameSound == name);
        if (sfx == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxAudio.PlayOneShot(sfx.clip);
        }
    }

    public void PlayMusic(string name)
    {
        Sound music = Array.Find(musicSounds, x => x.nameSound == name);
        if (music == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicAudio.clip = music.clip;
            musicAudio.Play();
            musicAudio.loop = true;
        }
    }

    public void RestartMusic()
    {
        musicAudio.Stop();
        musicAudio.Play();
    }
}

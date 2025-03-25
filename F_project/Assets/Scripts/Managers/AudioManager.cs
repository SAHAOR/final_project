using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance { get; private set; }
    [SerializeField] AudioMixer master;
    [SerializeField] AudioSource  sfxAudio, musicAudio;
    public Sound[] musicSounds, sfxSounds;

  
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        
        }
    }

    void Start()
    {
        PlayMusic("example menu theme");
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
            Debug.Log(music);
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

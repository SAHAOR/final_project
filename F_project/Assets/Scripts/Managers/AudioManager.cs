using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton para asegurar que solo haya una instancia del AudioManager en la escena
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioMixer master; // Controlador del audio mixer
    [SerializeField] private AudioSource sfxAudio, musicAudio; // AudioSources para efectos de sonido y m�sica
    [SerializeField] private Volume volumeController; // Referencia al controlador de volumen
    public Sound[] musicSounds, sfxSounds; // Arreglos que almacenan los sonidos de m�sica y efectos

    private void Awake()
    {
        // Asegura que solo haya una instancia de AudioManager y persista entre escenas
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


        // Si no se ha asignado manualmente, intenta encontrar el controlador de volumen en la escena
        if (volumeController == null)
        {
            volumeController = FindFirstObjectByType<Volume>();
        }

        // Si el controlador de volumen existe, carga las preferencias guardadas
        if (volumeController != null)
        {
            volumeController.LoadVolumePreferences();
        }
        else
        {
            Debug.LogWarning("No se encontr� el script Volume en la escena.");
        }

        // Se suscribe al evento de carga de escena para cambiar la m�sica autom�ticamente
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Si esta es la primera escena al iniciar el juego, se asegura de que la m�sica inicie
        PlayMusicByScene();

    }

    // M�todo que se llama cuando se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicByScene();
    }

    // Determina qu� m�sica reproducir seg�n la escena actual
    public void PlayMusicByScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "Felipe":
                PlayMusic("example menu theme 2"); // Reproduce la m�sica asignada al men�
                break;
            case "UIGameScene":
                PlayMusic("Game Theme"); // Reproduce la m�sica asignada al juego
                break;
            default:
                Debug.LogWarning($"No se ha asignado una musica para la escena {sceneName}");
                break;
        }
    }

    // M�todo para reproducir un efecto de sonido (SFX) seg�n el nombre
    public void PlaySfx(string name)
    {
        // Busca el sonido en el array de efectos de sonido
        Sound sfx = Array.Find(sfxSounds, x => x.nameSound == name);
        if (sfx == null)
        {
            Debug.Log("SFX Not Found");
        }
        else
        {
            sfxAudio.PlayOneShot(sfx.clip); // Reproduce el sonido sin interrumpir otros sonidos
        }
    }

    // M�todo para reproducir m�sica seg�n el nombre
    public void PlayMusic(string name)
    {
        // Busca la m�sica en el array de m�sica
        Sound music = Array.Find(musicSounds, x => x.nameSound == name);
        if (music == null)
        {
            Debug.Log("Music Not Found");
        }
        // Evita reiniciar la misma canci�n si ya est� sonando
        if (musicAudio.clip == music.clip && musicAudio.isPlaying) return;

        musicAudio.Stop();
        musicAudio.clip = music.clip;
        musicAudio.Play();
        musicAudio.loop = true;
    }

    // M�todo para reiniciar la m�sica actual
    public void RestartMusic()
    {
        musicAudio.Stop();
        musicAudio.Play();
    }
}

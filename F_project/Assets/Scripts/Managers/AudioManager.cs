using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton para asegurar que solo haya una instancia del AudioManager en la escena
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioMixer master; // Controlador del audio mixer
    [SerializeField] private AudioSource sfxAudio, musicAudio; // AudioSources para efectos de sonido y música
    [SerializeField] private Volume volumeController; // Referencia al controlador de volumen
    public Sound[] musicSounds, sfxSounds; // Arreglos que almacenan los sonidos de música y efectos

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
            Debug.LogWarning("No se encontró el script Volume en la escena.");
        }

        // Se suscribe al evento de carga de escena para cambiar la música automáticamente
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Si esta es la primera escena al iniciar el juego, se asegura de que la música inicie
        PlayMusicByScene();

    }

    // Método que se llama cuando se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicByScene();
    }

    // Determina qué música reproducir según la escena actual
    public void PlayMusicByScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "Felipe":
                PlayMusic("example menu theme 2"); // Reproduce la música asignada al menú
                break;
            case "UIGameScene":
                PlayMusic("Game Theme"); // Reproduce la música asignada al juego
                break;
            default:
                Debug.LogWarning($"No se ha asignado una música para la escena {sceneName}");
                break;
        }
    }

    // Método para reproducir un efecto de sonido (SFX) según el nombre
    public void PlaySfx(string name)
    {
        // Busca el sonido en el array de efectos de sonido
        Sound sfx = Array.Find(sfxSounds, x => x.nameSound == name);
        if (sfx == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxAudio.PlayOneShot(sfx.clip); // Reproduce el sonido sin interrumpir otros sonidos
        }
    }

    // Método para reproducir música según el nombre
    public void PlayMusic(string name)
    {
        // Busca la música en el array de música
        Sound music = Array.Find(musicSounds, x => x.nameSound == name);
        if (music == null)
        {
            Debug.Log("Sound Not Found");
        }
        // Evita reiniciar la misma canción si ya está sonando
        if (musicAudio.clip == music.clip && musicAudio.isPlaying) return;

        musicAudio.Stop();
        musicAudio.clip = music.clip;
        musicAudio.Play();
        musicAudio.loop = true;
    }

    // Método para reiniciar la música actual
    public void RestartMusic()
    {
        musicAudio.Stop();
        musicAudio.Play();
    }
}

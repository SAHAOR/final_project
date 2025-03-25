using UnityEngine;

public class BrilloManager : MonoBehaviour
{
    public static BrilloManager Instance { get; private set; }
    private float brilloActual = 0.35f;

    //garantiza que solo haya una instancia de un objeto en la escena
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            brilloActual = PlayerPrefs.GetFloat("Brillo", 0.35f);
        }
        else
        {
            Destroy(gameObject);
        }
        SetBrillo(PlayerPrefs.GetFloat("Brillo"));
    }

    //actualiza y guarda el nivel de brillo en (PlayerPrefs)
    public void SetBrillo(float valor)
    {
        brilloActual = valor;
        PlayerPrefs.SetFloat("Brillo", brilloActual); // Guardar en memoria
        PlayerPrefs.Save();
    }

    //devuelve el valor actual del brillo almacenado en la variable (brilloActual)
    public float GetBrillo()
    {
        return brilloActual;
    }
}

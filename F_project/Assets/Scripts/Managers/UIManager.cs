using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] private GameObject pantallaInicio;
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuHow;
    [SerializeField] private GameObject menuOpciones;
    [SerializeField] private GameObject menuCreditos;

    private void Awake()
    {
        // Asegura que solo haya una instancia de AudioManager y persista entre escenas
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }


    }


    // M�todo gen�rico para mostrar cualquier panel y ocultar los dem�s
    private void MostrarSoloEsteMenu(GameObject menuActivo)
    {
        pantallaInicio.SetActive(false);
        menuPrincipal.SetActive(false);
        menuHow.SetActive(false);
        menuOpciones.SetActive(false);

        menuActivo.SetActive(true);
    }

    // Bot�n: Mostrar el Men� Principal
    public void MostrarMenuPrincipal()
    {
        MostrarSoloEsteMenu(menuPrincipal);
    }

    // Bot�n: Mostrar el men� How To Play
    public void MostrarMenuHow()
    {
        MostrarSoloEsteMenu(menuHow);
    }

    // Bot�n: Mostrar el men� de Opciones
    public void MostrarMenuOpciones()
    {
        MostrarSoloEsteMenu(menuOpciones);
    }
    public void MostrarMenuCreditos()
    {
        MostrarSoloEsteMenu(menuCreditos);
    }

    // Bot�n: Cargar la escena del juego
    public void CargarEscenaJuego(string NombreMenu)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(NombreMenu, LoadSceneMode.Single);
    }
}

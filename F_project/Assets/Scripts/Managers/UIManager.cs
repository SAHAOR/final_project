using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pantallaInicio;
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuHow;
    [SerializeField] private GameObject menuOpciones;
    [SerializeField] private GameObject menuCreditos;

    // Método genérico para mostrar cualquier panel y ocultar los demás
    private void MostrarSoloEsteMenu(GameObject menuActivo)
    {
        pantallaInicio.SetActive(false);
        menuPrincipal.SetActive(false);
        menuHow.SetActive(false);
        menuOpciones.SetActive(false);

        menuActivo.SetActive(true);
    }

    // Botón: Mostrar el Menú Principal
    public void MostrarMenuPrincipal()
    {
        MostrarSoloEsteMenu(menuPrincipal);
    }

    // Botón: Mostrar el menú How To Play
    public void MostrarMenuHow()
    {
        MostrarSoloEsteMenu(menuHow);
    }

    // Botón: Mostrar el menú de Opciones
    public void MostrarMenuOpciones()
    {
        MostrarSoloEsteMenu(menuOpciones);
    }
    public void MostrarMenuCreditos()
    {
        MostrarSoloEsteMenu(menuCreditos);
    }

    // Botón: Cargar la escena del juego
    public void CargarEscenaJuego()
    {
        SceneManager.LoadScene(1); // O usa el nombre con SceneManager.LoadScene("NombreEscena");
    }
}

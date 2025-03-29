using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int maxFruits = 100;
    private int currentFruits = 1;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1; // Asegurar que el juego arranque activo
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public bool CanSpawnFruit()
    {
        return !isPaused && currentFruits < maxFruits;
    }

    public void AddFruit()
    {
        if (currentFruits < maxFruits)
        {
            currentFruits++;
            Debug.Log("Frutas totales: " + currentFruits);

            if (currentFruits >= maxFruits)
            {
                WinGame();
            }
        }
    }

    public void RemoveFruit()
    {
        currentFruits = Mathf.Max(0, currentFruits - 1); // No puede ser menor que 0
        Debug.Log("Frutas totales: " + currentFruits);

        if (currentFruits <= 0)
        {
            LoseGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("¡Has ganado!");
        Time.timeScale = 0; // Pausar juego al ganar
    }

    private void LoseGame()
    {
        Debug.Log("¡Has perdido!");
        Time.timeScale = 0; // Pausar juego al perder
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        Debug.Log(isPaused ? "Juego en pausa" : "Juego reanudado");
    }
}

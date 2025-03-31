using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject powerUpPrefab; // Prefab del Power Up
    public float spawnDelay = 15f; // Tiempo hasta que aparece (15 segundos)
    public float activeDuration = 5f; // Duración del Power Up activo
    public Transform spawnArea; // Área de spawn del Power Up

    private bool isPowerUpActive = false; // Estado del Power Up activo
    private GameObject currentPowerUp; // Referencia al Power Up activo

    public static PowerUpManager Instance; // Singleton para acceder al estado del Power Up

    private void Awake()
    {
        Instance = this; // Inicializamos el Singleton
    }

    private void Start()
    {
        Invoke(nameof(SpawnPowerUp), spawnDelay); // Invoca el spawn después de 15 segundos
    }

    private void SpawnPowerUp()
    {
        // Generar el Power Up en una posición aleatoria dentro del área
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnArea.position.x - 1, spawnArea.position.x + 1),
            Random.Range(spawnArea.position.y - 1, spawnArea.position.y + 1),
            0
        );

        currentPowerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
    }

    public void ActivatePowerUp()
    {
        if (currentPowerUp != null)
        {
            Destroy(currentPowerUp); // Destruir el Power Up al presionarlo
            currentPowerUp = null;  // Limpiar la referencia para evitar problemas
        }

        isPowerUpActive = true; // Activar el Power Up
        Invoke(nameof(DeactivatePowerUp), activeDuration); // Desactivar después de 5 segundos
    }

    private void DeactivatePowerUp()
    {
        isPowerUpActive = false; // Desactivar el Power Up
    }

    public bool IsPowerUpActive()
    {
        return isPowerUpActive; // Retorna si el Power Up está activo
    }
}
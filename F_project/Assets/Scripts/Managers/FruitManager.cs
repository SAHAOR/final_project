using UnityEngine;

public class FruitManager : MonoBehaviour
{
    public GameObject fruitPrefab;
    public float spawnDistance = 1.0f;
    private bool isMainFruit = true;
    [SerializeField] float destroyFruitSeconds = 5f;

    private void OnMouseDown()
    {
        if (isMainFruit && GameManager.Instance != null && GameManager.Instance.CanSpawnFruit())
        {
            SpawnNewFruit();
        }
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * 100 * Time.deltaTime); // Gira hacia la derecha
    }

    private void SpawnNewFruit()
    {
        if (GameManager.Instance == null || !GameManager.Instance.CanSpawnFruit()) return;

        int attempts = 10;
        Vector3 newPosition;

        do
        {
            float offsetX = Random.Range(-spawnDistance, spawnDistance);
            float offsetY = Random.Range(-spawnDistance, spawnDistance);
            newPosition = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
            attempts--;
        }
        while (IsPositionOccupied(newPosition) && attempts > 0);

        if (attempts > 0)
        {
            GameObject newFruit = Instantiate(fruitPrefab, newPosition, Quaternion.identity);
            newFruit.transform.localScale *= 0.5f;

            FruitManager newFruitScript = newFruit.GetComponent<FruitManager>();
            newFruitScript.isMainFruit = false;

            GameManager.Instance.AddFruit(); // Actualizar el contador de frutas

            Destroy(newFruit, destroyFruitSeconds); // Eliminar la banana después de 3 segundos
        }
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Fruit")) return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        if (!isMainFruit && GameManager.Instance != null)
        {
            GameManager.Instance.RemoveFruit();
        }
    }
}

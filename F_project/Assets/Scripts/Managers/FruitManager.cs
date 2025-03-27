using UnityEngine;
using System.Collections;

public class FruitManager : MonoBehaviour
{
    public GameObject fruitPrefab;
    public float spawnDistance = 2.0f; // Mayor distancia de aparición
    public float moveSpeed = 10.0f; // Velocidad alta para un flujo más frenético

    [SerializeField] private float destroyFruitSeconds = 5f;
    [SerializeField] private float disableTime = 1.5f;
    [SerializeField] private Color disabledColor = Color.gray;
    [SerializeField] private Color grownColor = Color.yellow;

    private Rigidbody rb;
    private Renderer fruitRenderer;
    private Color originalColor;
    private bool isMainFruit = true;
    private bool hasBeenClicked = false;
    private bool isDisabled = false;
    private bool isGrowing = false;

    private void Start()
    {
        // Inicialización del Rigidbody y Renderer
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody no está asignado al objeto: " + gameObject.name);
            return;
        }
        rb.freezeRotation = true;

        fruitRenderer = GetComponent<Renderer>();
        if (fruitRenderer == null)
        {
            Debug.LogError("Renderer no está asignado al objeto: " + gameObject.name);
            return;
        }

        // Establece y aplica el color inicial
        originalColor = fruitRenderer.material.color;
        fruitRenderer.material.color = originalColor;

        // Inicializa el movimiento dependiendo si es principal o hija
        if (!isMainFruit || hasBeenClicked)
        {
            InitializeFruitMovement(); // Activa el movimiento si no es principal o si ya fue clicada
        }
    }

    private void Update()
    {
        RotateFruit();
    }

    private void FixedUpdate()
    {
        ManageFruitMovement();
    }

    private void OnMouseDown()
    {
        if (isDisabled) return;
        HandleMouseClick();
    }

    private void RotateFruit()
    {
        transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }

    private void InitializeFruitMovement()
    {
        if (rb != null && !rb.isKinematic)
        {
            SetRandomDirection();
            InvokeRepeating(nameof(SetRandomDirection), 1f, 1f); // Cambiar dirección más frecuentemente
        }
    }

    private void ManageFruitMovement()
    {
        if (rb != null && (hasBeenClicked || !isMainFruit) && !rb.isKinematic)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }

    private void HandleMouseClick()
    {
        if (GameManager.Instance == null || !GameManager.Instance.CanSpawnFruit()) return;

        if (isMainFruit && !hasBeenClicked)
        {
            hasBeenClicked = true;
            StartCoroutine(DisableTemporarily());
        }

        SpawnNewFruit();
    }

    private IEnumerator DisableTemporarily()
    {
        isDisabled = true;
        fruitRenderer.material.color = disabledColor;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(disableTime);

        fruitRenderer.material.color = originalColor;

        if (rb != null)
        {
            rb.isKinematic = false;
            SetRandomDirection();
        }
        isDisabled = false;
    }

    private void SpawnNewFruit()
    {
        if (GameManager.Instance == null || !GameManager.Instance.CanSpawnFruit()) return;

        Vector3 newPosition = GenerateNewPosition();

        if (newPosition != Vector3.zero)
        {
            // Crear la primera fruta
            GameObject newFruit = Instantiate(fruitPrefab, newPosition, Quaternion.identity);
            SetupFruit(newFruit);

            // Crear una segunda fruta si el Power Up está activo
            if (PowerUpManager.Instance != null && PowerUpManager.Instance.IsPowerUpActive())
            {
                Vector3 secondPosition = GenerateNewPosition();
                if (secondPosition != Vector3.zero)
                {
                    GameObject secondFruit = Instantiate(fruitPrefab, secondPosition, Quaternion.identity);
                    SetupFruit(secondFruit);
                }
            }
        }
    }

    private void SetupFruit(GameObject fruit)
    {
        fruit.transform.localScale = Vector3.zero;
        FruitManager fruitScript = fruit.GetComponent<FruitManager>();
        if (fruitScript != null)
        {
            fruitScript.isMainFruit = false;
            fruitScript.hasBeenClicked = true; // Activar movimiento desde el inicio
            fruitScript.InitializeFruitMovement(); // Garantizar dirección inicial
            fruitScript.SetInitialColor(originalColor); // Color inicial correcto

            StartCoroutine(fruitScript.GrowFruit());
            GameManager.Instance.AddFruit();
            Destroy(fruit, destroyFruitSeconds);
        }
    }

    private Vector3 GenerateNewPosition()
    {
        int attempts = 10;
        Vector3 newPosition;

        do
        {
            float offsetX = Random.Range(-spawnDistance, spawnDistance);
            float offsetY = Random.Range(-spawnDistance, spawnDistance);
            newPosition = transform.position + new Vector3(offsetX, offsetY, 0);
            attempts--;
        } while (IsPositionOccupied(newPosition) && attempts > 0);

        return attempts > 0 ? newPosition : Vector3.zero;
    }

    private IEnumerator GrowFruit()
    {
        isGrowing = true;
        float elapsedTime = 0f;
        Vector3 targetScale = Vector3.one * 1.2f;

        while (elapsedTime < 2f) // Incrementar a 2 segundos el tiempo de crecimiento
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        fruitRenderer.material.color = grownColor;
        isGrowing = false;
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        return Physics.OverlapSphere(position, 0.5f).Length > 0;
    }

    private void SetRandomDirection()
    {
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = new Vector3(
                Random.Range(-2f, 2f), // Mayor rango de movimiento
                Random.Range(-2f, 2f),
                0f
            ).normalized * moveSpeed;
        }
    }

    public void SetInitialColor(Color initialColor)
    {
        if (fruitRenderer != null)
        {
            fruitRenderer.material.color = initialColor; // Aplicar el color inicial correcto
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Rebote cuando las bananas choquen entre ellas
        if (collision.gameObject.CompareTag("Fruit") && rb != null)
        {
            // Asegurarse de que el Rigidbody no sea cinemático antes de establecer la dirección
            if (!rb.isKinematic)
            {
                Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity, collision.contacts[0].normal);
                rb.linearVelocity = reflectDir.normalized * moveSpeed;
            }
        }
    }
}
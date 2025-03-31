using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    public GameObject fruitPrefab;    // Prefab de la fruta
    public float fruitSize = 1f;      // Tamaño final de la fruta
    public float growthTime = 2f;     // Tiempo para alcanzar el tamaño final
    public float lifetime = 10f;      // Tiempo antes de que desaparezca una fruta
    public float movementSpeed = 3f;  // Velocidad de movimiento
    public float rotationSpeed = 50f; // Velocidad de rotación uniforme para todas las frutas
    private bool isDisabled = false;  // Estado de desactivación
    private bool isFirstClickHandled = false; // Indica si la fruta principal ya recibió el primer clic
    public Vector3 direction;         // Dirección de movimiento
    public float directionChangeInterval = 0.5f; // Intervalo de cambio de dirección

    private void Start()
    {
        // Configuración inicial de la dirección (la primera banana estática)
        if (!isFirstClickHandled)
        {
            direction = Vector3.zero; // La fruta principal comienza sin moverse
        }
        else
        {
            StartCoroutine(ChangeDirectionPeriodically()); // Cambios aleatorios en la dirección
        }
    }

    private void Update()
    {
        if (!isDisabled && (isFirstClickHandled || direction != Vector3.zero))
        {
            MoveFruit();
        }

        // Rotación continua alrededor del eje Y
        RotateFruit();
    }

    private void OnMouseDown()
    {
        // Bloquear interacción si la fruta está deshabilitada
        if (isDisabled) return;

        // Comportamiento exclusivo para la fruta principal en su primer clic
        if (!isFirstClickHandled)
        {
            StartCoroutine(DisableFruitOnFirstClick());
            isFirstClickHandled = true; // Marcar que ya fue activada
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized; // Asignar movimiento
            StartCoroutine(ChangeDirectionPeriodically()); // Iniciar cambios aleatorios en dirección
        }
        else
        {
            SpawnNewFruit(); // Generar frutas hijas al clic
        }
    }

    private IEnumerator DisableFruitOnFirstClick()
    {
        isDisabled = true;
        GetComponent<Renderer>().material.color = Color.red; // Cambiar color al deshabilitar
        yield return new WaitForSeconds(2f); // Deshabilitado por 2 segundos
        GetComponent<Renderer>().material.color = Color.white; // Restaurar color
        isDisabled = false;
    }

    private void MoveFruit()
    {
        // Movimiento en la dirección actual dentro del plano X-Y
        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    private void RotateFruit()
    {
        // Rotación continua alrededor del eje Y con una velocidad fija
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Rebote al colisionar con objetos con el tag "Wall"
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Reflejar la dirección según el punto de contacto
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            direction.z = 0; // Asegurar que no haya movimiento en el eje Z
        }
    }

    private void SpawnNewFruit()
    {
        // Crear una nueva fruta hija
        GameObject newFruit = Instantiate(fruitPrefab, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), Quaternion.identity);

        // Configurar movimiento para la fruta hija
        FruitManager fruitManager = newFruit.GetComponent<FruitManager>();
        if (fruitManager != null)
        {
            fruitManager.direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized; // Asignar dirección aleatoria
            fruitManager.isFirstClickHandled = true; // Marcarla como activada para que se mueva
            fruitManager.StartCoroutine(fruitManager.ChangeDirectionPeriodically()); // Cambios de dirección aleatorios
        }

        newFruit.transform.localScale = Vector3.zero; // Iniciar la fruta hija en tamaño cero
        StartCoroutine(GrowFruit(newFruit)); // Activar el crecimiento gradual
        Destroy(newFruit, lifetime); // Destruir la fruta hija después del tiempo de vida
    }

    private IEnumerator GrowFruit(GameObject fruit)
    {
        float elapsedTime = 0f;
        while (elapsedTime < growthTime)
        {
            fruit.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * fruitSize, elapsedTime / growthTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fruit.transform.localScale = Vector3.one * fruitSize; // Asegurar que alcance el tamaño final
    }

    private IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            // Cambiar la dirección aleatoriamente cada cierto intervalo
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }
}
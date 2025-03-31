using UnityEngine;
using Photon.Pun;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public float baseSpeed = 5f; // Velocidad normal de movimiento
    public float reboundForce = 10f; // Fuerza extra en el rebote
    private Vector3 direction; // Dirección actual del movimiento
    private Rigidbody rb;
    private PhotonView photonView;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PhotonNetwork.SendRate = 60; // Número de paquetes enviados por segundo
        PhotonNetwork.SerializationRate = 30; // Número de actualizaciones por segundo
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient) // Solo el dueño del objeto controla el movimiento
        {
            direction = GetRandomDirection();
            rb.linearVelocity = direction * baseSpeed;

            photonView.RPC("SyncDirection", RpcTarget.OthersBuffered, direction);
        }
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Mantener el objeto en movimiento y ajustar velocidad gradualmente
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, direction * baseSpeed, Time.deltaTime * 5f);
            //StartCoroutine(DelayedSync());
            photonView.RPC("SyncVelocity", RpcTarget.OthersBuffered, rb.linearVelocity);
            photonView.RPC("SyncDirection", RpcTarget.OthersBuffered, direction);
        }
        
    }

    

    void OnCollisionEnter(Collision collision)
    {
        //if(PhotonNetwork.IsMasterClient)
        //{
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Object")) // Si choca con una pared
            {
                Vector3 normal = collision.contacts[0].normal; // Normal de la colisión
                direction = Vector3.Reflect(direction, normal).normalized; // Cambiar dirección
                //photonView.RPC("SyncDirection", RpcTarget.OthersBuffered, direction);
                // Aplicar un impulso extra para rebote más fuerte
                rb.linearVelocity = Vector3.zero; // Evitar acumulación de velocidad
                rb.AddForce(direction * reboundForce, ForceMode.Impulse);

            }
        //}
    }

    [PunRPC]
    void SyncDirection(Vector3 newDirection)
    {
        direction = newDirection;
        //StartCoroutine(SmoothDirectionTransition(newDirection));
    }

    IEnumerator SmoothDirectionTransition(Vector3 targetDirection)
    {
        float duration = 0.1f; // Duración de la interpolación
        float elapsedTime = 0f;
        Vector3 startDirection = direction;

        while (elapsedTime < duration)
        {
            direction = Vector3.Lerp(startDirection, targetDirection, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        direction = targetDirection; // Asegurar que termine con la dirección correcta
    }

    [PunRPC]
    void SyncVelocity(Vector3 newVelocity)
    {
        rb.linearVelocity = newVelocity;
        //StartCoroutine(SmoothVelocityTransition(newVelocity));/
    }

    IEnumerator SmoothVelocityTransition(Vector3 targetVelocity)
    {
        float duration = 0.1f; // Duración de la interpolación
        float elapsedTime = 0f;
        Vector3 startVelocity = rb.linearVelocity;

        while (elapsedTime < duration)
        {
            rb.linearVelocity = Vector3.Lerp(startVelocity, targetVelocity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = targetVelocity; // Asegurar que termine con la velocidad correcta
    }

    IEnumerator DelayedSync()
    {
        yield return new WaitForSeconds(0.1f); // Esperar un poco antes de enviar la sincronización
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncVelocity", RpcTarget.OthersBuffered, rb.linearVelocity);
        }
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
    }
}

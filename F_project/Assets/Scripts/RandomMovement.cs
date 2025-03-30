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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

            photonView.RPC("SyncVelocity", RpcTarget.Others, rb.linearVelocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Object")) // Si choca con una pared
            {
                Vector3 normal = collision.contacts[0].normal; // Normal de la colisión
                direction = Vector3.Reflect(direction, normal).normalized; // Cambiar dirección

                // Aplicar un impulso extra para rebote más fuerte
                rb.linearVelocity = Vector3.zero; // Evitar acumulación de velocidad
                rb.AddForce(direction * reboundForce, ForceMode.Impulse);

                photonView.RPC("SyncDirection", RpcTarget.OthersBuffered, direction);
            }
        }
    }

    [PunRPC]
    void SyncDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }

    [PunRPC]
    void SyncVelocity(Vector3 newVelocity)
    {
        rb.linearVelocity = newVelocity;
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
    }
}

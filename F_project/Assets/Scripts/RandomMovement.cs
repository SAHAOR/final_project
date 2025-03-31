using UnityEngine;
using Photon.Pun;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float reboundForce = 10f;
    private Vector3 direction;
    private Rigidbody rb;
    private PhotonView photonView;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            direction = GetRandomDirection();
            rb.linearVelocity = direction * baseSpeed;

            photonView.RPC("SyncMovement", RpcTarget.Others, direction, rb.linearVelocity, Vector3.zero);
        }
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DelayedSync());
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, direction * baseSpeed, Time.deltaTime * 5f);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, rb.position, Time.deltaTime * 10f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Object"))
            {
                Vector3 normal = collision.contacts[0].normal;
                direction = Vector3.Reflect(direction, normal).normalized;

                rb.linearVelocity = Vector3.zero;

                Vector3 reboundImpulse = direction * reboundForce;
                rb.AddForce(reboundImpulse, ForceMode.Impulse);

                // Enviar dirección, velocidad y el impulso del rebote a los clientes
                photonView.RPC("SyncMovement", RpcTarget.Others, direction, rb.linearVelocity, reboundImpulse);
            }
        }
    }

    [PunRPC]
    void SyncMovement(Vector3 newDirection, Vector3 newVelocity, Vector3 impulse)
    {
        rb.AddForce(impulse, ForceMode.Impulse);
        StartCoroutine(SmoothDirectionTransition(newDirection));
        StartCoroutine(SmoothVelocityTransition(newVelocity));
    
    }

    IEnumerator SmoothDirectionTransition(Vector3 targetDirection)
    {
        float duration = 0.1f;
        float elapsedTime = 0f;
        Vector3 startDirection = direction;

        while (elapsedTime < duration)
        {
            direction = Vector3.Lerp(startDirection, targetDirection, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        direction = targetDirection;
    }

    IEnumerator SmoothVelocityTransition(Vector3 targetVelocity)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody es NULL en SmoothVelocityTransition en " + gameObject.name);
            yield break;
        }

        float duration = 0.1f;
        float elapsedTime = 0f;
        Vector3 startVelocity = rb.linearVelocity;

        while (elapsedTime < duration)
        {
            rb.linearVelocity = Vector3.Lerp(startVelocity, targetVelocity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = targetVelocity;
    }

    IEnumerator DelayedSync()
    {
        yield return new WaitForSeconds(0.05f);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncMovement", RpcTarget.Others, direction, rb.linearVelocity, Vector3.zero);
        }
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
    }
}

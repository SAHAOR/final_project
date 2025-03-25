using UnityEngine;
using Photon.Pun;

public class CubeMover : MonoBehaviourPun, IPunObservable
{
    public float speed = 3f;
    private Vector3 networkPosition;

    void Update()
    {
        if (photonView.IsMine) // Solo el dueño del objeto lo mueve
        {
            float move = Mathf.PingPong(Time.time * speed, 600) - 300; // Movimiento de -2 a 2
            transform.position = new Vector3(move, transform.position.y, transform.position.z);
            Debug.Log(move);
        }
        else // Si no soy el dueño, interpolar la posición
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5);
        }
    }

    // Sincroniza la posición del cubo con otros jugadores
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // Enviar datos
        {
            stream.SendNext(transform.position);
        }
        else // Recibir datos
        {
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }
}

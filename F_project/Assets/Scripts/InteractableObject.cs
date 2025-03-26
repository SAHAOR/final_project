using UnityEngine;
using Photon.Pun;

public class InteractableObject : MonoBehaviourPun
{
    public int owner; // ID del jugador dueño del objeto
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine) // Si el objeto es instanciado por la red, asignar el dueño
        {
            owner = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("SetOwner", RpcTarget.AllBuffered, owner);
        }

    }

    public void SetOwnerRPC(int newOwner)
{
    if (photonView == null)
    {
        Debug.LogError($"❌ PhotonView es NULL en {gameObject.name}, no se puede asignar el owner.");
        return;
    }

    photonView.RPC("SetOwner", RpcTarget.AllBuffered, newOwner);
}

    [PunRPC]
    void SetOwner(int newOwner)
    {
        owner = newOwner;
    }

    private void OnMouseDown()
    {
        //if (!photonView.IsMine) return; // Evitar que otros jugadores interactúen con el objeto de otro
        

        if (PhotonNetwork.LocalPlayer.ActorNumber == owner)
        {
            photonView.RPC("AddScore", RpcTarget.AllBuffered, owner);
        }
        else
        {
            photonView.RPC("SubtractScore", RpcTarget.AllBuffered, owner);
        }
    }

    [PunRPC]
    void AddScore(int playerID)
    {

        GameManager.instance.AddScore(playerID);
    
    }

    [PunRPC]
    void SubtractScore(int playerID)
    {
        
        GameManager.instance.SubtractScore(playerID);
        
    }
}
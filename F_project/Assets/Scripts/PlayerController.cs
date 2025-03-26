using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    
    private string myPrefab;


    void Start()
    {
        if (!photonView.IsMine) return;

        myPrefab = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "ApplePrefab" : "BananaPrefab";
        Debug.Log($"🎯 Prefab asignado al jugador {PhotonNetwork.LocalPlayer.ActorNumber}: {myPrefab}");

    }

    void Update()
    {
        if (!photonView.IsMine) return; // Solo el jugador dueño ejecuta esto

        if (Input.GetKeyDown(KeyCode.Space)) // Ejemplo: Crear un objeto al presionar espacio
        {
            Debug.Log($"🟢 {PhotonNetwork.NickName} detectó SPACE en Update()"); // Ver si se llama 2 veces
            RequestNewObject(myPrefab, transform.position + Vector3.up);
        }
    }

    public void RequestNewObject(string prefabName, Vector3 position)
    {
        if (!photonView.IsMine) return;

        Debug.Log($"📨 {PhotonNetwork.NickName} ejecutando RequestNewObject() para {prefabName}"); // Depuración

        if (string.IsNullOrEmpty(prefabName)) return;

        int myPlayerID = PhotonNetwork.LocalPlayer.ActorNumber; // Asegurar que el id del jugador es el correcto

        //ObjectSpawner.instance.SpawnNewObject(prefabName, position, myPlayerID);
        ObjectSpawner.instance.photonView.RPC("SpawnNewObject", RpcTarget.MasterClient, prefabName, position, myPlayerID);
    }
}


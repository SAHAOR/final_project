using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    
    private string myPrefab;
    public static PlayerController instance; //////////////////////////

    private void Awake() /////////////////
    {
        instance = this;//////////
    }


    void Start()
    {
        if (!photonView.IsMine) return;

        myPrefab = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "ApplePrefab" : "BananaPrefab";
        Debug.Log($"🎯 Prefab asignado al jugador {PhotonNetwork.LocalPlayer.ActorNumber}: {myPrefab}");

    }

    void Update()
    {
    
    }

    public void RequestNewObject(string prefabName, Vector3 position)
    {
        //if (!photonView.IsMine) return;

        Debug.Log($"📨 {PhotonNetwork.NickName} ejecutando RequestNewObject() para {prefabName}"); // Depuración

        if (string.IsNullOrEmpty(prefabName)) return;

        int myPlayerID = PhotonNetwork.LocalPlayer.ActorNumber; // Asegurar que el id del jugador es el correcto

        if ((myPlayerID == 1 && prefabName != "ApplePrefab(Clone)") || 
            (myPlayerID == 2 && prefabName != "BananaPrefab(Clone)"))
        {
            Debug.LogError($"🚫 {PhotonNetwork.NickName} NO tiene permiso para instanciar {prefabName}.");
            return;
        }

        if(prefabName == "ApplePrefab(Clone)"){
            prefabName = "ApplePrefab";
        }
        else if(prefabName == "BananaPrefab(Clone)"){
            prefabName = "BananaPrefab";
        }
        else{
            Debug.LogError("No se encontro el prefab");
        }

        //ObjectSpawner.instance.SpawnNewObject(prefabName, position, myPlayerID);
        ObjectSpawner.instance.photonView.RPC("SpawnNewObject", RpcTarget.MasterClient, prefabName, position, myPlayerID);

    }
}


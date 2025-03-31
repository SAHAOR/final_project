using UnityEngine;
using Photon.Pun;
using System.Collections;

public class InteractableObject : MonoBehaviourPun
{
    public int owner; // ID del jugador dueño del objeto
    private PhotonView photonView;
    private bool canGenerate = true; // Permite controlar si puede generar un nuevo objeto
    public bool isIndestructible = false;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }


    void Start()
    {
        Debug.Log($"🚀 {gameObject.name} ha sido instanciado correctamente.");
        StartCoroutine(AutoDestroyAfterTime(10f)); // Iniciar el temporizador de destrucción

        if (photonView.Owner != null) // Si ya tiene un dueño, no hacer nada
        return;

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
        if (PhotonNetwork.LocalPlayer.ActorNumber == owner)
        {
            photonView.RPC("AddScore", RpcTarget.AllBuffered, owner);
            RequestInstance();
        }
        else
        {
            photonView.RPC("SubtractScore", RpcTarget.AllBuffered, owner);
            RequestDestroy();
        }
    }


    void RequestInstance ()
    {
        if (!canGenerate) return;

        Debug.Log($"🖱️ {PhotonNetwork.NickName} hizo clic en {gameObject.name}");
        string prefabName;

        if (gameObject.name.Contains("Apple")) { // Obtener el prefab basado en el nombre del objeto
            prefabName = "ApplePrefab(Clone)";
        }
        else if (gameObject.name.Contains("Banana")) {
            prefabName = "BananaPrefab(Clone)";
        }
        else {
            Debug.LogError($"❌ No se pudo determinar el prefab de {gameObject.name}");
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f; // Generar la nueva posición un poco arriba del objeto actual

        StartCoroutine(DisableGenerationTemporarily()); // Deshabilitar generación en el padre antes de instanciar

        PlayerController.instance.RequestNewObject(prefabName, spawnPosition);
    }


    public IEnumerator DisableGenerationTemporarily()
    {
        yield return new WaitUntil(() => photonView != null); // Esperar hasta que photonView esté listo

        if (photonView == null || !photonView.IsMine) yield break; //Si PhotonView es destruido interrumpe ejecucion

        photonView.RPC("SetCanSpawn", RpcTarget.AllBuffered, false); // Desactivar generación en todos los clientes

        yield return new WaitForSeconds(5);

        if (photonView == null || !photonView.IsMine) yield break;

        photonView.RPC("SetCanSpawn", RpcTarget.AllBuffered, true); // Reactivar generación en todos los clientes
    }


    [PunRPC]
    void SetCanSpawn(bool state) //Actualizar cambio de estado en la red
    {
        canGenerate = state;
    }


    void RequestDestroy ()
    {
        if (isIndestructible) 
        {
            Debug.Log($"🚫 No puedes eliminar {gameObject.name} porque es indestructible.");
            return; 
        }

        Debug.Log($"🗑️ {PhotonNetwork.NickName} quiere destruir {gameObject.name}");

        ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID); 
    }

    [PunRPC]
    public void SetIndestructible(bool value)
    {
        isIndestructible = value;
    }
    
    private IEnumerator AutoDestroyAfterTime(float time)
    {
        Debug.Log($"⏳ {gameObject.name} comenzará a contar {time} segundos para autodestruirse.");
        yield return new WaitForSeconds(time);

        RequestDestroy();
    }

    [PunRPC]
    void AddScore(int playerID)
    {
        if (!canGenerate) return;
        GameManager.instance.AddScore(playerID);
    }


    [PunRPC]
    void SubtractScore(int playerID)
    {
        if (isIndestructible) return;
        GameManager.instance.SubtractScore(playerID);
    }

}
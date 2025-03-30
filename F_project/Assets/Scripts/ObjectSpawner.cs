using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ObjectSpawner : MonoBehaviourPun
{
    public static ObjectSpawner instance;
    public Transform SpawnPoint;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Solo el MasterClient debe instanciar

        SpawnInitialObjects();
    }

    void SpawnInitialObjects()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameObject apple = PhotonNetwork.Instantiate("ApplePrefab", SpawnPoint.position, Quaternion.identity);
        GameObject banana = PhotonNetwork.Instantiate("BananaPrefab", SpawnPoint.position, Quaternion.identity);

        apple.GetComponent<PhotonView>().RPC("SetIndestructible", RpcTarget.AllBuffered, true);
        banana.GetComponent<PhotonView>().RPC("SetIndestructible", RpcTarget.AllBuffered, true);


        apple.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, 1);
        banana.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, 2);
    }

    [PunRPC]
    public void SpawnNewObject(string prefabName, Vector3 position, int ownerID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"🛠 {PhotonNetwork.NickName} está instanciando {prefabName} con ID {ownerID}");

        if (string.IsNullOrEmpty(prefabName)) return;

        GameObject obj = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);

        StartCoroutine(SetOwnerDelayed(obj, ownerID)); // Esperar un pequeño tiempo para asegurar que el objeto se inicializa antes de llamar al RPC

        if (obj != null)
        { 
            InteractableObject interactable = obj.GetComponent<InteractableObject>();
            if(interactable != null)
            {
                StartCoroutine(interactable.DisableGenerationTemporarily());
            }
            else
            {
                Debug.Log("No se encontro interactable");
            }
        }
    }

    private IEnumerator SetOwnerDelayed(GameObject obj, int ownerID)
    {
        yield return new WaitForSeconds(0.1f); // Esperar un pequeño tiempo antes de asignar el dueño

        InteractableObject interactable = obj.GetComponent<InteractableObject>();

        if (interactable != null)
        {
            interactable.SetOwnerRPC(ownerID);
        }
        else
        {
            Debug.LogError($"❌ Error: {obj.name} no tiene el script InteractableObject adjunto.");
        }
    }
    

    [PunRPC]
    void DestroyObject(int viewID)
    {
        PhotonView obj = PhotonView.Find(viewID);

        if (obj != null)
        {
            PhotonNetwork.Destroy(obj.gameObject);
            Debug.Log($"🗑️ Objeto {obj.gameObject.name} destruido por petición de otro jugador.");
        }
    }
}

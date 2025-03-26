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

        // Asignar dueño
        apple.GetComponent<PhotonView>().TransferOwnership(1);
        banana.GetComponent<PhotonView>().TransferOwnership(2);
    }

    [PunRPC]
    public void SpawnNewObject(string prefabName, Vector3 position, int ownerID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"🛠 {PhotonNetwork.NickName} está instanciando {prefabName} con ID {ownerID}");

        if (string.IsNullOrEmpty(prefabName)) return;

        GameObject obj = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);

        StartCoroutine(SetOwnerDelayed(obj, ownerID)); // Esperar un pequeño tiempo para asegurar que el objeto se inicializa antes de llamar al RPC
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
}

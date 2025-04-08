using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ObjectSpawner : MonoBehaviourPun
{
    public static ObjectSpawner instance;
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;
    public GameObject freezePanel; // Panel que bloquea la interacción
    public GameObject freezeIcon; // Panel que bloquea la interacción

    [SerializeField]
    public Transform[] spawnPoints;  // Array de posiciones predefinidas

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Solo el MasterClient debe instanciar

        SpawnInitialObjects();
        StartCoroutine(SpawnPowerUpRoutine());
    }
    

    void SpawnInitialObjects()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject apple = PhotonNetwork.Instantiate("ApplePrefab", SpawnPoint1.position, Quaternion.identity);
        GameObject banana = PhotonNetwork.Instantiate("BananaPrefab", SpawnPoint2.position, Quaternion.identity);

        apple.GetComponent<PhotonView>().RPC("SetIndestructible", RpcTarget.AllBuffered, true);
        banana.GetComponent<PhotonView>().RPC("SetIndestructible", RpcTarget.AllBuffered, true);


        apple.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, 1);
        banana.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, 2);
    }

    //////////////////////////////////////// POWER UP
    [PunRPC]
    public void FreezePlayer(int actorNumber, int actorNumber2)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // Enviar una RPC al jugador específico para activar el congelamiento
        photonView.RPC("ActivateFreeze", PhotonNetwork.CurrentRoom.GetPlayer(actorNumber));
        photonView.RPC("ActivateFreezeIcon", PhotonNetwork.CurrentRoom.GetPlayer(actorNumber2));
    }

    [PunRPC]
    private void ActivateFreeze()
    {
        StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        freezePanel.SetActive(true);
        AudioManager.instance.PlaySfx("Freeze sound"); // Reproducir SFX al hacer clic
        AudioManager.instance.PauseMusic(); // Reproducir SFX al hacer clic
        yield return new WaitForSeconds(10f);
        AudioManager.instance.ResumeMusic(); // Reproducir SFX al hacer clic
        freezePanel.SetActive(false);
    }

    [PunRPC]
    private void ActivateFreezeIcon()
    {
        StartCoroutine(FreezeIcon());
    }

    private IEnumerator FreezeIcon()
    {
        freezeIcon.SetActive(true);
        yield return new WaitForSeconds(10f);
        freezeIcon.SetActive(false);
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnPowerUp();
            }
        }
    }

    private void SpawnPowerUp()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No hay posiciones de aparición definidas.");
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        // Selecciona una posición aleatoria del array de spawnPoints
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Instancia el Power-Up en la posición seleccionada
        GameObject powerup = PhotonNetwork.Instantiate("PowerUpFreeze", spawnPoint.position, Quaternion.identity);
    }

    ////////////////////////////////////////////////// END POWER UP

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

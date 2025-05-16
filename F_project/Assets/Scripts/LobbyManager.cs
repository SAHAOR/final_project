using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Referencias a los elementos de la interfaz de usuario
    public TMP_Text statusText; // Texto para mostrar el estado del matchmaking
    public TMP_Text playersText; // Texto para mostrar el número de jugadores en la sala
    public Button matchmakingButton; // Botón para iniciar el matchmaking

    // Variables para controlar el matchmaking
    private float matchmakingTimeout = 10f; // Tiempo máximo de espera en segundos
    private bool isSearching = false; // Indica si se está buscando una partida
    private float searchStartTime; // Hora de inicio de la búsqueda

    void Start()
    {
        // Conectar al servidor de Photon al iniciar
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true; // Sincronizar automáticamente las escenas entre los jugadores

        // Configurar el estado inicial del botón y el texto de estado
        matchmakingButton.interactable = true; // Habilitar el botón al inicio
        statusText.text = "Conectando..."; // Mostrar mensaje de conexión
    }

    // Callback que se ejecuta cuando el cliente se conecta al servidor de Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster llamado: Conectado al servidor.");
        statusText.text = "Conectado al servidor."; // Actualizar el texto de estado

        if (matchmakingButton == null)
        {
            Debug.LogError("El botón de matchmaking no está asignado en el inspector.");
        }
        else
        {
            matchmakingButton.interactable = true; // Habilitar el botón de matchmaking
            Debug.Log("Botón de matchmaking habilitado.");
        }
    }

    // Método para iniciar el matchmaking al hacer clic en el botón
    public void StartMatchmaking()
    {
        // Verificar si el cliente está conectado al servidor
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "No estás conectado al servidor.";
            Debug.LogWarning("No estás conectado al servidor.");
            return;
        }

        // Actualizar el estado de la interfaz de usuario
        statusText.text = "Buscando partida...";
        matchmakingButton.interactable = false; // Deshabilitar el botón mientras busca
        isSearching = true; // Indicar que se está buscando una partida
        searchStartTime = Time.time; // Registrar el tiempo de inicio de la búsqueda

        // Intentar unirse a una sala existente o crear una nueva si no hay salas disponibles
        PhotonNetwork.JoinRandomOrCreateRoom(
            null, // Sin filtros específicos
            2,    // Máximo de jugadores por sala
            MatchmakingMode.FillRoom, // Llenar salas existentes antes de crear nuevas
            null, // Sin restricciones de tipo de sala
            null, // Sin propiedades personalizadas
            null, // Sin propiedades personalizadas
            new RoomOptions { MaxPlayers = 2 } // Opciones para crear una nueva sala
        );
    }

    void Update()
    {
        // Verificar si se está buscando una partida
        if (isSearching)
        {
            // Si el tiempo de búsqueda excede el límite, detener la búsqueda
            if (Time.time - searchStartTime > matchmakingTimeout)
            {
                isSearching = false; // Detener la búsqueda
                statusText.text = "No se encontró un jugador. Inténtalo de nuevo."; // Actualizar el texto de estado
                matchmakingButton.interactable = true; // Rehabilitar el botón
                PhotonNetwork.LeaveRoom(); // Salir de la sala si se creó una
            }
        }
    }

    // Callback que se ejecuta cuando el cliente se une a una sala
    public override void OnJoinedRoom()
    {
        statusText.text = "Unido a la sala: " + PhotonNetwork.CurrentRoom.Name; // Mostrar el nombre de la sala
        UpdatePlayerCount(); // Actualizar el número de jugadores en la sala
        isSearching = false; // Detener el temporizador de búsqueda
    }

    // Callback que se ejecuta cuando un nuevo jugador entra en la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount(); // Actualizar el número de jugadores en la sala

        // Si hay dos jugadores en la sala y el cliente actual es el host, iniciar la partida
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    // Callback que se ejecuta cuando un jugador abandona la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount(); // Actualizar el número de jugadores en la sala
    }

    // Actualizar el texto que muestra el número de jugadores en la sala
    void UpdatePlayerCount()
    {
        playersText.text = "Jugadores en la sala: " + PhotonNetwork.CurrentRoom.PlayerCount + "/2";
    }

    // Método para iniciar la partida
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) // Solo el host puede iniciar la partida
        {
            PhotonNetwork.LoadLevel("GameScene"); // Cargar la escena del juego para todos los jugadores
        }
    }

    // Callback que se ejecuta si no se encuentra una sala al intentar unirse
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = "No se encontró una sala. Creando una nueva..."; // Actualizar el texto de estado
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); // Crear una nueva sala
    }
}
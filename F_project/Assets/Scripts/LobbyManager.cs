using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInput;
    public TMP_Text roomCodeText;
    public TMP_Text playersText;
    public Button playButton;
    public Button createRoomButton;
    public Button joinRoomButton;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conectar a Photon al iniciar
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.interactable = false; // Deshabilitar al inicio

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); // Unirse al lobby cuando se conecte
    }

    public void CreateRoom()
    {
        string roomName = Random.Range(1000, 9999).ToString(); // Código aleatorio
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
        createRoomButton.interactable = false; // Deshabilitar el botón de crear sala
        joinRoomButton.interactable = false; // Deshabilitar el botón de unirse a sala
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomInput.text);
        joinRoomButton.interactable = false; // Deshabilitar el botón de unirse a sala
        createRoomButton.interactable = false; // Deshabilitar el botón de crear sala
    }

    public override void OnJoinedRoom()
    {
        roomCodeText.text = PhotonNetwork.CurrentRoom.Name;
        CheckPlayersInRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckPlayersInRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckPlayersInRoom();
    }

    void CheckPlayersInRoom()
    {
        playersText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/2";
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            playButton.interactable = true; // Solo el dueño de la sala puede presionar
        }
        else
        {
            playButton.interactable = false;
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) // Solo el host inicia la partida
        {
            PhotonNetwork.LoadLevel("GameScene"); // Carga la escena para todos los jugadores
        }
    }
}
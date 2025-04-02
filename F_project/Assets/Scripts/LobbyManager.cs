
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

    private bool isAttemptingToJoinOrCreate = false;
    private string pendingRoomName = null;
    private bool isCreatingRoom = false;

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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                // Si ya está en una sala, salir primero
                isAttemptingToJoinOrCreate = true;
                isCreatingRoom = true;
                pendingRoomName = Random.Range(1000, 9999).ToString();
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // Si no está en una sala, crear directamente
                string roomName = Random.Range(1000, 9999).ToString();
                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
            }
        }
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                // Si ya está en una sala, salir primero
                isAttemptingToJoinOrCreate = true;
                isCreatingRoom = false;
                pendingRoomName = roomInput.text;
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // Si no está en una sala, unirse directamente
                PhotonNetwork.JoinRoom(roomInput.text);
            }
        }
    }

    public override void OnLeftRoom()
    {
        if (isAttemptingToJoinOrCreate)
        {
            isAttemptingToJoinOrCreate = false;
            if (isCreatingRoom)
            {
                PhotonNetwork.CreateRoom(pendingRoomName, new RoomOptions { MaxPlayers = 2 });
            }
            else
            {
                PhotonNetwork.JoinRoom(pendingRoomName);
            }
            pendingRoomName = null;
        }
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
        playButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient;
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) // Solo el host inicia la partida
        {
            PhotonNetwork.LoadLevel("GameScene"); // Carga la escena para todos los jugadores
        }
    }
}
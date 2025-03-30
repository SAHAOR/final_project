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
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomInput.text);
    }

    public override void OnJoinedRoom()
    {
        roomCodeText.text = "Código: " + PhotonNetwork.CurrentRoom.Name;
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

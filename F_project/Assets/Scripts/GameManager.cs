using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    private int player1ID;
    private int player2ID;
    private int myPlayerID; // ID local del jugador en esta sesión

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;

    public TextMeshProUGUI scoreTextPlayer1;
    public TextMeshProUGUI scoreTextPlayer2;

    public int winningScore = 100;
    public Transform spawnPoint;   

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1ID = PhotonNetwork.LocalPlayer.ActorNumber;
                Debug.Log($"🎯 Me asigno como Player 1 - ID: {player1ID}");

                // Enviar a los demás jugadores quién es Player 1
                photonView.RPC("SetPlayer1ID", RpcTarget.OthersBuffered, player1ID);
            }
            else
            {
                // Pedir al MasterClient que me asigne como Player 2
                photonView.RPC("RequestPlayer2ID", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }

            myPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.Log($"📌 Mi Player ID es {myPlayerID}");

            PhotonNetwork.Instantiate("Player", spawnPoint.position, Quaternion.identity);
        }

        Debug.Log($"👥 Número total de jugadores en la sala: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }


    [PunRPC]
    void SetPlayer1ID(int p1ID)
    {
        player1ID = p1ID;
        Debug.Log($"✅ Player 1 ID Sincronizado: {player1ID}");
    }

    [PunRPC]
    void SetPlayer2ID(int p2ID)
    {
        player2ID = p2ID;
        Debug.Log($"✅ Player 2 ID Sincronizado: {player2ID}");
    }

    [PunRPC]
    void RequestPlayer2ID(int newPlayer2ID)
    {
        if (player2ID == 0) // Si aún no hay Player 2 asignado
        {
            player2ID = newPlayer2ID;
            Debug.Log($"🎯 Se ha asignado Player 2 - ID: {player2ID}");

            // Ahora enviamos esta información a todos los jugadores
            photonView.RPC("SetPlayer2ID", RpcTarget.AllBuffered, player2ID);
        }
    }

    [PunRPC]
    public void AddScore(int playerID)
    {
        Debug.Log($"✅ SUMANDO PUNTO A JUGADOR {playerID}");

        if (playerID == player1ID) 
        {
            scorePlayer1++;
            Debug.Log($"🎯 Nuevo Score P1: {scorePlayer1}");
        } 
        else if (playerID == player2ID) 
        {
            scorePlayer2++;
            Debug.Log($"🎯 Nuevo Score P2: {scorePlayer2}");
        }

        photonView.RPC("UpdateScores", RpcTarget.AllBuffered, scorePlayer1, scorePlayer2);
    }

    [PunRPC]
    public void SubtractScore(int playerID)
    {
        Debug.Log($"⛔ RESTANDO PUNTO A JUGADOR {playerID}");

        if (playerID == player1ID) 
        {
            scorePlayer1 = Mathf.Max(0, scorePlayer1 - 1);
            Debug.Log($"🎯 Nuevo Score P1: {scorePlayer1}");
        } 
        else if (playerID == player2ID) 
        {
            scorePlayer2 = Mathf.Max(0, scorePlayer2 - 1);
            Debug.Log($"🎯 Nuevo Score P2: {scorePlayer2}");
        }

        photonView.RPC("UpdateScores", RpcTarget.AllBuffered, scorePlayer1, scorePlayer2);
    }

    [PunRPC]
    void UpdateScores(int score1, int score2)
    {
        scorePlayer1 = score1;
        scorePlayer2 = score2;

        scoreTextPlayer1.text = $"Jugador 1: {scorePlayer1}";
        scoreTextPlayer2.text = $"Jugador 2: {scorePlayer2}";
    }
}

using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    private int player1ID;
    private int player2ID;
    private int myPlayerID; // ID local del jugador en esta sesión

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;

    private float startTime; // Guarda el tiempo de inicio

    public TextMeshProUGUI scoreTextPlayer1;
    public TextMeshProUGUI scoreTextPlayer2;
    public TextMeshProUGUI TimeMatch;

    public int winningScore = 100;
    public Transform spawnPoint;

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject background;
    public GameObject gamePanel;

    public TextMeshProUGUI victoryScoreText; // Texto en pantalla de victoria
    public TextMeshProUGUI defeatScoreText;  // Texto en pantalla de derrota
    public TextMeshProUGUI winTimeMatch;  // Texto en pantalla de derrota
    public TextMeshProUGUI loseTimeMatch;  // Texto en pantalla de derrota


    public Button loserButton;  // Botón del perdedor para pedir nueva partida
    public Button winnerButton; // Botón del ganador (inicialmente deshabilitado)

    private bool isWinner = false;
    private bool isLoser = false;

    public Image winnerImage;
    public Image loserImage;
    public Sprite player1Sprite;
    public Sprite player2Sprite;
    private Dictionary<int, Sprite> playerSprites = new Dictionary<int, Sprite>();

    private float elapsedTime = 0f;
    private bool isCounting = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartTimerAfterDelay(4.2f));
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1ID = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("SetPlayer1Sprite", RpcTarget.AllBuffered, player1ID);
                Debug.Log($"🎯 Me asigno como Player 1 - ID: {player1ID}");

                // Enviar a los demás jugadores quién es Player 1
                //photonView.RPC("SetPlayer1ID", RpcTarget.OthersBuffered, player1ID);
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

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        background.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1;

        startTime = Time.time;

        // loserButton.gameObject.SetActive(false);
        // winnerButton.gameObject.SetActive(false);
        // winnerButton.interactable = false;
    }

    void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            UpdateTime(elapsedTime);
        }
    }

    IEnumerator StartTimerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCounting = true;
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

        if (scorePlayer1 >= 100)
        {
            photonView.RPC("SetWinner", RpcTarget.All, player1ID, scorePlayer1);
            photonView.RPC("SetLoser", RpcTarget.All, player2ID, scorePlayer2);
        }
        else if (scorePlayer2 >= 100)
        {
            photonView.RPC("SetWinner", RpcTarget.All, player2ID, scorePlayer2);
            photonView.RPC("SetLoser", RpcTarget.All, player1ID, scorePlayer1);
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

        scoreTextPlayer1.text = $"{scorePlayer1}";
        scoreTextPlayer2.text = $"{scorePlayer2}";
    }

    [PunRPC]
    void SetWinner(int winnerID, int winnerScore)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerID)
        {
            isWinner = true;

            float elapsedTime = Time.time - startTime;
            string formattedTime = FormatTime(elapsedTime);

            // Mostrar la imagen del ganador
            winnerImage.sprite = (winnerID == player1ID) ? player1Sprite : player2Sprite;

            // Configurar la pantalla de victoria
            gamePanel.SetActive(false);
            background.SetActive(true);
            winPanel.SetActive(true);

            victoryScoreText.text = $"Puntaje Final: {winnerScore}";
            winTimeMatch.text = $"Tiempo de partida: {formattedTime}";

            Debug.Log($"🎉 El puntaje del ganador es: {victoryScoreText.text}");
            Debug.Log($"⏳ El tiempo del ganador es: {winTimeMatch.text}");
        }
        else
        {
            // Si no es el ganador, desactivamos la pantalla de victoria
            winPanel.SetActive(false);
        }
    }

    [PunRPC]
    void SetLoser(int loserID, int loserScore)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == loserID)
        {
            isLoser = true;

            float elapsedTime = Time.time - startTime;
            string formattedTime = FormatTime(elapsedTime);

            // Mostrar la imagen del perdedor
            loserImage.sprite = (loserID == player1ID) ? player1Sprite : player2Sprite;

            // Configurar la pantalla de derrota
            gamePanel.SetActive(false);
            background.SetActive(true);
            losePanel.SetActive(true);

            defeatScoreText.text = $"Puntaje Final: {loserScore}";
            loseTimeMatch.text = $"Tiempo de partida: {formattedTime}";

            Debug.Log($"😢 El puntaje del perdedor es: {defeatScoreText.text}");
            Debug.Log($"⏳ El tiempo del perdedor es: {loseTimeMatch.text}");
        }
        else
        {
            // Si no es el perdedor, desactivamos la pantalla de derrota
            losePanel.SetActive(false);
        }
    }

    void FreezeGame()
    {
        Time.timeScale = 0;  // Detener el tiempo del juego        
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateTime(float time)// Muestra el tiempo en el Game Scene
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimeMatch.text = $"{minutes:00}:{seconds:00}";
    }



    // // 🟢 EL PERDEDOR SOLICITA UNA NUEVA PARTIDA
    // public void RequestNewGame()
    // {
    //     if (isLoser)
    //     {
    //         Debug.Log("📢 El perdedor solicitó nueva partida.");
    //         loserButton.interactable = false;
    //         photonView.RPC("EnableWinnerButton", RpcTarget.Others);
    //     }
    // }

    // // 🔓 HABILITAR BOTÓN DEL GANADOR
    // [PunRPC]
    // void EnableWinnerButton()
    // {
    //     Debug.Log("✅ Botón del ganador habilitado.");
    //     winnerButton.interactable = true;
    // }

    // // 🔄 REINICIAR EL JUEGO CUANDO EL GANADOR CONFIRMA
    // public void RestartGame()
    // {
    //     if (isWinner && winnerButton.interactable)
    //     {
    //         Debug.Log("🔄 Reiniciando la partida...");

    //         Time.timeScale = 1;
    //         StartNewGame();

    //         photonView.RPC("ResetGameForAll", RpcTarget.AllBuffered);
    //     }
    // }

    // [PunRPC]
    // void ResetGameForAll()
    // {
    //     StartNewGame();
    //     photonView.RPC("UpdateScores", RpcTarget.AllBuffered, scorePlayer1, scorePlayer2);
    // }

    // void StartNewGame()
    // {
    //     scorePlayer1 = 0;
    //     scorePlayer2 = 0;
    //     startTime = Time.time;

    //     winPanel.SetActive(false);
    //     losePanel.SetActive(false);
    //     background.SetActive(false);

    //     loserButton.gameObject.SetActive(false);
    //     winnerButton.gameObject.SetActive(false);
    //     winnerButton.interactable = false;

    //     isWinner = false;
    //     isLoser = false;
    // }

    public void ExitToMenu()
    {
        UIManager.instance.CargarEscenaJuego("Felipe");
        PhotonNetwork.LeaveRoom(); // Sale de la sala
                                   //PhotonNetwork.LoadLevel("Felipe"); // Carga la escena del menú
    }



    [PunRPC]
    void SetPlayer1Sprite(int playerID)
    {
        player1ID = playerID;
        playerSprites[player1ID] = player1Sprite;
    }

    [PunRPC]
    void SetPlayer2Sprite(int playerID)
    {
        player2ID = playerID;
        playerSprites[player2ID] = player2Sprite;
    }


}

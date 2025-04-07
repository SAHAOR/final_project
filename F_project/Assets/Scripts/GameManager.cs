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

 
    private Dictionary<int, Sprite> playerSprites = new Dictionary<int, Sprite>();

    private float elapsedTime = 0f;
    private bool isCounting = false;

    public Button requestRematchButton; // Botón en la pantalla de derrota
    public Button acceptRematchButton;  // Botón en la pantalla de victoria

    private bool isRematchRequested = false; // Indica si el perdedor solicitó la revancha

    public Camera winnerCamera; // Cámara secundaria para el ganador
    public Camera loserCamera;  // Cámara secundaria para el perdedor
    public GameObject player1Prefab; // Prefab del jugador 1
    public GameObject player2Prefab; // Prefab del jugador 2

    public GameObject winnerCrownPrefab; // Prefab de la corona del ganador
    public GameObject loserCrownPrefab;  // Prefab de la corona del perdedor

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

        if (scorePlayer1 >= 4)
        {
            photonView.RPC("SetWinner", RpcTarget.All, player1ID, scorePlayer1);
            photonView.RPC("SetLoser", RpcTarget.All, player2ID, scorePlayer2);
        }
        else if (scorePlayer2 >= 4)
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


            // Mostrar el prefab del ganador en la cámara secundaria
            GameObject winnerPrefab = (winnerID == player1ID) ? player1Prefab : player2Prefab;
            Vector3 winnerCrownScale = new Vector3(30f, 30f, 30f); // Escala específica para la corona del ganador
            ShowPrefabInCamera(winnerPrefab, winnerCamera.transform, winnerCrownPrefab, winnerCrownScale);


            // Configurar la pantalla de victoria
            gamePanel.SetActive(false);
            background.SetActive(true);
            winPanel.SetActive(true);

            string scoreText = LocalizationManager.Instance.GetText(22); // Clave para "Puntaje"
            string timeText = LocalizationManager.Instance.GetText(23);  // Clave para "Tiempo"

            victoryScoreText.text = $"{scoreText}: {winnerScore}";
            winTimeMatch.text = $"{timeText}: {formattedTime}";

            Debug.Log($"🎉 El puntaje del ganador es: {victoryScoreText.text}");
            Debug.Log($"⏳ El tiempo del ganador es: {winTimeMatch.text}");

            // Deshabilitar el botón de aceptar revancha inicialmente
            acceptRematchButton.interactable = false;
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
            Debug.Log("✅ Este cliente es el perdedor.");
            isLoser = true;

            // Calcular el tiempo transcurrido
            float elapsedTime = Time.time - startTime;
            string formattedTime = FormatTime(elapsedTime);

            // Mostrar el prefab del perdedor en la cámara secundaria
            GameObject loserPrefab = (loserID == player1ID) ? player1Prefab : player2Prefab;
            Vector3 loserCrownScale = new Vector3(0.5f, 0.5f, 0.5f); // Escala específica para la corona del perdedor
            ShowPrefabInCamera(loserPrefab, loserCamera.transform, loserCrownPrefab, loserCrownScale);



            // Configurar la pantalla de derrota
            gamePanel.SetActive(false);
            background.SetActive(true);
            losePanel.SetActive(true);

            // Obtener los textos traducidos desde LocalizationManager
            string scoreText = LocalizationManager.Instance.GetText(52); // Clave para "Puntaje"
            string timeText = LocalizationManager.Instance.GetText(53);  // Clave para "Tiempo"

            // Actualizar los textos de puntaje y tiempo
            defeatScoreText.text = $"{scoreText}: {loserScore}";
            loseTimeMatch.text = $"{timeText}: {formattedTime}";

            Debug.Log($"😢 El puntaje del perdedor es: {defeatScoreText.text}");
            Debug.Log($"⏳ El tiempo del perdedor es: {loseTimeMatch.text}");

            // Habilitar el botón de solicitar revancha
            requestRematchButton.interactable = true;
        }
        else
        {
            Debug.Log("❌ Este cliente NO es el perdedor.");
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
       
    }

    [PunRPC]
    void SetPlayer2Sprite(int playerID)
    {
        player2ID = playerID;
       
    }

    public void RequestRematch()
    {
        Debug.Log("🔄 El perdedor ha solicitado una revancha.");
        photonView.RPC("NotifyRematchRequest", RpcTarget.All);
    }

    public void AcceptRematch()
    {
        Debug.Log("✅ El ganador ha aceptado la revancha.");
        photonView.RPC("StartRematch", RpcTarget.All);
    }

    [PunRPC]
    void NotifyRematchRequest()
    {
        Debug.Log("🔔 Notificación de solicitud de revancha recibida.");
        isRematchRequested = true;

        // Habilitar el botón de aceptar revancha en la pantalla del ganador
        if (isWinner)
        {
            acceptRematchButton.interactable = true;
        }
    }

    [PunRPC]
    void StartRematch()
    {
        Debug.Log("🎮 Iniciando la revancha...");
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name); // Recarga la escena actual
    }

    void ShowPrefabInCamera(GameObject prefab, Transform cameraTransform, GameObject crownPrefab = null, Vector3? crownScale = null)
    {
        // Limpiar cualquier objeto previo en la cámara secundaria
        foreach (Transform child in cameraTransform)
        {
            Destroy(child.gameObject);
        }

        // Instanciar el prefab frente a la cámara secundaria
        GameObject instance = Instantiate(prefab, cameraTransform);


        // Ajustar la posición y rotación del prefab
        instance.transform.localPosition = new Vector3(0, 0, 29.9f); // Coloca el prefab a 5 unidades frente a la cámara
        instance.transform.localRotation = Quaternion.Euler(0, 180, 0); // Ajusta la rotación si es necesario
        instance.transform.localScale = new Vector3(20f, 20f, 20f); // Escala predeterminada de 4, 4, 4

        // Si se proporciona un prefab de corona, instanciarlo como hijo del jugador
        if (crownPrefab != null)
        {
            GameObject crownInstance = Instantiate(crownPrefab, instance.transform);
            crownInstance.transform.localPosition = new Vector3(-0.07f, 0.221f, 0); // Ajusta la posición de la corona sobre la cabeza del jugador
            crownInstance.transform.localRotation = Quaternion.Euler(167f, 118f, 9.08f);

            // Si la corona es la de derrota, girarla 180 grados
            if (crownPrefab == loserCrownPrefab)
            {
                crownInstance.transform.localRotation = Quaternion.Euler(13.073f,-120.01f,-7.443f); // Girar 180 grados
            }

            // Ajustar la escala de la corona (usar el valor proporcionado o un valor por defecto)
            crownInstance.transform.localScale = crownScale ?? new Vector3(0.5f, 0.5f, 0.5f); // Si no se proporciona escala, usar (1, 1, 1)

        }



    }


}

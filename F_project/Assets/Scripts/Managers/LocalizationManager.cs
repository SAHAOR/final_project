using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    public Dictionary<int, string> languageFile = new Dictionary<int, string>();
    public string currentLanguage = "English";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage(PlayerPrefs.GetString("Language", "English"));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLanguage(string language)
    {
        currentLanguage = language;
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();

        // Cargar el diccionario con los textos en el idioma seleccionado
        languageFile = LoadLanguageFile(language);

        // Notificar a todos los textos que deben actualizarse
        LocalizedText.UpdateAllTexts();
    }

    private Dictionary<int, string> LoadLanguageFile(string language)
    {
        Dictionary<int, string> tempDictionary = new Dictionary<int, string>();

        bool isEnglish = language == "English";

        tempDictionary.Add(1, isEnglish ? "Touch the screen to play" : "Toca la pantalla para jugar.");
        tempDictionary.Add(2, isEnglish ? "Options" : "Opciones");
        tempDictionary.Add(3, isEnglish ? "Exit" : "Salir");
        tempDictionary.Add(4, isEnglish ? "Settings" : "Opciones");
        tempDictionary.Add(5, isEnglish ? "Resolution" : "Resolución");
        tempDictionary.Add(6, isEnglish ? "Master" : "General");
        tempDictionary.Add(7, isEnglish ? "BGM" : "BGM");
        tempDictionary.Add(8, isEnglish ? "SFX" : "SFX");
        tempDictionary.Add(9, isEnglish ? "Mute" : "Silenciar");
        tempDictionary.Add(10, isEnglish ? "Language" : "Lenguaje");
        tempDictionary.Add(11, isEnglish ? "Back" : "Atrás");
        tempDictionary.Add(12, isEnglish ? "Home" : "Inicio");
        tempDictionary.Add(13, isEnglish ? "FullScreen" : "Pantalla Completa");
        tempDictionary.Add(14, isEnglish ? "Create / Join private room" : "Crear / Unirse a una sala privada");
        tempDictionary.Add(15, isEnglish ? "How to play?" : "¿Cómo jugar?");
        tempDictionary.Add(16, isEnglish ? "Credits" : "Créditos");
        tempDictionary.Add(17, isEnglish ? "Back" : "Volver");
        tempDictionary.Add(18, isEnglish ? "Credits" : "Créditos");
        tempDictionary.Add(19, isEnglish ? "Brightness" : "Brillo");
        tempDictionary.Add(24, isEnglish ? "¡Go!" : "¡inicia!");
        tempDictionary.Add(25, isEnglish ? "Sound" : "Sonido");
        //How to play    
        tempDictionary.Add(26, isEnglish ? "-One player must create a room.\n-Must generate an access code.\n-The second player must enter the code to join.\n-Must press the play button.\n-Wait for the countdown to start." : "-Un jugador debe crear una sala.\n-Debe generar un código de acceso.\n-El segundo jugador debe ingresar el \ncódigo para unirse.\n-Debe presionar el botón de jugar.\n-Esperar a que comience la cuenta regresiva.");
        tempDictionary.Add(27, isEnglish ? "-Each player has a unique fruit: banana for one player, apple for the other. \n(Your fruit appears twinkling on top)\n\n -Click on your fruit to:\n    -Multiply it.\n    -Earn points (+1)." : "-Cada jugador tiene una fruta única: plátano para un jugador, manzana para el otro. \n (Tu fruta aparece parpadeando en la parte superior)\n\n -Haz clic en tu fruta para:\n    -Multiplícala.\n    -Gana puntos (+1).");
        tempDictionary.Add(28, isEnglish ? "If you click on your opponent's fruit:\n    -You take away 1 point.\n    -That fruit disappears." : "Si haces clic en la fruta de tu oponente:\n-Le quitas 1 punto.\n-Esa fruta desaparece.");
        tempDictionary.Add(29, isEnglish ? "A snowflake appears randomly.\n  When clicked:\n  -Disables the opponent for a few seconds.\n  -Particle effects are displayed." : "Aparece un copo de nieve aleatoriamente.\n Al hacer clic:\n -Desactiva al oponente durante unos segundos.\n -Se muestran los efectos de partículas.");
        tempDictionary.Add(30, isEnglish ? "The game ends when:\n  -One player reaches 100 points.\n  -The first to reach this \npoint wins the game!" : "El juego termina cuando:\nUn jugador alcanza los 100 puntos.\n¡El primero en llegar a este \npunto gana el juego!");

        //Credits
        tempDictionary.Add(31, isEnglish ? "Samir Hassan Ordoñez (Project Manager, game designer, network architect, gameplay programmer, VFX, devops)" : "Samir Hassan Ordoñez (Gestión de proyecto, diseño de juego, arquitecto de red, gameplay, VFX, devops)");
        tempDictionary.Add(32, isEnglish ? "Miguel Ariza (Technical artist, 3D models, shaders and gameplay mechanics)" : "Miguel Ariza (Artista técnico, modelos 3D, shaders y mecánicas de jugabilidad)");
        tempDictionary.Add(33, isEnglish ? "Luis Carlos Villalobos (Sound/SFX engineer, UI and backend programmer)" : "Luis Carlos Villalobos (Ingeniero de sonido/SFX, desarrollador backend y UI)");
        tempDictionary.Add(34, isEnglish ? "Felipe Gualteros Viasus (UI programmer and designer, backend configuration, aesthetic design)" : "Felipe Gualteros  Viasus (Diseñador y desarrollador de UI, programador de configuraciones y diseño estético)");

        //Samir Scene
        tempDictionary.Add(35, isEnglish ? "Create Room" : "Crear Sala");
        tempDictionary.Add(36, isEnglish ? "Enter room code" : "Ingrese número de sala");
        tempDictionary.Add(37, isEnglish ? "Join Room" : "Unirse a la sala");
        tempDictionary.Add(38, isEnglish ? "Room code" : "Codigo de sala");
        tempDictionary.Add(39, isEnglish ? "Players in room:" : "Jugadores en sala");
        tempDictionary.Add(40, isEnglish ? "Play" : "Jugar");
        tempDictionary.Add(41, isEnglish ? "Or" : "O");
        tempDictionary.Add(64, isEnglish ? "Back" : "Regresar");

        //Game Scene Win Panel and Lose panel 
        tempDictionary.Add(20, isEnglish ? "You Win !!!!" : "Ganaste !!!!");
        tempDictionary.Add(21, isEnglish ? "You Lose !!!!" : "Perdiste !!!!");
        tempDictionary.Add(22, isEnglish ? "Score" : "Puntaje");
        tempDictionary.Add(23, isEnglish ? "Time" : "Tiempo");
        tempDictionary.Add(42, isEnglish ? "Accept Rematch " : "Aceptar Revancha ");
        tempDictionary.Add(43, isEnglish ? "Main Menu" : "Menu Principal");
        tempDictionary.Add(44, isEnglish ? "*If the other player requests another game, the accept button will be enabled." : "*Si el otro jugador solicita otra partida, el botón de aceptar se habilitará.");
        tempDictionary.Add(45, isEnglish ? "Recuest Rematch" : "Pedir Revancha");
        tempDictionary.Add(46, isEnglish ? "*If the other player requests another game, the accept button will be enabled." : "*Si el otro jugador solicita otra partida, el botón de aceptar se habilitará.");

       // canvas Game scene
        tempDictionary.Add(47, isEnglish ? "player 1 " : "Jugador 1 ");
        tempDictionary.Add(48, isEnglish ? "player 2 " : "Jugador 2 ");

        //Cargando
        tempDictionary.Add(49, isEnglish ? "Lo" : "Carg");
        tempDictionary.Add(50, isEnglish ? "ad" : "ando");
        /*Error
        tempDictionary.Add(51, isEnglish ? "The code you entered does not exist." : "El codigo que introdujiste no existe.");*/
        
        //Game Scene Lose panel     
        tempDictionary.Add(52, isEnglish ? "Score" : "Puntaje");
        tempDictionary.Add(53, isEnglish ? "Time" : "Tiempo");

        //Error
        tempDictionary.Add(54, isEnglish ? "The code you entered does not exist." : "El codigo que introdujiste no existe.");
        tempDictionary.Add(55, isEnglish ? "The room you are trying to access is full." : "La sala a la que intentas acceder está llena.");
        tempDictionary.Add(56, isEnglish ? "Could not connect to the server." : "No se pudo conectar con el servidor.");
        tempDictionary.Add(57, isEnglish ? "The host locked the room." : "El anfitrión cerró la habitación.");
        tempDictionary.Add(58, isEnglish ? "Invalid code.\nOnly numbers are allowed." : "Código inválido.\nSolo se permiten números.");

        //Titles How to play menu 
        tempDictionary.Add(59, isEnglish ? "How to get started?" : "¿Cómo empezar?");
        tempDictionary.Add(60, isEnglish ? "Your fruit is your point" : "Tu fruta es tu punto");
        tempDictionary.Add(61, isEnglish ? "Pay attention to the rival's fruit" : "Atento en la fruta del rival");
        tempDictionary.Add(62, isEnglish ? "Power Up: Freezing Snow!" : "¡Potencia: Nieve helada!");
        tempDictionary.Add(63, isEnglish ? "How to win?" : "¿Cómo ganar?");

        //WinAndLoseMenu
        tempDictionary.Add(65, isEnglish ? "Return to the main menu" : "Volver al menu inicial");

        //PowerUp
        tempDictionary.Add(66, isEnglish ? "Frozen!" : "¡Congelado!");
        tempDictionary.Add(67, isEnglish ? "Frozen Enemy" : "Enemigo Congelado");

        return tempDictionary;
    }

    public string GetText(int key)
    {
        return languageFile.ContainsKey(key) ? languageFile[key] : key.ToString();
    }
}

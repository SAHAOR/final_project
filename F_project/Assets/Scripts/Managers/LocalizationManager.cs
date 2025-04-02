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
        tempDictionary.Add(26, isEnglish ? "Welcome to Splashy!\n\nIt's a competitive multiplayer game where speed and strategy are key. The goal is to reach 100 points before your opponent by clicking on the correct fruit." : "¡Bienvenido a Splashy!\n\nEs un juego multijugador competitivo donde la rapidez y la estrategia son clave. El objetivo es llegar a 100 puntos antes que tu oponente, haciendo clic en la fruta correcta.");
        tempDictionary.Add(27, isEnglish ? "-One player must create a game, which will generate a room code.\n\n-The other player must enter the code to join the same game.\n\n-When both of you are ready, press the play button to begin!" : "-Un jugador debe crear una partida, lo que generará un código de sala.\n\n-El otro jugador debe ingresar el código para unirse a la misma partida.\n\n-Cuando ambos estén listos, ¡presione el botón de jugar para comenzar!");
        tempDictionary.Add(28, isEnglish ? "-Each player has their own assigned fruit.\n\n-Clicking on your fruit will multiply it and give you 1 point.\n\n-Tip: If you click on your opponent's fruit, it will disappear and they will lose that fruit." : "-Cada jugador tiene su propia fruta asignada.\n\n-Hacer clic en tu fruta la multiplicará y te dará 1 punto.\n\n-Concejo: Si haces clic en la fruta de tu rival, desaparecerá y el perderá esa fruta.");
        tempDictionary.Add(29, isEnglish ? "-Be quick to click your fruit and accumulate points quickly.\n\n-Use the tactic of eliminating your opponent's fruit to delay him.\n\n-Keep an eye on the distribution of fruits on the screen." : "-Sé rápido para clickear tu fruta y acumular puntos rápidamente.\n\n-Usa la táctica de eliminar la fruta de tu rival para retrasarlo.\n\n-Mantente atento a la distribución de las frutas en pantalla.");
        tempDictionary.Add(30, isEnglish ? "-The first player to reach 100 points will be the winner.\n\n-Practice your speed and accuracy to improve your performance." : "-El primer jugador en alcanzar 100 puntos será el ganador.\n\n-Practica tu velocidad y precisión para mejorar tu rendimiento.");
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
        tempDictionary.Add(49, isEnglish ? "Load" : "Carg");
        tempDictionary.Add(50, isEnglish ? "ing" : "ando");
        //Error
        tempDictionary.Add(51, isEnglish ? "The code you entered does not exist." : "El codigo que introdujiste no existe.");
        


        return tempDictionary;
    }

    public string GetText(int key)
    {
        return languageFile.ContainsKey(key) ? languageFile[key] : key.ToString();
    }
}

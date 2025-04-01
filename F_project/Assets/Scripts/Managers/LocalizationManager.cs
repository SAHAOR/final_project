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
        tempDictionary.Add(20, isEnglish ? "You Win !!!!" : "Ganaste !!!!");
        tempDictionary.Add(21, isEnglish ? "You Lose !!!!" : "Perdiste !!!!");
        tempDictionary.Add(22, isEnglish ? "Score" : "Puntaje");
        tempDictionary.Add(23, isEnglish ? "Time" : "Tiempo");
        tempDictionary.Add(24, isEnglish ? "¡Go!" : "¡inicia!");
        tempDictionary.Add(25, isEnglish ? "Sound" : "Sonido");       


        return tempDictionary;
    }

    public string GetText(int key)
    {
        return languageFile.ContainsKey(key) ? languageFile[key] : key.ToString();
    }
}

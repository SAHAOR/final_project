using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    private Dictionary<string, string> localizedText;
    public string currentLanguage = "English"; // Idioma por defecto
    public TextAsset languageFile; // Nuevo campo para asignar en el Inspectorpublic TextAsset languageFile; // Nuevo campo para asignar en el Inspector

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            localizedText = new Dictionary<string, string>();
        }
        else
        {
            Destroy(gameObject);
        }

        if (languageFile != null)
        {
            LoadLocalizedText(languageFile);
        }
        else
        {
            Debug.LogError("? No se ha asignado el archivo de idioma en LocalizationManager.");
        }
    }


    private void Start()
    {
        if (languageFile != null)
        {
            LoadLocalizedText(languageFile);
        }
        else
        {
            Debug.LogError("No se ha asignado un archivo de idioma en LocalizationManager.");
        }
    }

    public void LoadLocalizedText(TextAsset textAsset)
    {
        localizedText = new Dictionary<string, string>();

        string[] lines = textAsset.text.Split('\n');

        int languageIndex = -1;
        string[] headers = lines[0].Split(';');

        // Buscar la columna del idioma actual
        for (int i = 1; i < headers.Length; i++)
        {
            if (headers[i].Trim() == currentLanguage)
            {
                languageIndex = i;
                break;
            }
        }

        if (languageIndex == -1)
        {
            Debug.LogError("Idioma no encontrado en el archivo.");
            return;
        }

        // Cargar las traducciones
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(';');
            if (row.Length > languageIndex)
            {
                localizedText[row[0].Trim()] = row[languageIndex].Trim();
            }
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        return "? " + key;
    }

    public void ChangeLanguage(string newLanguage, TextAsset textAsset)
    {
        currentLanguage = newLanguage;
        LoadLocalizedText(textAsset);
    }
}

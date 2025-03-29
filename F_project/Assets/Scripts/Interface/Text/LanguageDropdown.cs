using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LanguageDropDown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private List<string> availableLanguages = new List<string> { "English", "Español" };

    void Start()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(availableLanguages);

        // Cargar el idioma guardado en PlayerPrefs y reflejarlo en el Dropdown
        string savedLanguage = PlayerPrefs.GetString("Language", "English");
        dropdown.value = availableLanguages.IndexOf(savedLanguage);
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    void ChangeLanguage(int index)
    {
        string selectedLanguage = availableLanguages[index];
        LocalizationManager.Instance.LoadLanguage(selectedLanguage);
    }
}

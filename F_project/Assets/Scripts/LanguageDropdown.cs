using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public LocalizationManager localizationManager;

    void Start()
    {
        // Asegurar que el Dropdown tenga opciones
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string> { "English", "Español" });

        // Cargar el idioma guardado o el predeterminado
        int savedIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);
        dropdown.value = savedIndex;

        // Agregar el listener para el cambio de idioma
        dropdown.onValueChanged.AddListener(delegate { ChangeLanguage(); });

        // Aplicar el idioma inicial
        ChangeLanguage();
    }

    public void ChangeLanguage()
    {
        string selectedLanguage = dropdown.options[dropdown.value].text;
        localizationManager.ChangeLanguage(selectedLanguage, localizationManager.languageFile);

        // Guardar el idioma seleccionado en PlayerPrefs
        PlayerPrefs.SetInt("SelectedLanguage", dropdown.value);
        PlayerPrefs.Save();

        // Actualizar los textos en la escena con la nueva sintaxis
        LocalizedText[] texts = Object.FindObjectsByType<LocalizedText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (LocalizedText text in texts)
        {
            text.UpdateText();
        }
    }
}

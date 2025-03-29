using UnityEngine;
using TMPro; // Importar TextMeshPro
using System.Collections.Generic;
public class LocalizedText : MonoBehaviour
{
  public int key;
    private TMP_Text textComponent;
    private static List<LocalizedText> allTexts = new List<LocalizedText>();

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        allTexts.Add(this);
    }

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null)
            textComponent.text = LocalizationManager.Instance.GetText(key);
    }

    public static void UpdateAllTexts()
    {
        foreach (var langText in allTexts)
        {
            langText.UpdateText();
        }
    }
}

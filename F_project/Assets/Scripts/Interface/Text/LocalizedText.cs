using UnityEngine;
using TMPro; // Importar TextMeshPro

public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text textComponent; // Usar TMP_Text en vez de Text

    void Start()
    {
        textComponent = GetComponent<TMPro.TMP_Text>();

        if (textComponent == null)
        {
            Debug.LogWarning($"?? No se encontró un componente TMP_Text en {gameObject.name}. Lo omitiré.");
            return;
        }

        if (LocalizationManager.instance == null)
        {
            Debug.LogError("? LocalizationManager no está inicializado.");
            return;
        }

        UpdateText();
    }


    public void UpdateText()
    {
        if (LocalizationManager.instance == null)
        {
            Debug.LogError("? LocalizationManager no está inicializado en UpdateText.");
            return;
        }

        if (textComponent == null)
        {
            Debug.LogWarning($"?? No se encontró un componente TMP_Text en {gameObject.name}. Lo omitiré.");
            return;
        }

        textComponent.text = LocalizationManager.instance.GetLocalizedValue(key);
    }
}

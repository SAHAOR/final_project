using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    [SerializeField] private Image brightnessPanel; // Panel de brillo
    [SerializeField] private Slider brightnessSlider; // Slider de brillo
    private const string BrightnessKey = "brightnessLevel"; // Clave para guardar la configuración

    void Start()
    {
        // Cargar el brillo guardado
        if (PlayerPrefs.HasKey(BrightnessKey))
        {
            float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey);
            brightnessSlider.value = savedBrightness;
            AjustarBrillo(savedBrightness);
        }
        else
        {
            AjustarBrillo(0.5f); // Valor por defecto
        }

        // Asignar evento al Slider
        brightnessSlider.onValueChanged.AddListener(AjustarBrillo);
    }

    public void AjustarBrillo(float value)
    {
        if (brightnessPanel != null)
        {
            // Convertimos el rango del slider (0 a 1) al rango del Alpha (0 a 128 en el inspector)
            float alpha = Mathf.Lerp(0f, 128f, 1 - value) / 255f;

            // Asignamos el nuevo color con el Alpha modificado
            brightnessPanel.color = new Color(brightnessPanel.color.r, brightnessPanel.color.g, brightnessPanel.color.b, alpha);
        }

        // Guardamos el nivel de brillo en PlayerPrefs
        PlayerPrefs.SetFloat("brightnessLevel", value);
    }


}

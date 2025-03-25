using UnityEngine;
using UnityEngine.UI;

public class BrilloUI : MonoBehaviour
{
    public Slider slider;
    public Image panelBrillo;

    //inicializa la configuración del brillo y configura un slider para que el usuario pueda ajustarlo.
    void Start()
    {
        if (BrilloManager.Instance != null)
        {
            float brilloGuardado = BrilloManager.Instance.GetBrillo();
            slider.value = brilloGuardado;
            UpdateBrightness(brilloGuardado);
        }

        slider.onValueChanged.AddListener(ChangeBrillo);
    }

    //actualiza el nivel de brillo cuando el usuario ajusta el slider.
    public void ChangeBrillo(float valor)
    {
        BrilloManager.Instance.SetBrillo(valor);
        UpdateBrightness(valor);
    }

    //ajusta la transparencia (alfa) de un panel de brillo en la interfaz.
    private void UpdateBrightness(float valor)
    {
        if (panelBrillo != null)
        {
            float alpha = 0.7f - valor; // Invertimos la relación
            panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, alpha);
        }
    }
}

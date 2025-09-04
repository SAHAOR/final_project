using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorLetter : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Color normalColor;
    public Color hoverColor;
    public Color pressedColor;

    private Button button;

    //configura el comportamiento visual de un botón al interactuar con el cursor o clics del usuario.
    void Start()
    {
        button = GetComponent<Button>();
        
        var colors = button.colors;
        colors.selectedColor = Color.white;
        button.colors = colors;

        buttonText.color = normalColor;

        var eventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        AddEventTrigger(eventTrigger, UnityEngine.EventSystems.EventTriggerType.PointerEnter, () => buttonText.color = hoverColor);
        AddEventTrigger(eventTrigger, UnityEngine.EventSystems.EventTriggerType.PointerExit, () => buttonText.color = normalColor);
        AddEventTrigger(eventTrigger, UnityEngine.EventSystems.EventTriggerType.PointerDown, () => buttonText.color = pressedColor);
        AddEventTrigger(eventTrigger, UnityEngine.EventSystems.EventTriggerType.PointerUp, () => buttonText.color = hoverColor);
    }

    //agrega dinámicamente eventos personalizados a un EventTrigger.
    private void AddEventTrigger(UnityEngine.EventSystems.EventTrigger trigger, UnityEngine.EventSystems.EventTriggerType eventType, System.Action action)
    {
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action.Invoke());
        trigger.triggers.Add(entry);
    }
}


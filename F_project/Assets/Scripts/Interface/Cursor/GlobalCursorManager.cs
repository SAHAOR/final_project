using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalCursorManager : MonoBehaviour
{
    public Texture2D normalCursor;      
    public Texture2D hoverCursor;      
    public Texture2D clickCursor;    

    private bool isCursorOverButton = false;  
    private bool isMousePressed = false;  

    private static GlobalCursorManager instance;  // Instancia estática para asegurarse de que solo haya un objeto persistente

    void Awake()
    {
        // Comprobar si ya existe una instancia del GlobalCursorManager
        if (instance == null)
        {
            instance = this;  // Si no existe, asigna la instancia actual
            DontDestroyOnLoad(gameObject);  // No destruir el objeto al cargar una nueva escena
        }
        else
        {
            Destroy(gameObject);  // Si ya existe una instancia, destruye esta copia
        }
    }

    void Start()
    {
        SetCursor(normalCursor); // Inicia el cursor normal
        AddCursorListenersToButtons(); // Añadir los listeners de los botones
    }

    void Update()
    {
        // Cambiar cursor a hover cuando esté sobre un botón y no se haya presionado el mouse
        if (isCursorOverButton && !isMousePressed)
        {
            SetCursor(hoverCursor); // Mostrar cursor de hover
        }
        
        // Si se hace clic en cualquier parte
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            if (isCursorOverButton)
            {
                SetCursor(clickCursor); // Cursor de clic en el botón
            }
        }

        // Cuando se suelta el clic, el cursor vuelve al estado de hover o normal
        if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            if (isCursorOverButton)
            {
                SetCursor(hoverCursor); // Revertir a cursor hover
            }
            else
            {
                SetCursor(normalCursor); // Si no está sobre un botón, cursor normal
            }
        }
    }

    void AddCursorListenersToButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = button.gameObject.AddComponent<EventTrigger>();

            AddEvent(trigger, EventTriggerType.PointerEnter, () =>
            {
                isCursorOverButton = true;
                if (!isMousePressed)
                    SetCursor(hoverCursor); // Cambiar a hover cuando entra el cursor
            });

            AddEvent(trigger, EventTriggerType.PointerExit, () =>
            {
                isCursorOverButton = false;
                if (!isMousePressed)
                    SetCursor(normalCursor);  // Cambiar a normal cuando sale el cursor
            });
        }
    }

    void SetCursor(Texture2D cursor)
    {
        if (cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    void AddEvent(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}

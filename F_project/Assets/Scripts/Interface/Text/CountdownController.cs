using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public GameObject countdownPanel; // Panel del contador
    public TMP_Text countdownText; // Texto de la cuenta regresiva
    public GameObject localizedTextPrefab; // Prefab con el texto localizado
    public Animator curtainAnimator; // Animador del telón
    public string curtainAnimationName = "CurtainClose"; // Nombre de la animación del telón

    private LocalizedText endMessage; // Variable para el texto localizado

    void Start()
    {
        countdownPanel.SetActive(false); // Ocultar el panel al inicio
        StartCoroutine(DisableMouseTemporarily(5f)); 
        StartCoroutine(WaitForCurtainAndStartCountdown());

    }

     IEnumerator DisableMouseTemporarily(float seconds)
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor
        Cursor.visible = false; // Lo oculta

        yield return new WaitForSeconds(seconds);

        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
        Cursor.visible = true; // Lo hace visible
    }

    IEnumerator WaitForCurtainAndStartCountdown()
    {
        // Esperar a que termine la animación del telón
        while (curtainAnimator.GetCurrentAnimatorStateInfo(0).IsName(curtainAnimationName) &&
               curtainAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Esperar 1 segundo antes de mostrar el 3

        countdownPanel.SetActive(true); // Mostrar el contador

        // Instanciar el prefab del texto localizado y acceder a su componente LocalizedText
        GameObject localizedTextObject = Instantiate(localizedTextPrefab);
        endMessage = localizedTextObject.GetComponent<LocalizedText>();

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = endMessage.GetComponent<TMP_Text>().text; // Usar el texto del LocalizedText
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false); // Ocultar el contador al terminar
    }
}

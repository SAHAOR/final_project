using UnityEngine;
using Photon.Pun;
public class ButtonSfx : MonoBehaviour
{
   [SerializeField] private string sfxName; // Nombre del SFX que se reproducirá

    [PunRPC]
    public void PlaySfx()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(sfxName);
        }
        else
        {
            Debug.LogWarning("AudioManager no está disponible.");
        }
    }
}

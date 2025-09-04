using UnityEngine;
using Photon.Pun;

public class BackMenu : MonoBehaviour
{

    public void ExitToMenu()
    {
        UIManager.instance.CargarEscenaJuego("Felipe");
        PhotonNetwork.LeaveRoom();
        
    }
}

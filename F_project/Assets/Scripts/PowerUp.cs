using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Linq;

public class PowerUp : MonoBehaviourPun
{
    private void OnMouseDown()
    {
            int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            Player opponent = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber != myActorNumber);

            if (opponent != null)
            {
                // Llamar al método del GameManager para congelar al oponente
                ObjectSpawner.instance.FreezePlayer(opponent.ActorNumber);
            }

            ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID); 

    }
}

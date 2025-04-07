using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Linq;

public class PowerUp : MonoBehaviourPun
{

    void Start()
    {
        StartCoroutine(AutoDestroyAfterTime(5f));
    }
    private void OnMouseDown()
    {
            int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            Player opponent = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber != myActorNumber);

            if (opponent != null)
            {
                ObjectSpawner.instance.photonView.RPC("FreezePlayer", RpcTarget.MasterClient, opponent.ActorNumber, PhotonNetwork.LocalPlayer.ActorNumber); 
            }

            ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID); 

    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {
        Debug.Log($"⏳ {gameObject.name} comenzará a contar {time} segundos para autodestruirse.");
        yield return new WaitForSeconds(time);

        ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID);
    }
}

using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using System.Linq;

public class PowerUp : MonoBehaviourPun
{

    public ParticleSystem destroyParticles;

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

            GetComponent<MeshRenderer>().enabled = false;
            if (destroyParticles != null)
            {
                destroyParticles.transform.parent = null; // para que no desaparezca junto con el objeto
                destroyParticles.Play();
            }

            ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID); 
            AudioManager.instance.PlaySfx("Click Power Up"); // Reproducir SFX al hacer clic

    }

    private IEnumerator AutoDestroyAfterTime(float time)
    {
        Debug.Log($"⏳ {gameObject.name} comenzará a contar {time} segundos para autodestruirse.");
        yield return new WaitForSeconds(time);

        ObjectSpawner.instance.photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID);
    }
}

using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class AnimationUI : MonoBehaviour
{    
    [SerializeField] private GameObject menuExtra;
    void Start()
    {

    }

    public void ActivarMenuExtra ()
    {
        LeanTween.moveX(menuExtra.GetComponent<RectTransform>(), 30, 1f).setEase(LeanTweenType.easeOutElastic);
    }
    public void EsconderMenuExtra ()
    {
        LeanTween.moveX(menuExtra.GetComponent<RectTransform>(), -125, 1f).setEase(LeanTweenType.easeOutElastic);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopUpController : MonoBehaviour
{
    [SerializeField] private Transform initialPos;
    [SerializeField] private Transform finalPos;
    [SerializeField] Ease ease;
    [SerializeField] private float timeAmmount, delayToAppear, delayToDisappear;

    private void Awake()
    {
        Destroy(gameObject, delayToAppear + timeAmmount + delayToDisappear + 1f);


        

        Invoker.InvokeDelayed(Appear, delayToAppear);
        
    }

    private void Appear()
    {
        //transform.DOMove(finalPos.position, timeAmmount).SetEase(ease);
        // move and squash
        transform.DOMove(finalPos.position, timeAmmount).SetEase(ease);
        transform.DORotate(finalPos.rotation.eulerAngles, timeAmmount).SetEase(ease);
        
        //Debug.Log("ForcedPause: "+ FindObjectOfType<GameManagerProxy>().IsForcedPause());
        Invoker.InvokeDelayed(DelayPause, 1f);
        Invoker.InvokeDelayed(Disappear, timeAmmount + delayToDisappear);
        Debug.Log("Appear");

        
    }

    private void Disappear()
    {

        Time.timeScale = 1;
        FindObjectOfType<GameManagerProxy>().ForcedPause(false);
        transform.DOMove(initialPos.position, timeAmmount).SetEase(ease);
        transform.DORotate(initialPos.rotation.eulerAngles, timeAmmount).SetEase(ease);
        Debug.Log("ForcedPause: " + FindObjectOfType<GameManagerProxy>().IsForcedPause());
        Debug.Log("Disappear");
    }

    private void DelayPause()
    {
        FindObjectOfType<GameManagerProxy>().ForcedPause(true);
        Time.timeScale = 0.0001f;
    }
}

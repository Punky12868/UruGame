using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;

public class PopUpController : MonoBehaviour
{
    [SerializeField] private Transform initialPos;
    [SerializeField] private Transform finalPos;
    [SerializeField] Ease ease;
    [SerializeField] private float timeAmmount, delayToAppear, delayToDisappear;
    private Player input;
    public bool attackTuto;
    public bool rollTuto;
    public bool parryTuto;
    private bool active;


    private void Awake()
    {
        Destroy(gameObject, delayToAppear + timeAmmount + delayToDisappear + 1f);

        input = ReInput.players.GetPlayer(0);


        //Invoker.InvokeDelayed(Appear, delayToAppear);
        Invoke("Appear", delayToAppear);
        
    }

    private void Update()
    {
        DeletePopUp();
    }
    private void Appear()
    {
        //transform.DOMove(finalPos.position, timeAmmount).SetEase(ease);
        // move and squash
        transform.DOMove(finalPos.position, timeAmmount).SetEase(ease);
        transform.DORotate(finalPos.rotation.eulerAngles, timeAmmount).SetEase(ease);
        //Invoker.InvokeDelayed(CooldownActive, 1);
        //Invoker.InvokeDelayed(Disappear, timeAmmount + delayToDisappear);

        Invoke("CooldownActive", 1);
        Invoke("Disappear", timeAmmount + delayToDisappear);
        //Debug.Log("Appear");


    }

    private void Disappear()
    {
        active = false;
        FindObjectOfType<GameManagerProxy>().ForcedPause(false);
        transform.DOMove(initialPos.position, timeAmmount).SetEase(ease);
        transform.DORotate(initialPos.rotation.eulerAngles, timeAmmount).SetEase(ease);
        //Debug.Log("ForcedPause: " + FindObjectOfType<GameManagerProxy>().IsForcedPause());
        //Debug.Log("Disappear");
    }

    private void CooldownActive()
    {
        active = true;
    }


    private void DeletePopUp()
    {
        if (active)
        {
            if (attackTuto)
            {
                if (input.GetButtonDown("Attack"))
                {
                    Disappear();
                }
            }
            if (rollTuto)
            {
                if (input.GetButtonDown("Roll"))
                {
                    Disappear();
                }
            }
            if (parryTuto)
            {
                if (input.GetButtonDown("Parry"))
                {
                    Disappear();
                }
            }
        }
        
    }

}

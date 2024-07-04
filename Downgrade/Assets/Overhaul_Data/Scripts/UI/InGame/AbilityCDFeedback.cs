using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCDFeedback : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    public Image image;
    public float cooldownTime;
    public bool playAnim = false;
    void Start()
    {
        cooldownTime = FindObjectOfType<PlayerControllerOverhaul>().GetAbilityCooldown();
        //Invoke("delaySatart", 0.1f);
        image.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playAnim)
        {
            if (image.fillAmount <= 0)
            {
                playAnim = false;

            }
            else 
            {
                image.fillAmount -= Time.deltaTime / cooldownTime;
            }
        }
       
        
    }

    public void ActiveAnim()
    {
        playAnim = true;
        image.fillAmount = 1;
    }
    public void OnPlayerNotify(AllPlayerActions actions)
    {
        switch (actions)
        {
            case AllPlayerActions.useAbility:
                
                break;
        }
    }

    public void OnEnemyNotify(AllEnemyActions actions)
    {

    }


    public void OnBossesNotify(AllBossActions actions)
    {

    }
}

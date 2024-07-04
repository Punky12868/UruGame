using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FeedbackDG : MonoBehaviour
{
    public Image image;
    [HideInInspector] public SelectedDowngrade downgrade;
    private DGType type;
    private float timeToDisapear;
    private bool animPlay;
    [HideInInspector]public float maxRolls;

    private bool notBosque = true;
    public IconHolder holder;

    private Color imageColor = new Color(0, 0, 0, 0.7f);
    void Start()
    {
        //Invoker.InvokeDelayed(DelayAwake, 0.2f);
        DelayAwake();
    }

    // Update is called once per frame
    void Update()
    {
        if (notBosque == false) { return; }

        if (animPlay)
        {
            if (image.fillAmount <= 0)
            {

                animPlay = false;
                image.fillAmount = 1;
                image.sprite = holder.vacio;
            }
            else
            {
                image.sprite = holder.image;
                image.fillAmount -= Time.deltaTime / timeToDisapear;
            }
            
        }

       
    }
    public void SwitchIcon(bool active)
    {
        if (active)
        {
            image.sprite = holder.vacio;
        }
        else
        {
            image.sprite = holder.image;
        }
    }
    public void PlayAnimation(float Value, float SecondValue = 0)
    {
        image.fillAmount = 1;
        timeToDisapear = Value;
        animPlay = true;  
    }
    public void SetInitial()
    {
        switch (type)
        {
            case DGType.Count:
                image.fillAmount = 1;
                image.sprite = holder.image;
                break; 
            case DGType.Time:
                image.fillAmount = 0; 
                break; 
            case DGType.Instant:
                image.fillAmount = 0; 
                break; 
            case DGType.Switch:
                image.fillAmount = 1; 
                break;


        }   
    }
    void DelayAwake()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 2)
        {
            notBosque = false;
        }
        else
        {
            FindObjectOfType<DowngradeSystem>().SetDowngradeFeedback(this);
            image.color = imageColor;
            SelectDGType(downgrade);
            SetInitial();
        }
        
    }
    void SelectDGType(SelectedDowngrade downgrade)
    {
        if(notBosque== false) { return; }

        switch (downgrade)
        {
            case SelectedDowngrade.Dados:
                type = DGType.Count;
                break;
            case SelectedDowngrade.Daga:
                type = DGType.Switch;
                break;
            case SelectedDowngrade.Debil:
                type = DGType.Time;
                break;
            case SelectedDowngrade.Esqueleto:
                type = DGType.Time;
                break;
            case SelectedDowngrade.Moneda:
                type = DGType.Instant;
                break;
            case SelectedDowngrade.Paralisis:
                type = DGType.Time;
                break;
            case SelectedDowngrade.Rodilla:
                type = DGType.Instant;
                break;
            case SelectedDowngrade.Slime:
                type = DGType.Time;
                break;
            case SelectedDowngrade.Stamina:
                type = DGType.Instant;
                break;
            case SelectedDowngrade.None:
                image.sprite = holder.vacio;
                break;
        }
    }
    public enum DGType
    {
        Count,
        Time,
        Instant,
        Switch
    }
    [System.Serializable]
    public class IconHolder
    {
        public Sprite vacio;
        public Sprite image;
    }
}

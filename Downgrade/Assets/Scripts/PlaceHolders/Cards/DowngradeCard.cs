using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DowngradeCard : ScriptableObject
{
    //public GameObject cardObject;
    public SelectedDowngrade selectedDowngrade;
    public Sprite dgIcon;
    public Sprite cardSprite;
    public Sprite cardBackSprite;
    public string cardName;
    public string cardStat;
    public string cardDescription;

    public virtual void CardEffect()
    {
    }
}

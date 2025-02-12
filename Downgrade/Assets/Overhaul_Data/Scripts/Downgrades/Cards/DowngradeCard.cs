using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DowngradeCard : ScriptableObject
{
    [Header("Downgrade ID")]
    public SelectedDowngrade selectedDowngrade;

    [Header("Card Art")]
    public Sprite dgIcon;
    public Sprite cardSprite;
    public Sprite cardSelectedSprite;
    public Sprite cardBackSprite;

    [Header("Card Info")]
    public string cardName;
    public string cardStat;

    [TextArea(10, 20)]
    public string cardDescription;

    [Header("Custom Card Stats")]
    public bool placeholder;

    public virtual void CardEffect()
    {
    }
}

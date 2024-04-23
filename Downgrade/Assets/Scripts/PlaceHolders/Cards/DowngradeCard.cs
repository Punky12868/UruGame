using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DowngradeCard : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Sprite cardSprite;

    public virtual void CardEffect()
    {
    }
}

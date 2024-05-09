using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourType { Movement, Attack }
public class BehaviourBase : ScriptableObject
{
    public BehaviourType behaviourType;

    public virtual void MovementBehaviour()
    {
    }

    public virtual void AttackBehaviour()
    {
    }
}

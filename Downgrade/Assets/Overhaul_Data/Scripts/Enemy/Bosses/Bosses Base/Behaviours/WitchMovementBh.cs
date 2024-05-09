using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WitchMovementBh", menuName = "Behaviours/WitchMovementBh")]
public class WitchMovementBh : BehaviourBase
{
    protected Transform boss;
    protected Vector3 targetDir;
    protected float speed;

    public virtual void MovementBehaviour()
    {
        boss.position += targetDir * speed * Time.deltaTime;
    }

    public void SetDynamicVariables(Vector3 targetDir)
    {
        this.targetDir = targetDir;
    }

    public void SetStaticVariables(Transform boss, float speed)
    {
        this.boss = boss;
        this.speed = speed;
    }
}

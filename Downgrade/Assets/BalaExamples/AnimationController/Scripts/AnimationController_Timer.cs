using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController_Timer : MonoBehaviour
{
    private AnimationController animLogic;
    private bool isUsingTimeDeltaTime;

    public void SetAnimatorController(AnimationController controller, bool usingTimeDeltaTime = true)
    {
        animLogic = controller;
    }

    void Update()
    {
        if (animLogic == null) return;

        if (isUsingTimeDeltaTime)
        {
            animLogic.UpdateTimer(Time.deltaTime);
        }
        else
        {
            animLogic.UpdateTimer(Time.unscaledDeltaTime);
        }
    }
}

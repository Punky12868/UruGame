using UnityEngine;

public class AnimationController_Timer : MonoBehaviour
{
    private AnimationController animLogic;
    private bool isUsingTimeDeltaTime;

    public void SetAnimatorController(AnimationController controller)
    {
        animLogic = controller;
    }

    public void SetTimeSettings(bool value)
    {
        isUsingTimeDeltaTime = value;
    }

    void Update()
    {
        if (animLogic == null) return;

        if (isUsingTimeDeltaTime)
        {
            animLogic.UpdateTimer(Time.deltaTime);
        }
        else if (!isUsingTimeDeltaTime)
        {
            animLogic.UpdateTimer(Time.unscaledDeltaTime);
        }
    }
}

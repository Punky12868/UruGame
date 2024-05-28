using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMode : MonoBehaviour
{
    // select fullscren mode or windowed mode

    public void SetFullScreen(bool isFullScreen)
    {
        SetResolution(1920, 1080, isFullScreen);
    }

    public void SetResolution(int width, int height, bool isFullScreen)
    {
        if (isFullScreen) Screen.SetResolution(width, height, isFullScreen);
        else Screen.SetResolution(width / 2, height / 2, isFullScreen);
        FindObjectOfType<AspectRatioUtility>().UpdateAspectRatio();
    }
}

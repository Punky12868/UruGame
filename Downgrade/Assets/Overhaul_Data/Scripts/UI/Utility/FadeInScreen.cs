using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScreen : MonoBehaviour
{
    public float fadeSpeed = 0.5f;
    public bool fadeOut = true;
    private bool isFadeDone;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (fadeOut)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    private void Update()
    {
        if (isFadeDone) return;

        if (fadeOut)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                isFadeDone = true;
                //fadeIn = false;
            }
        }
        else
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                isFadeDone = true;
                //fadeIn = true;
            }
        }
    }

    public void SetFade(bool value)
    {
        isFadeDone = value;
    }
}

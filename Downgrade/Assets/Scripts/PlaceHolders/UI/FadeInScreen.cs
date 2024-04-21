using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScreen : MonoBehaviour
{
    public float fadeSpeed = 0.5f;
    public bool fadeIn = true;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (fadeIn)
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
        if (fadeIn)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //fadeIn = false;
            }
        }
        else
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                //fadeIn = true;
            }
        }
    }
}

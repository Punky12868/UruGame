using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransparencyOverTransition : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.2f;

    private ChapterSelector chapterSelector;
    private CanvasGroup canvasGroup;
    private bool faded;

    private void Awake()
    {
        chapterSelector = FindObjectOfType<ChapterSelector>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (chapterSelector.GetTransition() && !faded) { faded = true; canvasGroup.DOFade(0, fadeDuration); }
        if (!chapterSelector.GetTransition() && faded) { faded = false; canvasGroup.DOFade(1, fadeDuration); }
    }
}

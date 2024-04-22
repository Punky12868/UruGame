using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public bool active = false;
    public bool activated = false;
    public float fadeSpeed = 1;
    public float timeToActivate = 1;

    [SerializeField] private GameObject deadText;
    [SerializeField] private GameObject victoryText;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (active)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;

            if (canvasGroup.alpha >= 1)
            {
                GameManager.Instance.RestartGame();
            }
        }
    }

    public void OnDeath()
    {
        if (!activated)
        {
            activated = true;
            deadText.SetActive(true);
            Invoke("Activate", timeToActivate);
        }
    }

    public void OnVictory()
    {
        if (!activated)
        {
            activated = true;
            victoryText.SetActive(true);
            Invoke("Activate", timeToActivate);
        }
    }

    private void Activate()
    {
        active = true;
    }
}

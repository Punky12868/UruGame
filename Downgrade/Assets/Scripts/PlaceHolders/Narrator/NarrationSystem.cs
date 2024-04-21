using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationSystem : MonoBehaviour, IObserver
{
    [SerializeField] Subject player;
    [SerializeField] Transform subtitlesParent;
    [SerializeField] GameObject subtitles;

    [SerializeField] float aditionalTimeAfterNarration = 1f;
    [SerializeField] float lowHealthThreshold = 10f;
    [SerializeField] float lowStaminaThreshold = 10f;
    [SerializeField] int attackThresshold = 15;
    [SerializeField] int parryThreshold = 3;
    [SerializeField] int dodgeThreshold = 5;
    [SerializeField] int hitThreshold = 10;
    [SerializeField] float timeAfterRandomNoise = 30;
    float timeForRandomNoise;

    bool isWaiting = false;

    [SerializeField] AudioClip[] narrationCLips;

    [TextArea]
    [SerializeField] string[] subtitlesText;

    public void OnNotify(AllActions actions)
    {
        if (isWaiting)
            return;

        
        switch (actions)
        {
            case AllActions.Start:
                Debug.Log("Game Started");
                break;
            case AllActions.End:
                Debug.Log("Game Ended");
                break;
            case AllActions.LowHealth: //
                if (FindObjectOfType<PlayerController>().GetCurrentHealth() < lowHealthThreshold)
                {
                    AudioManager.instance.PlayVoice(narrationCLips[0]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[0], narrationCLips[0].length);
                    SetWaiting(narrationCLips[0].length);
                    Debug.Log("Low Health");
                }
                break;
            case AllActions.LowStamina: //
                if (FindObjectOfType<PlayerController>().GetCurrentStamina() < lowStaminaThreshold)
                {
                    AudioManager.instance.PlayVoice(narrationCLips[1]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[1], narrationCLips[1].length);
                    SetWaiting(narrationCLips[1].length);
                    Debug.Log("Low Stamina");
                }
                break;
            case AllActions.NotKilling:
                Debug.Log("Not Killing");
                break;
            case AllActions.RandomNoise:
                Debug.Log("Random Noise");
                break;
            case AllActions.StartBoss:
                Debug.Log("Boss Fight Started");
                break;
            case AllActions.DieToFirstStageBoss:
                Debug.Log("Died to First Stage Boss");
                break;
            case AllActions.MidBoss:
                Debug.Log("Mid Boss Fight");
                break;
            case AllActions.DieToMidStageBoss:
                Debug.Log("Died to Mid Stage Boss");
                break;
            case AllActions.ParryBoss:
                Debug.Log("Parried Boss Attack");
                break;
            case AllActions.EndBoss:
                Debug.Log("Boss Fight Ended");
                break;
            case AllActions.Attack: //
                Debug.Log("Attacked");
                break;
            case AllActions.Parry: //
                Debug.Log("Parried");
                break;
            case AllActions.Dodge:
                Debug.Log("Dodged");
                break;
            case AllActions.Hit:
                Debug.Log("Hit");
                break;
            case AllActions.Die:
                AudioManager.instance.PlayVoice(narrationCLips[2]);
                Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[2], narrationCLips[2].length);
                SetWaiting(narrationCLips[2].length);
                Debug.Log("Died");
                break;
            case AllActions.Victory:
                Debug.Log("Victory");
                break;
            case AllActions.Defeat:
                Debug.Log("Defeat");
                break;
            case AllActions.None:
                Debug.Log("None");
                break;
        }
    }

    private void OnEnable()
    {
        player.AddObserver(this);
    }

    private void OnDisable()
    {
        player.RemoveObserver(this);
    }

    private void Update()
    {
        if (timeForRandomNoise >= timeAfterRandomNoise)
        {
            OnNotify(AllActions.RandomNoise);
            timeForRandomNoise = 0;
        }
        else
        {
            timeForRandomNoise += Time.deltaTime;
        }
    }

    private void SetWaiting(float lenht)
    {
        isWaiting = true;
        Invoke("ResetWaiting", lenht + aditionalTimeAfterNarration);
    }

    private void ResetWaiting()
    {
        isWaiting = false;
    }
}

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
    bool lowStaminaTriggered = false;
    [SerializeField] float lowStaminaThreshold = 10f;
    [SerializeField] int attackThresshold = 15;
    [SerializeField] int parryThreshold = 3;
    [SerializeField] int dodgeThreshold = 5;
    [SerializeField] int hitThreshold = 10;
    [SerializeField] float timeAfterRandomNoise = 30;
    float timeForRandomNoise;

    bool isWaiting = false;

    [SerializeField] AudioClip[] narrationClips;

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
                if (FindObjectOfType<PlayerComponent>().GetCurrentHealth() < lowHealthThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    AudioManager.instance.PlayVoice(narrationClips[0]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[0], narrationClips[0].length);
                    SetWaiting(narrationClips[0].length);
                    Debug.Log("Low Health");
                }
                break;
            case AllActions.LowStamina: //
                if (FindObjectOfType<PlayerComponent>().GetCurrentStamina() < lowStaminaThreshold)
                {
                    AudioManager.instance.PlayVoice(narrationClips[1]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[1], narrationClips[1].length);
                    SetWaiting(narrationClips[1].length);
                    Debug.Log("Low Stamina");
                }
                break;
            case AllActions.NotKilling:
                Debug.Log("Not Killing");
                break;
            case AllActions.RandomNoise:
                Debug.Log("Random Noise");
                break;
            case AllActions.RandomPausedNoise:
                Debug.Log("Random Paused Noise");
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
                AudioManager.instance.PlayVoice(narrationClips[2]);
                Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[2], narrationClips[2].length);
                SetWaiting(narrationClips[2].length);
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
            if (GameManager.Instance.IsGamePaused())
            {
                OnNotify(AllActions.RandomPausedNoise);
            }
            else
            {
                OnNotify(AllActions.RandomNoise);
            }
            timeForRandomNoise = 0;
        }
        else
        {
            timeForRandomNoise += Time.unscaledDeltaTime;
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

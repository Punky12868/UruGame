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

    public void OnPlayerNotify(AllPlayerActions actions)
    {
        if (isWaiting)
            return;

        
        switch (actions)
        {
            case AllPlayerActions.Start:
                Debug.Log("Game Started");
                break;
            case AllPlayerActions.End:
                Debug.Log("Game Ended");
                break;
            case AllPlayerActions.LowHealth: //
                if (FindObjectOfType<PlayerComponent>().GetHealth() < lowHealthThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    AudioManager.instance.PlayVoice(narrationClips[0]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[0], narrationClips[0].length);
                    SetWaiting(narrationClips[0].length);
                    Debug.Log("Low Health");
                }
                break;
            case AllPlayerActions.LowStamina: //
                if (FindObjectOfType<PlayerComponent>().GetStamina() < lowStaminaThreshold)
                {
                    AudioManager.instance.PlayVoice(narrationClips[1]);
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[1], narrationClips[1].length);
                    SetWaiting(narrationClips[1].length);
                    Debug.Log("Low Stamina");
                }
                break;
            case AllPlayerActions.NotKilling:
                Debug.Log("Not Killing");
                break;
            case AllPlayerActions.RandomNoise:
                Debug.Log("Random Noise");
                break;
            case AllPlayerActions.RandomPausedNoise:
                Debug.Log("Random Paused Noise");
                break;
            case AllPlayerActions.StartBoss:
                Debug.Log("Boss Fight Started");
                break;
            case AllPlayerActions.DieToFirstStageBoss:
                Debug.Log("Died to First Stage Boss");
                break;
            case AllPlayerActions.MidBoss:
                Debug.Log("Mid Boss Fight");
                break;
            case AllPlayerActions.DieToMidStageBoss:
                Debug.Log("Died to Mid Stage Boss");
                break;
            case AllPlayerActions.ParryBoss:
                Debug.Log("Parried Boss Attack");
                break;
            case AllPlayerActions.EndBoss:
                Debug.Log("Boss Fight Ended");
                break;
            case AllPlayerActions.Attack: //
                Debug.Log("Attacked");
                break;
            case AllPlayerActions.Parry: //
                Debug.Log("Parried");
                break;
            case AllPlayerActions.Dodge:
                Debug.Log("Dodged");
                break;
            case AllPlayerActions.Hit:
                Debug.Log("Hit");
                break;
            case AllPlayerActions.Die:
                AudioManager.instance.PlayVoice(narrationClips[2]);
                Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[2], narrationClips[2].length);
                SetWaiting(narrationClips[2].length);
                Debug.Log("Died");
                break;
            case AllPlayerActions.Victory:
                Debug.Log("Victory");
                break;
            case AllPlayerActions.Defeat:
                Debug.Log("Defeat");
                break;
            case AllPlayerActions.None:
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
                OnPlayerNotify(AllPlayerActions.RandomPausedNoise);
            }
            else
            {
                OnPlayerNotify(AllPlayerActions.RandomNoise);
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

    public void OnEnemyNotify(AllEnemyActions actions)
    {
    }
}

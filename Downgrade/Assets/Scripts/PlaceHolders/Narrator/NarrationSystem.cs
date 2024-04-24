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
    bool lowHealthTriggered = false;
    bool lowStaminaTriggered = false;
    [SerializeField] float lowStaminaThreshold = 10f;
    [SerializeField] int attackThresshold = 15;
    [SerializeField] int parryThreshold = 3;
    [SerializeField] int dodgeThreshold = 5;
    [SerializeField] int hitThreshold = 10;
    [SerializeField] float timeAfterRandomNoise = 30;
    float timeForRandomNoise;

    bool isWaiting = false;
    bool isWaitingPatch = false;

    [SerializeField] AudioClip[] narrationClips;
    [SerializeField] AudioClip[] inBetweenNarrationClips;
    private int chainedIndex = 0;

    [TextArea]
    [SerializeField] string[] subtitlesText;
    [SerializeField] string[] inBetweenSubtitlesText;

    public void PlaySubs(int index)
    {
        if (isWaiting || isWaitingPatch)
        {
            int random = Random.Range(0, inBetweenNarrationClips.Length);

            chainedIndex = index;
            AudioManager.instance.PlayVoice(inBetweenNarrationClips[random]);
            FindObjectOfType<Subtitles>().DisplayOnPlayingSubtitles(inBetweenSubtitlesText[random], inBetweenNarrationClips[random].length);

            Invoker.InvokeDelayed(ChainedDialog, inBetweenNarrationClips[random].length);
        }
        else
        {
            AudioManager.instance.PlayVoice(narrationClips[index]);
            Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(subtitlesText[index], narrationClips[index].length);
            isWaiting = true;
            Invoker.InvokeDelayed(ResetWait, narrationClips[index].length);
        }
    }

    private void ChainedDialog()
    {
        AudioManager.instance.PlayVoice(narrationClips[chainedIndex]);
        FindObjectOfType<Subtitles>().DisplayOnPlayingSubtitles(subtitlesText[chainedIndex], narrationClips[chainedIndex].length);
        isWaiting = true;
        Invoker.InvokeDelayed(ResetWaitPatch, narrationClips[chainedIndex].length);
    }

    private void ResetWait()
    {
        isWaiting = false;
        isWaitingPatch = false;
    }

    private void ResetWaitPatch()
    {
        isWaiting = false;
        isWaitingPatch = false;
    }

    public void OnPlayerNotify(AllPlayerActions actions)
    {
        /*if (isWaiting)
            return;*/

        
        switch (actions)
        {
            case AllPlayerActions.Start:
                Debug.Log("Game Started");
                break;
            case AllPlayerActions.End:
                Debug.Log("Game Ended");
                break;
            case AllPlayerActions.LowHealth: //
                if (FindObjectOfType<PlayerComponent>().GetHealth() < lowHealthThreshold && !lowHealthTriggered)
                {
                    lowHealthTriggered = true;
                    PlaySubs(0);
                    Debug.Log("Low Health");
                }
                break;
            case AllPlayerActions.LowStamina: //
                if (FindObjectOfType<PlayerComponent>().GetStamina() < lowStaminaThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    PlaySubs(1);
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
                PlaySubs(2);
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

    public void OnEnemyNotify(AllEnemyActions actions)
    {
    }
}

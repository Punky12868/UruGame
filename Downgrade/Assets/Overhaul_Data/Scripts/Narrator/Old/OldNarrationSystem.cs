using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldNarrationSystem : MonoBehaviour, IObserver
{
    [SerializeField] Subject player;
    [SerializeField] Transform subtitlesParent;
    [SerializeField] GameObject subtitles;

    [SerializeField] string subtitleKey = "subs";
    [SerializeField] bool subtitlesActivated = true;
    [SerializeField] int startDialogChainLength;
    int startDialogChainIndex;

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

    [Header("Audio")]
    public bool placeHolderAudio;
    [SerializeField] AudioClip[] startGame, endGame, playerLowHealth, playerHeal, playerLowStamina, playerHitEnemy, playerKilledEnemy, playerNotKilling;
    [SerializeField] AudioClip[] randomNoise, randomPausedNoise, startBoss, playerDieToFirstFaseBoss, midBoss, playerDieToMidFaceBoss, parryBoss, endBoss;
    [SerializeField] AudioClip[] playerAttack, playerParry, playerDodge, playerUseItem, playerUseEmptyItem, playerPickUpItem, playerDropItem, playerDropEmptyItem, playerGotHit, playerDies, victory, defeat;
    [SerializeField] AudioClip[] inBetweenNarrationClips;

    private AudioClip chainedClip;
    private string chainedDialog;

    [Header("Dialog")]
    public bool placeHolderDialog;
    [TextArea][SerializeField] string[] startGameDialog, endGameDialog, playerLowHealthDialog, playerHealDialog, playerLowStaminaDialog, playerHitEnemyDialog, playerKilledEnemyDialog, playerNotKillingDialog;
    [TextArea][SerializeField] string[] randomNoiseDialog, randomPausedNoiseDialog, startBossDialog, playerDieToFirstFaseBossDialog, midBossDialog, playerDieToMidFaceBossDialog, parryBossDialog, endBossDialog;
    [TextArea][SerializeField] string[] playerAttackDialog, playerParryDialog, playerDodgeDialog, playerUseItemDialog, playerUseEmptyItemDialog, playerPickUpItemDialog, playerDropItemDialog, playerDropEmptyItemDialog, playerGotHitDialog, playerDiesDialog, victoryDialog, defeatDialog;
    [TextArea][SerializeField] string[] inBetweenNarrationDialog;

    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }

    private void DelayedAwake()
    {
        subtitlesActivated = SimpleSaveLoad.Instance.LoadData<bool>(FileType.Config, subtitleKey, true);
    }

    public void PlaySubs(AudioClip[] clip, string[] dialog, bool hasPriority = false, bool isChainedOnStart = false)
    {
        Subtitles subs = null;
        int randomDialog = Random.Range(0, clip.Length);
        if (isChainedOnStart) { randomDialog = startDialogChainIndex; }

        if (subtitlesActivated) { if (FindObjectOfType<Subtitles>()) subs = FindObjectOfType<Subtitles>(); }

        if (isWaiting && hasPriority)
        {
            int randomInBetween = Random.Range(0, inBetweenNarrationClips.Length);
            if (isChainedOnStart) { randomInBetween = startDialogChainIndex;}

            if (subtitlesActivated) chainedDialog = dialog[randomDialog];
            chainedClip = clip[randomDialog];

            AudioManager.instance.PlayVoice(inBetweenNarrationClips[randomInBetween]);
            if (subtitlesActivated) subs.DisplayOnPlayingSubtitles(inBetweenNarrationDialog[randomInBetween], inBetweenNarrationClips[randomInBetween].length);
            Invoker.InvokeDelayed(ChainedDialog, inBetweenNarrationClips[randomInBetween].length);
        }
        else
        {
            int random = Random.Range(0, inBetweenNarrationClips.Length);
            if (isChainedOnStart) { random = startDialogChainIndex; }

            AudioManager.instance.PlayVoice(clip[randomDialog]);

            if (subtitlesActivated)
            {
                if (subs == null)
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(dialog[randomDialog], clip[randomDialog].length);
                else
                    subs.DisplaySubtitles(dialog[randomDialog], clip[randomDialog].length);
            }

            isWaiting = true;
            Invoker.InvokeDelayed(ResetWait, clip[randomDialog].length);
        }
    }

    private void ChainedDialog()
    {
        AudioManager.instance.PlayVoice(chainedClip);
        if (subtitlesActivated) FindObjectOfType<Subtitles>().DisplayOnPlayingSubtitles(chainedDialog, chainedClip.length);
        isWaiting = true;
        Invoker.InvokeDelayed(ResetWait, chainedClip.length);
    }

    private void ResetWait()
    {
        isWaiting = false;
        if (startDialogChainLength > startDialogChainIndex) 
        { 
            Invoke("DelayedDialog", 1f);
        }
    }

    private void DelayedDialog()
    {
        if (startGame.Length > 0 || startGameDialog.Length > 0)
        {
            PlaySubs(startGame, startGameDialog, false, true);
            startDialogChainIndex++;
        }
    }

    public void OnPlayerNotify(AllPlayerActions actions)
    {
        /*if (isWaiting)
            return;*/


        switch (actions)
        {
            case AllPlayerActions.Start:
                Debug.Log("Game Started");
                Invoke("DelayedDialog", 1f);
                break;
            case AllPlayerActions.End:
                Debug.Log("Game Ended");
                break;
            case AllPlayerActions.LowHealth: //
                if (FindObjectOfType<PlayerControllerOverhaul>().GetHealth() < lowHealthThreshold && !lowHealthTriggered)
                {
                    lowHealthTriggered = true;
                    PlaySubs(playerLowHealth, playerLowHealthDialog);
                    Debug.Log("Low Health");
                }
                break;
            case AllPlayerActions.LowStamina: //
                if (FindObjectOfType<PlayerControllerOverhaul>().GetStamina() < lowStaminaThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    PlaySubs(playerLowStamina, playerLowStaminaDialog);
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
                PlaySubs(randomPausedNoise, randomPausedNoiseDialog);
                Debug.Log("Random Paused Noise");
                break;
            case AllPlayerActions.StartBoss:
                Debug.Log("Boss Fight Started");
                break;
            case AllPlayerActions.DieToFirstFaseBoss:
                Debug.Log("Died to First Stage Boss");
                break;
            case AllPlayerActions.MidBoss:
                Debug.Log("Mid Boss Fight");
                break;
            case AllPlayerActions.DieToMidFaceBoss:
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
                PlaySubs(playerDies, playerDiesDialog, true);
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
        GameManager.onPauseGame += OnPause;
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

    private void OnPause()
    {
        timeForRandomNoise = 0;
    }

    public void OnEnemyNotify(AllEnemyActions actions)
    {
    }

    public void OnBossesNotify(AllBossActions actions)
    {
    }

    public bool GetSubtitlesActivated()
    {
        return subtitlesActivated;
    }

    public void SetSubtitlesActivated(bool value)
    {
        subtitlesActivated = value;
    }
}
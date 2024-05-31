using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class NarrationSystem : MonoBehaviour, IObserver
{
    [SerializeField] Subject player;
    [SerializeField] Transform subtitlesParent;
    [SerializeField] GameObject subtitles;

    [SerializeField] string subtitleKey = "subs";
    [SerializeField] bool subtitlesActivated = true;

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
    int dialogeContinueCount = 0;

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
    [TextArea] [SerializeField] string[] startGameDialog, endGameDialog, playerLowHealthDialog, playerHealDialog, playerLowStaminaDialog, playerHitEnemyDialog, playerKilledEnemyDialog, playerNotKillingDialog;
    [TextArea] [SerializeField] string[] randomNoiseDialog, randomPausedNoiseDialog, startBossDialog, playerDieToFirstFaseBossDialog, midBossDialog, playerDieToMidFaceBossDialog, parryBossDialog, endBossDialog;
    [TextArea] [SerializeField] string[] playerAttackDialog, playerParryDialog, playerDodgeDialog, playerUseItemDialog, playerUseEmptyItemDialog, playerPickUpItemDialog, playerDropItemDialog, playerDropEmptyItemDialog, playerGotHitDialog, playerDiesDialog, victoryDialog, defeatDialog;
    [TextArea] [SerializeField] string[] inBetweenNarrationDialog;

    [Header("BoolNextDialog")]
    public bool placeHolderBools;
    [SerializeField] bool[] startGameBool, endGameBool, playerLowHealthBool, playerHealBool, playerLowStaminaBool, playerHitEnemyBool, playerKilledEnemyBool, playerNotKillingBool;
    [SerializeField] bool[] randomNoiseBool, randomPausedNoiseBool, startBossBool, playerDieToFirstFaseBossBool, midBossBool, playerDieToMidFaceBossBool, parryBossBool, endBossBool;
    [SerializeField] bool[] playerAttackBool, playerParryBool, playerDodgeBool, playerUseItemBool, playerUseEmptyItemBool, playerPickUpItemBool, playerDropItemBool, playerDropEmptyItemBool, playerGotHitBool, playerDiesBool, victoryBool, defeatBool;
    [SerializeField] bool[] inBetweenNarrationBool;

    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }

    private void DelayedAwake()
    {
        subtitlesActivated = SimpleSaveLoad.Instance.LoadData<bool>(FileType.Config, subtitleKey, true);
    }

    public void PlaySubs(AudioClip[] clip, string[] dialog, bool[] largeDialoge, bool hasPriority = false, bool hasComeFromClip = false) //array de bools
    {
        Subtitles subs = null;
        int randomDialog = Random.Range(0, clip.Length);
        bool continueDialog = false;

        if (largeDialoge.Length > 0 || hasComeFromClip)
        {
            if (largeDialoge[0] == true)
            {
                continueDialog = true;
            }

            randomDialog = dialogeContinueCount;
            dialogeContinueCount += 1;
        }


        if (subtitlesActivated) { if (FindObjectOfType<Subtitles>()) subs = FindObjectOfType<Subtitles>(); }

        if (isWaiting && hasPriority && !continueDialog)
        {
            int randomInBetween = Random.Range(0, inBetweenNarrationClips.Length);

            if (subtitlesActivated) chainedDialog = dialog[randomDialog];
            chainedClip = clip[randomDialog];

            AudioManager.instance.PlayVoice(inBetweenNarrationClips[randomInBetween]);
            if (subtitlesActivated) subs.DisplayOnPlayingSubtitles(inBetweenNarrationDialog[randomInBetween], inBetweenNarrationClips[randomInBetween].length);
            Invoker.InvokeDelayed(ChainedDialog, inBetweenNarrationClips[randomInBetween].length);
        }
        else
        {
            int random = Random.Range(0, inBetweenNarrationClips.Length);

            AudioManager.instance.PlayVoice(clip[randomDialog]);

            if (subtitlesActivated)
            {
                if (subs == null)
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(dialog[randomDialog], clip[randomDialog].length);
                else
                    subs.DisplaySubtitles(dialog[randomDialog], clip[randomDialog].length);
            }
            
            isWaiting = true;


            if (continueDialog == true)
            {
                DelayND(clip[randomDialog].length, clip, dialog, largeDialoge, hasPriority, true);
            }
            else
            {
                Invoker.InvokeDelayed(ResetWait, clip[randomDialog].length);
                dialogeContinueCount = 0;
            }
           
        }
    }
    async void DelayND(float delay, AudioClip[] clip, string[] dialog, bool[] largeDialoge, bool hasPriority = false, bool hasComeFromClip = false)
    {
        await Task.Delay((int)(delay * 1000));
        NextDialoge(clip, dialog, largeDialoge, hasPriority, hasComeFromClip);
    }
    private void NextDialoge(AudioClip[] clip, string[] dialog, bool[] largeDialoge, bool hasPriority = false, bool hasComeFromClip = false)
    {
        PlaySubs(clip, dialog, largeDialoge, hasPriority, hasComeFromClip);
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
    }

    public void OnPlayerNotify(AllPlayerActions actions)
    {
        /*if (isWaiting)
            return;*/

        
        switch (actions)
        {
            case AllPlayerActions.Start:
                Invoke("PlayStartAudio", 0.1f);
                break;
            case AllPlayerActions.End:
                Debug.Log("Game Ended");
                break;
            case AllPlayerActions.LowHealth: //
                if (FindObjectOfType<PlayerControllerOverhaul>().GetHealth() < lowHealthThreshold && !lowHealthTriggered)
                {
                    lowHealthTriggered = true;
                    PlaySubs(playerLowHealth, playerLowHealthDialog,playerLowHealthBool);
                    Debug.Log("Low Health");
                }
                break;
            case AllPlayerActions.LowStamina: //
                if (FindObjectOfType<PlayerControllerOverhaul>().GetStamina() < lowStaminaThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    PlaySubs(playerLowStamina, playerLowStaminaDialog,playerLowStaminaBool);
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
                PlaySubs(randomPausedNoise, randomPausedNoiseDialog, randomPausedNoiseBool);
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
                PlaySubs(playerDies, playerDiesDialog,playerDiesBool, true);
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

    private void PlayStartAudio()
    {
        PlaySubs(startGame, startGameDialog,startGameBool, true);
        Debug.Log("Game Started");
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

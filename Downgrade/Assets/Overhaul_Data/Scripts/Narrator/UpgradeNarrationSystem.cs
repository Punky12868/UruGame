using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class UpgradeNarrationSystem : MonoBehaviour, IObserver
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


    int chainedDialoguesIndex;

    public Dialogue startGame, endGame, playerLowHealth, playerHeal, playerLowStamina, playerHitEnemy, playerKilledEnemy, playerNotKilling,
                    randomNoise, randomPausedNoise, startBoss, playerDieToFirstFaseBoss, midBoss, playerDieToMidFaceBoss, parryBoss, endBoss,
                    playerAttack, playerParry, playerDodge, playerUseItem, playerUseEmptyItem, playerPickUpItem, playerDropItem, playerDropEmptyItem, playerGotHit, playerDies, victory, defeat, inBetweenNarrationClips;
    
    private AudioClip chainedClip;
    private string chainedDialog;



    public void PlaySubs(Dialogue dialogueType, bool hasPriority = false, int fixedDialogue = 0)
    {
        Subtitles subs = null;
        bool isFixed = false;

        if (fixedDialogue != 0)
        {
            isFixed = true;
        }
        
        int dialogueSelected = Random.Range(0, dialogueType.allDialogues.Length);
        
        if (subtitlesActivated) { if (FindObjectOfType<Subtitles>()) subs = FindObjectOfType<Subtitles>(); }
        
        if (isFixed)
        {
            dialogueSelected = fixedDialogue -1 ;
        }

        if (isWaiting && hasPriority)
        {
            int randomInBetween = Random.Range(0, inBetweenNarrationClips.allDialogues.Length);
            if (dialogueType.allDialogues[dialogueSelected].HasChain) { randomInBetween = startDialogChainIndex; }

            if (subtitlesActivated) chainedDialog = dialogueType.allDialogues[dialogueSelected].Dialog;
            chainedClip = dialogueType.allDialogues[dialogueSelected].audioClip;

            AudioManager.instance.PlayVoice(dialogueType.allDialogues[randomInBetween].audioClip);
            if (subtitlesActivated) subs.DisplayOnPlayingSubtitles(dialogueType.allDialogues[dialogueSelected].Dialog, dialogueType.allDialogues[dialogueSelected].audioClip.length);
            Invoker.InvokeDelayed(ChainedDialog, dialogueType.allDialogues[dialogueSelected].audioClip.length);
        }
        else
        {
            AudioManager.instance.PlayVoice(dialogueType.allDialogues[dialogueSelected].audioClip);

            if (subtitlesActivated)
            {
                if (subs == null)
                    Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(dialogueType.allDialogues[dialogueSelected].Dialog, dialogueType.allDialogues[dialogueSelected].audioClip.length);
                else
                    subs.DisplaySubtitles(dialogueType.allDialogues[dialogueSelected].Dialog, dialogueType.allDialogues[dialogueSelected].audioClip.length);
            }

            if (dialogueType.allDialogues[dialogueSelected].HasChain)
            {
                chainedDialoguesIndex = 0;
                DelayND(dialogueType.allDialogues[dialogueSelected].audioClip.length, dialogueType.allDialogues[dialogueSelected].chainedDialogues);
            }
            else
            {
                isWaiting = true;
                Invoker.InvokeDelayed(ResetWait, dialogueType.allDialogues[dialogueSelected].audioClip.length);
            }
        }
    }

    async void DelayND(float delay, ChainedDialogues[] chainedDialogues)
    {
        await Task.Delay((int)((delay+ aditionalTimeAfterNarration )* 1000));
        NextDialoge(chainedDialogues);
    }
    private void NextDialoge(ChainedDialogues[] chainedDialogues)
    {
        Subtitles subs = null;
        AudioManager.instance.PlayVoice(chainedDialogues[chainedDialoguesIndex].audioClip);
        if (subtitlesActivated)
        {
            if (subs == null)
                Instantiate(subtitles, subtitlesParent).GetComponentInChildren<Subtitles>().DisplaySubtitles(chainedDialogues[chainedDialoguesIndex].Dialog, chainedDialogues[chainedDialoguesIndex].audioClip.length);
            else
                subs.DisplaySubtitles(chainedDialogues[chainedDialoguesIndex].Dialog, chainedDialogues[chainedDialoguesIndex].audioClip.length);
        }
        chainedDialoguesIndex++;

        if (chainedDialogues.Length > chainedDialoguesIndex)
        {
            DelayND(chainedDialogues[chainedDialoguesIndex].audioClip.length, chainedDialogues);
        }
        else
        {
            isWaiting = true;
            Invoker.InvokeDelayed(ResetWait, chainedDialogues[chainedDialoguesIndex-1].audioClip.length);
        }



        //PlaySubs(clip, dialog, largeDialoge, hasPriority, hasComeFromClip);
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



    private void DelayedDialog()
    {
        PlaySubs(startGame);
 
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
                    PlaySubs(playerLowHealth);
                    Debug.Log("Low Health");
                }
                break;
            case AllPlayerActions.LowStamina: //
                if (FindObjectOfType<PlayerControllerOverhaul>().GetStamina() < lowStaminaThreshold && !lowStaminaTriggered)
                {
                    lowStaminaTriggered = true;
                    PlaySubs(playerLowStamina);
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
                PlaySubs(randomPausedNoise);
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
                PlaySubs(playerDies, true);
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


    #region Classes
    [System.Serializable]
    public class Dialogue
    {
        //public DialogueType type;
        public InitialDialogue[] allDialogues;
    }
    [System.Serializable]
    public class InitialDialogue
    {

        public AudioClip audioClip;
        [TextArea(5, 5)] public string Dialog;
        public bool HasChain;
        public ChainedDialogues[] chainedDialogues;


    }

    [System.Serializable]
    public class ChainedDialogues
    {
        [TextArea(5, 5)] public string Dialog;
        public AudioClip audioClip;
    }


    #endregion

}

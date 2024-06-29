using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using EasyTransition;
using Cinemachine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isSelectingDowngrade = false;
    private bool introTextShowed = false;
    private int downgradeSceneIndex;
    private bool displayFps = false;
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool vSync = true;
    [SerializeField] private string levelUnlockerKey = "level_";
    [SerializeField] private int firstLevelIndex = 1;
    [SerializeField] private int firstLevelDowngradeSelectionIndex = 2;
    [SerializeField] private int maxLevelPantano = 8;
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float transitionDelay;
    private bool forcedPause;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        displayFps = false;
        downgradeSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        QualitySettings.vSyncCount = vSync ? 1 : 0;
        if (!vSync) { Application.targetFrameRate = targetFrameRate; }

        
    }

    private void Update() { FrameRate(); CursorHandler(); }

    private void FrameRate()
    {
        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl)) displayFps = !displayFps;
        if (vSync && QualitySettings.vSyncCount == 0) { QualitySettings.vSyncCount = 1; return; }

        if (!vSync)
        {
            if (QualitySettings.vSyncCount == 1) { QualitySettings.vSyncCount = 0; }
            if (Application.targetFrameRate != targetFrameRate) { Application.targetFrameRate = targetFrameRate; }
        }
    }

    private void CursorHandler()
    {
        if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        if (Cursor.visible) Cursor.visible = false;
    }

    public void StartNewGame()
    {
        if (!EraseProgressOnNewGame())
        {
            TransitionManager.Instance().Transition(firstLevelIndex, transitionSettings, transitionDelay);
            Debug.Log("New Game Started");
        }
        else
        {
            FindObjectOfType<GameManagerProxy>().NewGame();
        }

        if(introTextShowed == true) SetIntroText(false);
    }

    public void GoToDowngrade()
    {
        if (firstLevelDowngradeSelectionIndex == SceneManager.GetActiveScene().buildIndex + 1)
        {
            TransitionManager.Instance().Transition(downgradeSceneIndex, transitionSettings, transitionDelay);
            Debug.Log("Downgrade Started");
        }
        if (introTextShowed == true) SetIntroText(false);
    }

    public void NewGameErasedProgress()
    {
        if (FindObjectOfType<LevelUnlocker>()) FindObjectOfType<LevelUnlocker>().LockAllLevels();
        TransitionManager.Instance().Transition(firstLevelIndex, transitionSettings, transitionDelay);
        Debug.Log("Erased Game Started");
        if (introTextShowed == true) SetIntroText(false);
    }

    private bool EraseProgressOnNewGame()
    {
        return SimpleSaveLoad.Instance.LoadData<bool>(FileType.Gameplay, levelUnlockerKey + firstLevelIndex, false);
    }

    public void BossDefeated()
    {
        FindObjectOfType<TextScreens>().OnVictory();
        TransitionManager.Instance().Transition(0, transitionSettings, transitionDelay + 0.5f);
        if (introTextShowed == true) SetIntroText(false);
    }

    public void LoadScene(int id)
    {
        // Load scnene by id
        SceneManager.LoadScene(id);
        if (introTextShowed == true) SetIntroText(false);
    }

    public void RestartGame()
    {
        // Restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }

    public void LoadNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        // Load the previous scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void UnlockLevel(int i)
    {
        SimpleSaveLoad.Instance.SaveData<bool>(FileType.Gameplay, levelUnlockerKey + i, true);
        // victory and then loads next scene
    }

    public void Victory()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        FindObjectOfType<TextScreens>().OnVictory();
        SimpleSaveLoad.Instance.SaveData<bool>(FileType.Gameplay, levelUnlockerKey + SceneManager.GetActiveScene().buildIndex, true);

        if (sceneIndex == firstLevelDowngradeSelectionIndex)
        {
            GoToDowngrade();
        }
        else
        {
            TransitionManager.Instance().Transition(sceneIndex, transitionSettings, transitionDelay);
        }
        if (introTextShowed == true) SetIntroText(false);
    }

    public delegate void OnPauseGame();
    public static event OnPauseGame onPauseGame;

    
    public void PauseGame(bool pause, bool downgrade = false)
    {
        isSelectingDowngrade = downgrade;

        if (pause)
        {
            Time.timeScale = 0;
            onPauseGame?.Invoke();
        }
        else
        {
            Time.timeScale = 1;
            onPauseGame?.Invoke();
        }
    }

    public void ForcePauseGame(bool forced = true)
    {
        forcedPause = forced;
    }

    public bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    public bool IsSelectingDowngrade()
    {
        return isSelectingDowngrade;
    }

    public bool IsForcedPause() 
    {
        return forcedPause; 
    }
    public void ResetArenaPantano()
    {
        for (int i = 0; i < maxLevelPantano; i++)
        {
            if (i <= 1) continue;
            SimpleSaveLoad.Instance.SaveData(FileType.Gameplay, levelUnlockerKey + i, false);
        }

        Time.timeScale = 1;
        TransitionManager.Instance().Transition(downgradeSceneIndex, transitionSettings, transitionDelay);
        if (introTextShowed == true) SetIntroText(false);
    }

    private void OnGUI()
    {
        if (displayFps)
        {
            GUI.color = Color.green;
            GUI.skin.label.fontSize = 20;
            GUI.Label(new Rect(10, 10, 200, 40), "FPS: " + (1.0f / Time.deltaTime).ToString("0"));
        }
    }

    public void SetIntroText(bool value)
    {
        introTextShowed = value;
    }

    public bool GetIntroText()
    {
        return introTextShowed;
    }

    public void CameraShake(float duration, float magnitude, float gain, bool fadeOnStop = false)
    {
        // cinemachine camera shake
        //DOTween.To(() amplitude from 0 to magnitude, frequency from 0 to gain, duration)
        DOTween.To(() => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, x 
            => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x, magnitude, duration / 2);

        DOTween.To(() => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain, x 
            => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x, gain, duration / 2);

        //FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = magnitude;
        //FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = gain;
        if (fadeOnStop) { Invoke("StopShakeWFade", duration); return; }
        Invoke("StopShake", duration);
    }

    private void StopShake()
    {
        FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
    }

    private void StopShakeWFade()
    {
        DOTween.To(() => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, x
            => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x, 0, 1.5f);

        DOTween.To(() => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain, x
            => FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x, 0, 1.5f);

        //FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        //FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
    }
}

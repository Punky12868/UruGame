using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using EasyTransition;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isSelectingDowngrade = false;
    [SerializeField] private string levelUnlockerKey = "level_";
    [SerializeField] private int firstLevelIndex = 1;
    [SerializeField] private int downgradeSceneIndex = 11;
    [SerializeField] private int firstLevelDowngradeSelectionIndex = 2;
    [SerializeField] private int maxLevelPantano = 8;
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float transitionDelay;

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

        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
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
    }

    public void GoToDowngrade()
    {
        if (firstLevelDowngradeSelectionIndex == SceneManager.GetActiveScene().buildIndex + 1)
        {
            TransitionManager.Instance().Transition(downgradeSceneIndex, transitionSettings, transitionDelay);
            Debug.Log("Downgrade Started");
        }
    }

    public void NewGameErasedProgress()
    {
        if (FindObjectOfType<LevelUnlocker>()) FindObjectOfType<LevelUnlocker>().LockAllLevels();
        TransitionManager.Instance().Transition(firstLevelIndex, transitionSettings, transitionDelay);
        Debug.Log("Erased Game Started");
    }

    private bool EraseProgressOnNewGame()
    {
        return SimpleSaveLoad.Instance.LoadData<bool>(FileType.Gameplay, levelUnlockerKey + firstLevelIndex, false);
    }

    public void BossDefeated()
    {
        FindObjectOfType<TextScreens>().OnVictory();
        TransitionManager.Instance().Transition(0, transitionSettings, transitionDelay + 0.5f);
    }

    public void LoadScene(int id)
    {
        // Restart the game
        SceneManager.LoadScene(id);
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

    public bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    public bool IsSelectingDowngrade()
    {
        return isSelectingDowngrade;
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

    }
}

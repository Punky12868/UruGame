using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isSelectingDowngrade = false;
    [SerializeField] private string levelUnlockerKey = "level_";

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
        FindObjectOfType<TextScreens>().OnVictory();
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
}

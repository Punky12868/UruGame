using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerProxy : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //GameManager.Instance.PauseGame(false);
    }

    public void LoadScene(int id)
    {
        GameManager.Instance.LoadScene(id);
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void LoadNextScene()
    {
        GameManager.Instance.LoadNextScene();
    }

    public void LoadPreviousScene()
    {
        GameManager.Instance.LoadPreviousScene();
    }

    public void Victory()
    {
        GameManager.Instance.Victory();
    }

    public void PauseGame(bool pause)
    {
        GameManager.Instance.PauseGame(pause);
    }

    public void PauseGame(bool pause, bool downgrade)
    {
        GameManager.Instance.PauseGame(pause, downgrade);
    }

    public bool IsGamePaused()
    {
        return GameManager.Instance.IsGamePaused();
    }

    public bool IsSelectingDowngrade()
    {
        return GameManager.Instance.IsSelectingDowngrade();
    }
}

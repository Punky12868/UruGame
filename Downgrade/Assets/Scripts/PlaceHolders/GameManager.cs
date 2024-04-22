using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public void Victory()
    {
        FindObjectOfType<DeathScreen>().OnVictory();
    }
}

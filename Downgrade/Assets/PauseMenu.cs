using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject pauseMenu;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (player.GetButtonDown("Pause"))
        {
            GameManager.Instance.PauseGame(!GameManager.Instance.IsGamePaused());
        }

        pauseMenu.SetActive(GameManager.Instance.IsGamePaused());
    }
}

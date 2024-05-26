using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject configMenu;
    [SerializeField] Button onPauseSelect;
    [SerializeField] Button optionsButton;
    [SerializeField] bool onConfig;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (player.GetButtonDown("Pause") && !GameManager.Instance.IsSelectingDowngrade())
        {
            if (onConfig) { onConfig = false; configMenu.SetActive(onConfig); optionsButton.Select(); return; }
            GameManager.Instance.PauseGame(!GameManager.Instance.IsGamePaused());
        }

        if (!GameManager.Instance.IsSelectingDowngrade())
        {
            pauseMenu.SetActive(GameManager.Instance.IsGamePaused());
        }

        if (GameManager.Instance.IsGamePaused() && !FindObjectOfType<SetPlayerMap>().GetLoadUiMap())
        {
            FindObjectOfType<SetPlayerMap>().SetLoadUiMap(GameManager.Instance.IsGamePaused());
            onPauseSelect.Select();
        }
        else if (!GameManager.Instance.IsGamePaused() && FindObjectOfType<SetPlayerMap>().GetLoadUiMap())
        {
            FindObjectOfType<SetPlayerMap>().SetLoadUiMap(GameManager.Instance.IsGamePaused());
        }
    }

    public void SetOnConfig(bool value) { onConfig = value; }
}

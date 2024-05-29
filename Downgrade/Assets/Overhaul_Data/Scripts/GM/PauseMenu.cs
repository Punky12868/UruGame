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
    [SerializeField] Button subsButton;
    [SerializeField] bool onConfig;
    [SerializeField] bool onSlider;
    private Button sliderButtonSelected;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (player.GetButtonDown("Pause") && !GameManager.Instance.IsSelectingDowngrade())
        {
            if (onSlider) { onSlider = false; sliderButtonSelected.Select(); subsButton.interactable = true; return; }
            if (onConfig) { onConfig = false; pauseMenu.SetActive(!onConfig); configMenu.SetActive(onConfig); optionsButton.Select(); return; }
            GameManager.Instance.PauseGame(!GameManager.Instance.IsGamePaused());
        }

        if (player.GetButtonDown("Submit") && !GameManager.Instance.IsSelectingDowngrade() && onSlider && onConfig)
        {
            onSlider = false; sliderButtonSelected.Select(); subsButton.interactable = true; return;
        }

        if (!GameManager.Instance.IsSelectingDowngrade())
        {
            if (onConfig) return;
            if (onSlider) return;
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
    public bool GetOnConfig() { return onConfig; }
    public void SetOnSlider(bool value) {  onSlider = value; }
    public bool GetOnSlider() { return onSlider; }
    public void SetSliderButtonSelected(Button sliderButton) { sliderButtonSelected = sliderButton; }
}

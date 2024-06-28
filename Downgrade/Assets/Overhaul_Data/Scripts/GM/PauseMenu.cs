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
    [SerializeField] GameObject controllerMenu;
    [SerializeField] Button onPauseSelect;
    [SerializeField] Button optionsButton;
    [SerializeField] Button subsButton;
    [SerializeField] Button controllerButton;
    [SerializeField] bool onConfig;
    [SerializeField] bool onSlider;
    [SerializeField] bool onController;
    private Button sliderButtonSelected;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (player.GetButtonDown("Pause") && !GameManager.Instance.IsSelectingDowngrade())
        {
            if (onController)
            {
                onController = false;
                pauseMenu.SetActive(!onController);
                controllerMenu.SetActive(onController);
                controllerButton.Select();
                //Debug.Log("a");
                return;
            }
            if (onSlider) 
            { 
                onSlider = false; 
                sliderButtonSelected.Select(); 
                if (subsButton != null) subsButton.interactable = true;
                return; 
            }
            if (onConfig) 
            { 
                onConfig = false; 
                pauseMenu.SetActive(!onConfig); 
                configMenu.SetActive(onConfig); 
                optionsButton.Select(); 
                return; 
            }
            GameManager.Instance.PauseGame(!GameManager.Instance.IsGamePaused());
        }

        if (player.GetButtonDown("Cancel") && !GameManager.Instance.IsSelectingDowngrade() && onSlider && onConfig)
        {
            onSlider = false; 
            sliderButtonSelected.Select(); 
            if (subsButton != null) subsButton.interactable = true; 
            if (controllerButton != null) controllerButton.interactable = true;
            Debug.Log("D");
            return;
        }

        if (player.GetButtonDown("Cancel") && !GameManager.Instance.IsSelectingDowngrade() && onController)
        {
            onController = false;
            controllerMenu.SetActive(false);
            controllerButton.Select();
            return;
        }

        if (!GameManager.Instance.IsSelectingDowngrade())
        {
            if (onConfig) return;
            if (onController) return;
            if (onSlider) return;
            pauseMenu.SetActive(GameManager.Instance.IsGamePaused());
        }

        if (GameManager.Instance.IsGamePaused() && !FindObjectOfType<SetPlayerMap>().GetLoadUiMap() )
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
    public void SetOnController(bool value) { onController = value; }
    public bool GetOnController() { return onController; }
    public void SetSliderButtonSelected(Button sliderButton) { sliderButtonSelected = sliderButton; }
}

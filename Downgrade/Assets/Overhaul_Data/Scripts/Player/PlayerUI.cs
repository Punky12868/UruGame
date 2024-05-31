using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerControllerOverhaul player;

    [SerializeField] private float updateSpeed;
    [SerializeField] private float timeBeforeUpdate;
    [SerializeField] private float lerpMargen;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthHitSlider;

    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider staminaUsedSlider;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Image dgIcon;

    bool alreadySet;

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = FindObjectOfType<PauseMenu>().gameObject.GetComponent<Canvas>().worldCamera;
    }

    public void SetUI()
    {
        if (alreadySet) return;
        player = FindObjectOfType<PlayerControllerOverhaul>();

        healthSlider.maxValue = player.GetHealth();
        healthSlider.value = player.GetHealth();
        healthHitSlider.maxValue = player.GetHealth();
        healthHitSlider.value = player.GetHealth();

        staminaSlider.maxValue = player.GetStamina();
        staminaSlider.value = player.GetStamina();
        staminaUsedSlider.maxValue = player.GetStamina();
        staminaUsedSlider.value = player.GetStamina();

        dgIcon.sprite = DowngradeSystem.Instance.GetIcon();
        alreadySet = true;
    }

    private void Update()
    {
        if (player == null) { player = FindObjectOfType<PlayerControllerOverhaul>(); }
        if (GameManager.Instance.IsGamePaused()) return;

        UpdateHealth();
        UpdateStamina();

        Hit();
        UseStamina();
    }

    public void SetItemIcon(Sprite icon) { itemIcon.sprite = icon; }
    private void UpdateHealth() {if (healthSlider.value != player.GetHealth()) healthSlider.value = player.GetHealth();}
    private void UpdateStamina() {if (staminaSlider.value != player.GetStamina()) staminaSlider.value = player.GetStamina();}
    private void Hit() {healthHitSlider.value = Mathf.Lerp(healthHitSlider.value, player.GetHealth(), Time.deltaTime * updateSpeed);}
    private void UseStamina() {staminaUsedSlider.value = Mathf.Lerp(staminaUsedSlider.value, player.GetStamina(), Time.deltaTime * updateSpeed);}
}

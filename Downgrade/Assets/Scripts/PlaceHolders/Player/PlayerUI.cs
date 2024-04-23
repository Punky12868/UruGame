using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerComponent player;

    [SerializeField] private float updateSpeed;
    [SerializeField] private float timeBeforeUpdate;
    [SerializeField] private float lerpMargen;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthHitSlider;

    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider staminaUsedSlider;

    public void SetUI()
    {
        player = GetComponent<PlayerComponent>();

        healthSlider.maxValue = player.GetCurrentHealth();
        healthSlider.value = player.GetCurrentHealth();
        healthHitSlider.maxValue = player.GetCurrentHealth();
        healthHitSlider.value = player.GetCurrentHealth();

        staminaSlider.maxValue = player.GetCurrentStamina();
        staminaSlider.value = player.GetCurrentStamina();
        staminaUsedSlider.maxValue = player.GetCurrentStamina();
        staminaUsedSlider.value = player.GetCurrentStamina();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        UpdateHealth();
        UpdateStamina();

        Hit();
        UseStamina();
    }

    private void UpdateHealth()
    {
        if (healthSlider.value != player.GetCurrentHealth())
        {
            healthSlider.value = player.GetCurrentHealth();
        }
    }

    private void UpdateStamina()
    {
        if (staminaSlider.value != player.GetCurrentStamina())
        {
            staminaSlider.value = player.GetCurrentStamina();
        }
    }

    private void Hit()
    {
        healthHitSlider.value = Mathf.Lerp(healthHitSlider.value, player.GetCurrentHealth(), Time.deltaTime * updateSpeed);
    }

    private void UseStamina()
    {
        staminaUsedSlider.value = Mathf.Lerp(staminaUsedSlider.value, player.GetCurrentStamina(), Time.deltaTime * updateSpeed);
    }
}

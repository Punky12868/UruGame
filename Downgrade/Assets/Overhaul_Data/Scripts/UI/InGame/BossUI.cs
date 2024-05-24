using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    private BossBase boss;
    private bool initialized;
    private bool faseChanged;
    [SerializeField] private float barAppearingTime;
    [SerializeField] private TMP_Text bossName;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBarBg;
    [SerializeField] private float healthBarRegenSpeed;
    [SerializeField] private float healthBarBgSpeed;

    public void SetUI(BossBase boss)
    {
        this.boss = boss;
        bossName.text = boss.GetBossName();
        healthBar.maxValue = boss.GetHealth();
        healthBarBg.maxValue = boss.GetHealth();

        healthBar.value = boss.GetHealth();
        healthBarBg.value = boss.GetHealth();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        UpdateHealthUI();

        if (!faseChanged) return;
        SetChangeFase();
    }

    private void UpdateHealthUI()
    {
        if (faseChanged) return;

        if (GetComponent<CanvasGroup>().alpha != 1)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime * barAppearingTime;
        }

        if (healthBar.value != boss.GetCurrentHealth()) healthBar.value = boss.GetCurrentHealth();
        healthBarBg.value = Mathf.Lerp(healthBarBg.value, boss.GetCurrentHealth(), Time.deltaTime * healthBarBgSpeed);
    }

    public void SetChangeFase()
    {
        if (!faseChanged)
        {
            faseChanged = true;
        }

        healthBar.maxValue = boss.GetHealth();
        healthBarBg.maxValue = boss.GetHealth();

        if (healthBar.value != boss.GetHealth())
        {
            healthBar.value += Time.deltaTime * healthBarRegenSpeed;
            healthBarBg.value = healthBar.value;
        }
        else
        {
            faseChanged = false;
        }
    }
}

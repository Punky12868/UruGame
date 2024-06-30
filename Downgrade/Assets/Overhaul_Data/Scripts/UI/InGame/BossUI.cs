using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Febucci.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    private EnemyBase enemyBase;
    private bool initialized;
    private bool faseChanged;
    private bool isChangingValues;
    [SerializeField] private float barAppearingTime;
    [SerializeField] private TMP_Text bossName;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBarBg;
    [SerializeField] private float healthBarRegenSpeed;
    [SerializeField] private float healthBarBgSpeed;

    public void SetUI(EnemyBase boss)
    {
        this.enemyBase = boss;

        if (!initialized)
        {
            bossName.text = ""; //boss.GetName();
            typewriter.ShowText(boss.GetName());
            healthBar.maxValue = boss.GetCurrentHealth();
            healthBarBg.maxValue = boss.GetCurrentHealth();

            healthBar.value = boss.GetCurrentHealth();
            healthBarBg.value = boss.GetCurrentHealth();
            initialized = true; return;
        }

        isChangingValues = true;
        //SetChangeFase();
        typewriter.onTextDisappeared.AddListener(() => { typewriter.ShowText(boss.GetName()); });
        typewriter.StartDisappearingText();
        DOTween.To(() => healthBar.value, x => healthBar.value = x, boss.GetCurrentHealth(), 2);
        DOTween.To(() => healthBarBg.value, x => healthBarBg.value = x, boss.GetCurrentHealth(), 2).onComplete += () => isChangingValues = false;
    }

    private void Update()
    {
        if (!initialized || isChangingValues) return;
        UpdateHealthUI();


        /*if (!faseChanged) return;
        SetChangeFase();*/
    }

    private void UpdateHealthUI()
    {
        //if (faseChanged) return;

        if (GetComponent<CanvasGroup>().alpha != 1)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime * barAppearingTime;
        }

        if (healthBar.value != enemyBase.GetCurrentHealth()) healthBar.value = enemyBase.GetCurrentHealth();
        healthBarBg.value = Mathf.Lerp(healthBarBg.value, enemyBase.GetCurrentHealth(), Time.deltaTime * healthBarBgSpeed);
    }

    public void SetChangeFase()
    {
        if (!faseChanged)
        {
            faseChanged = true;
        }

        healthBar.maxValue = enemyBase.GetCurrentHealth();
        healthBarBg.maxValue = enemyBase.GetCurrentHealth();

        if (healthBar.value != enemyBase.GetCurrentHealth())
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

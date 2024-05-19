using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DowngradeSystem : MonoBehaviour, IObserver
{
    public static DowngradeSystem Instance { get; private set; }

    SelectedDowngrade dg;
    Sprite dgIcon;
    Subject player;
    List<Subject> enemies = new List<Subject>();

    //[Header("Stamina")]
    private float staminaLossAmmount;

    //[Header("Slime")]
    private float fatrollSpeedAmmount;
    private float fatrollTime;
    private float stored_PlayerSpeed;

    //[Header("Rodillas")]
    private float asthmaStaminaThresshold;
    private float asthmaHealthLossPercentage;

    //[Header("Paralisis")]
    private float damageAmmount;
    private float paralysisTime;

    //[Header("Moneda")]
    private float badLuckHealthLossAmmount;

    //[Header("Esqueleto")]
    private int enemyBoostDamageAmmount;
    private float enemyBoostTimeThresshold;
    private float enemyBoostTime;
    private bool isNotKilling = false;
    private bool enemySpawned = false;

    //[Header("Debil")]
    private Vector2 weaknessDamageAmmount;
    private Vector2 stored_PlayerDamage;
    private float weaknessCooldownTime;
    private float weaknessTime;

    //[Header("Daga")]
    private bool onSwitch = true;

    //[Header("Dados")]
    private int diceRolls;

    #region Values
    public void SetPlayer(Subject player)
    {
        this.player = player;
        player.AddObserver(this);

        SetPLayerStoredStats();
    }

    private void SetPLayerStoredStats()
    {
        stored_PlayerSpeed = player.GetComponent<PlayerComponent>().GetSpeed();
        stored_PlayerDamage = player.GetComponent<PlayerComponent>().GetDamage();
    }

    public void SetEnemy(Subject enemy)
    {
        this.enemies.Add(enemy);
        enemy.AddObserver(this);
    }

    public void RemoveEnemy(Subject enemy)
    {
        enemies.Remove(enemy);
        enemy.RemoveObserver(this);
    }

    public void SetDowngrade(SelectedDowngrade dg)
    {
        this.dg = dg;
    }

    public void RemoveDowngrade()
    {
        dg = SelectedDowngrade.None;
    }

    public Sprite GetIcon()
    {
        return dgIcon;
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.RemoveObserver(this);
        }
    }
    #endregion

    #region Unity Methods
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

        DontDestroyOnLoad(gameObject);

        RemoveDowngrade();
    }

    private void Update()
    {
        if (isNotKilling && enemySpawned)
        {
            enemyBoostTime += Time.deltaTime;

            if (enemyBoostTime >= enemyBoostTimeThresshold)
            {
                foreach (Subject enemy in enemies)
                {
                    enemy.GetComponent<EnemyBase>().normalAttackdamage += enemyBoostDamageAmmount;
                }
                enemyBoostTime = 0;
                Debug.Log("EnemyBoost");
            }
        }

        if (dg == SelectedDowngrade.Debil)
        {
            if (weaknessTime < weaknessCooldownTime)
            {
                weaknessTime += Time.deltaTime;
            }
            else
            {
                FindObjectOfType<PlayerComponent>().SetDamage(stored_PlayerDamage);
            }
        }
    }
    #endregion

    #region Notify
    public void OnPlayerNotify(AllPlayerActions actions)
    {
        switch (dg)
        {
            case SelectedDowngrade.Stamina:
                if (actions == AllPlayerActions.useAbility)
                {
                    FindObjectOfType<PlayerComponent>().UseStamina(staminaLossAmmount);
                    Debug.Log("StaminaUsed");
                }
                break;
            case SelectedDowngrade.Slime:
                if (actions == AllPlayerActions.Dodge)
                {
                    FindObjectOfType<PlayerComponent>().SetSpeed(fatrollSpeedAmmount);
                    Invoke("ResetFatRoll", fatrollTime);
                    Debug.Log("Fatroll");
                }
                break;
            case SelectedDowngrade.Rodilla:
                if (actions == AllPlayerActions.LowStamina)
                {
                    if (FindObjectOfType<PlayerComponent>().GetStamina() <= asthmaStaminaThresshold)
                    {
                        FindObjectOfType<PlayerComponent>().TakeDamage(asthmaHealthLossPercentage);
                        Debug.Log("Asthma");
                    }
                }
                break;
            case SelectedDowngrade.Paralisis:
                if (actions == AllPlayerActions.SuccesfullParry)
                {
                    FindObjectOfType<PlayerComponent>().SetParalisisStatus(true, paralysisTime);

                    EnemyBase[] enemyBases = FindObjectsOfType<EnemyBase>();
                    foreach (EnemyBase enemy in enemyBases)
                    {
                        if (enemy.isParried) enemy.TakeDamage(damageAmmount);
                    }

                    Debug.Log("Paralysis");
                }
                break;
            case SelectedDowngrade.Moneda:
                if (actions == AllPlayerActions.Start)
                {
                    FindObjectOfType<PlayerComponent>().SetMoneda(true);
                    Debug.Log("Moneda Active");
                }

                if (actions == AllPlayerActions.useItem)
                {
                    // 50% chance to take damage
                    if (Random.Range(0, 2) == 0)
                    {
                        FindObjectOfType<PlayerComponent>().TakeDamage(badLuckHealthLossAmmount);
                        FindObjectOfType<PlayerComponent>().MonedaUseItem(false);
                        Debug.Log("BadLuck");
                    }
                    else
                    {
                        FindObjectOfType<PlayerComponent>().MonedaUseItem(true);
                    }
                }
                break;
            case SelectedDowngrade.Esqueleto:
                if (actions == AllPlayerActions.KilledEnemy)
                {
                    isNotKilling = false;
                    enemyBoostTime = 0;
                }

                if (actions == AllPlayerActions.NotKilling)
                {
                    isNotKilling = true;
                }
                break;
            case SelectedDowngrade.Debil:
                if (actions == AllPlayerActions.KilledEnemy)
                {
                    FindObjectOfType<PlayerComponent>().SetDamage(weaknessDamageAmmount);
                    weaknessTime = 0;
                    Debug.Log("Weakness");
                }
                break;
            case SelectedDowngrade.Daga:
                if (actions == AllPlayerActions.Dodge)
                {
                    onSwitch = !onSwitch;
                    FindObjectOfType<PlayerComponent>().SetCanAttack(onSwitch);
                    Debug.Log("Attack is: " + onSwitch);
                }
                break;
            case SelectedDowngrade.Dados:
                if (actions == AllPlayerActions.Start)
                {
                    FindObjectOfType<PlayerComponent>().SetRollQuantity(true, diceRolls);
                    Debug.Log("DiceRolls");
                }
                break;

        }
    }

    public void OnEnemyNotify(AllEnemyActions actions)
    {
        switch (dg)
        {
            case SelectedDowngrade.Esqueleto:
                if (actions == AllEnemyActions.Spawned && !enemySpawned)
                {
                    enemySpawned = true;
                }
                break;
        }
    }

    public void OnBossesNotify(AllBossActions actions)
    {
        switch (dg)
        {
            /*case SelectedDowngrade.EnemyBoost:
                if (actions == AllBossActions.Start && !enemySpawned)
                {
                    enemySpawned = true;
                }
                break;*/
        }
    }
    #endregion

    #region Set Downgrade Values

    public void SetStaminaDg(float time, Sprite icon)
    {
        staminaLossAmmount = time;
        dgIcon = icon;
    }

    public void SetSlimeDg(float speed, float cooldown, Sprite icon)
    {
        fatrollSpeedAmmount = speed;
        fatrollTime = cooldown;
        dgIcon = icon;
    }

    public void SetFatRollStoredSpeed(float storedSpeed)
    {
        stored_PlayerSpeed = storedSpeed;
    }

    public void SetRodillasDg(float stamina, float healthLoss, Sprite icon)
    {
        asthmaStaminaThresshold = stamina;
        asthmaHealthLossPercentage = healthLoss;
        dgIcon = icon;
    }

    public void SetParalisisDg(float damage, float time, Sprite icon)
    {
        damageAmmount = damage;
        paralysisTime = time;
        dgIcon = icon;
    }

    public void SetMonedaDg(float healthLoss, Sprite icon)
    {
        badLuckHealthLossAmmount = healthLoss;
        dgIcon = icon;
    }

    public void SetEsqueletoDg(int damage, float cooldown, Sprite icon)
    {
        enemyBoostDamageAmmount = damage;
        enemyBoostTimeThresshold = cooldown;
        dgIcon = icon;
    }

    public void SetDebilDg(Vector2 damage, float cooldown, Sprite icon)
    {
        weaknessDamageAmmount = damage;
        weaknessCooldownTime = cooldown;
        dgIcon = icon;
    }

    public void SetDagaDg(Sprite icon)
    {
        dgIcon = icon;
    }

    public void SetDadosDg(int rolls, Sprite icon)
    {
        diceRolls = rolls;
        dgIcon = icon;
    }
    #endregion

    #region Cooldown Invoke Values
    public void ResetFatRoll()
    {
        FindObjectOfType<PlayerComponent>().SetSpeed(stored_PlayerSpeed);
    }
    #endregion
}

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

    //[Header("Fatroll")]
    private float fatrollSpeedAmmount;
    private float fatrollTime;
    private float stored_PlayerSpeed;

    //[Header("Asthma")]
    private float asthmaStaminaThresshold;
    private float asthmaHealthLossPercentage;

    //[Header("BadLuck")]
    private float badLuckHealthLossAmmount;

    //[Header("Weakness")]
    private Vector2 weaknessDamageAmmount;
    private Vector2 stored_PlayerDamage;
    private float weaknessCooldownTime;
    private float weaknessTime;

    //[Header("EnemyBoost")]
    private int enemyBoostDamageAmmount;
    private float enemyBoostTimeThresshold;
    private float enemyBoostTime;
    private bool isNotKilling = false;
    private bool enemySpawned = false;

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
            case SelectedDowngrade.Moneda:
                if (actions == AllPlayerActions.useItem)
                {
                    // 50% chance to take damage
                    if (Random.Range(0, 2) == 0)
                    {
                        FindObjectOfType<PlayerComponent>().TakeDamage(badLuckHealthLossAmmount);
                        Debug.Log("BadLuck");
                    }
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
    public void SetFatRollDg(float speed, float cooldown, Sprite icon)
    {
        fatrollSpeedAmmount = speed;
        fatrollTime = cooldown;
        dgIcon = icon;
    }

    public void SetFatRollStoredSpeed(float storedSpeed)
    {
        stored_PlayerSpeed = storedSpeed;
    }

    public void SetAsthmaDg(float stamina, float healthLoss, Sprite icon)
    {
        asthmaStaminaThresshold = stamina;
        asthmaHealthLossPercentage = healthLoss;
        dgIcon = icon;
    }

    public void SetBadLuckDg(float healthLoss, Sprite icon)
    {
        badLuckHealthLossAmmount = healthLoss;
        dgIcon = icon;
    }

    public void SetWeaknessDg(Vector2 damage, float cooldown, Sprite icon)
    {
        weaknessDamageAmmount = damage;
        weaknessCooldownTime = cooldown;
        dgIcon = icon;
    }

    public void SetEnemyBoostDg(int damage, float cooldown, Sprite icon)
    {
        enemyBoostDamageAmmount = damage;
        enemyBoostTimeThresshold = cooldown;
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

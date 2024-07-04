using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DowngradeSystem : MonoBehaviour, IObserver
{
    public static DowngradeSystem Instance { get; private set; }

    SelectedDowngrade dg;
    Sprite dgIcon;
    [SerializeField] private Sprite dgNoIcon;
    Subject player;
    List<Subject> enemies = new List<Subject>();
    private FeedbackDG feedbackRef;

    [SerializeField] private Dados dadosDg;
    [SerializeField] private Daga dagaDg;
    [SerializeField] private Debil debilDg;
    [SerializeField] private Esqueleto esqueletoDg;
    [SerializeField] private Moneda monedaDg;
    [SerializeField] private Paralisis paralisisDg;
    [SerializeField] private Rodillas rodillasDg;
    [SerializeField] private Slime slimeDg;
    [SerializeField] private StaminaDg StaminaDg;

    //[Header("Stamina")]
    private float staminaLossAmmount;

    //[Header("Slime")]
    private float fatrollSpeedAmmount;
    private float fatrollTime;
    private float fatrollTimeLeft;
    private float stored_PlayerSpeed;
    private bool runTimerSlime;

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
        stored_PlayerSpeed = player.GetComponent<PlayerControllerOverhaul>().GetSpeed();
        stored_PlayerDamage = player.GetComponent<PlayerControllerOverhaul>().GetDamage();
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

    private void LoadDg()
    {
        dg = SimpleSaveLoad.Instance.LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade", SelectedDowngrade.None);

        switch (dg)
        {
            case SelectedDowngrade.None:
                break;
            case SelectedDowngrade.Debil:
                debilDg.CardEffect();
                break;
            case SelectedDowngrade.Stamina:
                StaminaDg.CardEffect();
                break;
            case SelectedDowngrade.Esqueleto:
                esqueletoDg.CardEffect();
                break;
            case SelectedDowngrade.Daga:
                dagaDg.CardEffect();
                break;
            case SelectedDowngrade.Dados:
                dadosDg.CardEffect();
                break;
            case SelectedDowngrade.Paralisis:
                paralisisDg.CardEffect();
                break;
            case SelectedDowngrade.Slime:
                slimeDg.CardEffect();
                break;
            case SelectedDowngrade.Rodilla:
                rodillasDg.CardEffect();
                break;
            case SelectedDowngrade.Moneda:
                monedaDg.CardEffect();
                break;
            default:
                break;
        }
    }

    public void SetDowngrade(SelectedDowngrade dg)
    {
        this.dg = dg;
        SimpleSaveLoad.Instance.SaveData(FileType.Gameplay, "Downgrade", dg);
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

        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }

    private void DelayedAwake()
    {
        LoadDg();
        /*if (FindObjectOfType<FeedbackDG>())
        {
            feedbackRef = FindObjectOfType<FeedbackDG>();
            feedbackRef.downgrade = dg;
        }*/
       
        //RemoveDowngrade();
    }

    public void SetDowngradeFeedback(FeedbackDG script)
    {
        feedbackRef = script;
        feedbackRef.downgrade = dg;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 2) return;

        if (isNotKilling && enemySpawned)
        {
            enemyBoostTime += Time.deltaTime;

            if (enemyBoostTime >= enemyBoostTimeThresshold)
            {
                foreach (Subject enemy in enemies)
                {
                    //enemy.GetComponent<OldEnemyBase>().normalAttackdamage += enemyBoostDamageAmmount;
                    float newDamage = enemy.GetComponent<EnemyBase>().GetAttackDamage();
                    enemy.GetComponent<EnemyBase>().SetAttackDamage(newDamage + enemyBoostDamageAmmount);
                }
                feedbackRef.PlayAnimation(enemyBoostTimeThresshold);
                enemyBoostTime = 0;
                Debug.Log("EnemyBoost");
            }
        }


        //Efectos de dongrades
        if (SceneManager.GetActiveScene().buildIndex == 10) return;
        if (dg == SelectedDowngrade.Debil)
        {
            if (weaknessTime < weaknessCooldownTime)
            {
                weaknessTime += Time.deltaTime;
            }
            else
            {
                FindObjectOfType<PlayerControllerOverhaul>().SetDamage(stored_PlayerDamage);
            }
        }

        if(dg == SelectedDowngrade.Slime)
        {
            if (runTimerSlime)
            {
                if (fatrollTimeLeft <= 0)
                {
                    runTimerSlime = false;
                }
                else if (fatrollTimeLeft >=0)
                {
                    fatrollTimeLeft -= Time.deltaTime;
                }
                
            }
            else
            {

                FindObjectOfType<PlayerControllerOverhaul>().SetSpeed(stored_PlayerSpeed);
            }
        }
    }
    #endregion

    #region Notify
    public void OnPlayerNotify(AllPlayerActions actions)
    {
        if (SceneManager.GetActiveScene().buildIndex <= 2)
        {
            if (actions == AllPlayerActions.Start && dg == SelectedDowngrade.None)
            {
                dgIcon = dgNoIcon;
                FindObjectOfType<PlayerUI>().SetUI();
                
            }
            return;
        }

        if (actions == AllPlayerActions.Start && dg == SelectedDowngrade.None)
        {
            LoadDg();
            //Debug.Log("DiceRolls");
        }

        FindObjectOfType<PlayerUI>().SetUI();
        if (actions == AllPlayerActions.Start)
        {
            enemies.Clear();
        }
        switch (dg)
        {
            case SelectedDowngrade.Stamina:
                if (actions == AllPlayerActions.useAbility)
                {
                    FindObjectOfType<PlayerControllerOverhaul>().UseStamina(staminaLossAmmount);
                    feedbackRef.PlayAnimation(0, 5f);
                    Debug.Log("StaminaUsed");

                }
                break;
            case SelectedDowngrade.Slime:
                if (actions == AllPlayerActions.Dodge)
                {
                    ResetFatRoll();
                    FindObjectOfType<PlayerControllerOverhaul>().SetSpeed(fatrollSpeedAmmount);
                    
                    feedbackRef.PlayAnimation(fatrollTime);
                    Debug.Log("Fatroll");
                }
                break;
            case SelectedDowngrade.Rodilla:
                if (actions == AllPlayerActions.LowStamina)
                {
                    if (FindObjectOfType<PlayerControllerOverhaul>().GetStamina() <= asthmaStaminaThresshold)
                    {
                        FindObjectOfType<PlayerControllerOverhaul>().TakeDamageProxy(asthmaHealthLossPercentage);
                        feedbackRef.PlayAnimation(0, 5f);
                        Debug.Log("Asthma");
                    }
                }
                break;
            case SelectedDowngrade.Paralisis:
                if (actions == AllPlayerActions.SuccesfullParry)
                {
                    FindObjectOfType<PlayerControllerOverhaul>().SetParalisisStatus(true, paralysisTime);

                    EnemyBase[] enemyBases = FindObjectsOfType<EnemyBase>();
                    foreach (EnemyBase enemy in enemyBases)
                    {
                        if (enemy.GetIsParried()) enemy.TakeDamageProxy(damageAmmount);
                    }
                    feedbackRef.PlayAnimation(paralysisTime);
                    Debug.Log("Paralysis");
                }
                break;
            case SelectedDowngrade.Moneda:
                if (actions == AllPlayerActions.Start)
                {
                    FindObjectOfType<PlayerControllerOverhaul>().SetMoneda(true);
                    Debug.Log("Moneda Active");
                }

                if (actions == AllPlayerActions.useItem)
                {
                    // 50% chance to take damage
                    if (Random.Range(0, 2) == 0)
                    {
                        FindObjectOfType<PlayerControllerOverhaul>().TakeDamageProxy(badLuckHealthLossAmmount);
                        FindObjectOfType<PlayerControllerOverhaul>().MonedaUseItem(false);
                        feedbackRef.PlayAnimation(0,5f);
                        Debug.Log("BadLuck");
                    }
                    else
                    {
                        FindObjectOfType<PlayerControllerOverhaul>().MonedaUseItem(true);
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
                    FindObjectOfType<PlayerControllerOverhaul>().SetDamage(weaknessDamageAmmount);
                    weaknessTime = 0;
                    feedbackRef.PlayAnimation(weaknessCooldownTime);
                    Debug.Log("Weakness");
                }
                break;
            case SelectedDowngrade.Daga:
                if (actions == AllPlayerActions.Dodge)
                {
                    onSwitch = !onSwitch;
                    FindObjectOfType<PlayerControllerOverhaul>().SetCanAttack(onSwitch);
                    feedbackRef.SwitchIcon(onSwitch);
                    Debug.Log("Attack is: " + onSwitch);
                }
                break;
            case SelectedDowngrade.Dados:
                if (actions == AllPlayerActions.Start)
                {
                    FindObjectOfType<PlayerControllerOverhaul>().SetRollQuantity(true, diceRolls);
                    feedbackRef.maxRolls = diceRolls;
                    Debug.Log("DiceRolls");
                }
                if (actions == AllPlayerActions.Dodge)
                {

                    feedbackRef.image.fillAmount = (FindObjectOfType<PlayerControllerOverhaul>().GetRemainingsRolls() / diceRolls);
                    //print("Rolls en fillAmount: " + FindObjectOfType<PlayerControllerOverhaul>().GetRemainingsRolls() / diceRolls);
                }
                break;

        }
    }

    public void OnEnemyNotify(AllEnemyActions actions)
    {
        if (SceneManager.GetActiveScene().buildIndex <= 2) return;

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
        fatrollTimeLeft = fatrollTime;
        runTimerSlime = true;
    }
    #endregion

    public SelectedDowngrade GetSelectedDowngrade()
    {
        return dg;
    }
}

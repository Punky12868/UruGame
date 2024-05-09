using System.Collections.Generic;
using UnityEngine;

public class BossBase : Subject, IAnimController
{
    #region Variables

    #region Hidden Variables
    [HideInInspector] protected AnimationsHolder animHolder;
    [HideInInspector] protected List<string> animationIDs;
    [HideInInspector] protected Animator anim;
    [HideInInspector] protected Rigidbody rb;
    [HideInInspector] protected Transform pivot;
    [HideInInspector] protected Transform target;
    [HideInInspector] protected Vector3 lastTargetDir;
    [HideInInspector] protected Vector3 targetDir;
    [HideInInspector] protected ParticleSystem.EmissionModule _particleEmission;

    [HideInInspector] protected float animClipLength;
    [HideInInspector] protected bool isAnimationDone;

    [HideInInspector] protected bool isMoving;
    [HideInInspector] protected bool isAttacking;
    [HideInInspector] protected string attackType;
    [HideInInspector] protected bool isSummoningObjects;
    [HideInInspector] protected bool isSpriteFlipped;
    [HideInInspector] protected bool hasQueuedAnimation;

    [HideInInspector] protected BehaviourBase currentMovementBehaviour;
    [HideInInspector] protected BehaviourBase currentAttackBehaviour;
    #endregion

    [Header("Boss General Stats")]
    public string bossName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float ammountOfFases;

    [Header("Boss Behaviours")]
    [SerializeField] protected BehaviourBase[] behaviours;

    [Header("Health")]
    public float health;
    protected float currentHealth;

    [Header("Attack")]
    public AttackBase[] attacks;

    [Header("Dialogs"), TextArea]
    public string[] dialogLines;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        SetAnimHolder();
        SetBehaviours();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        Movement();
        //Attack();
    }

    #endregion

    #region Movement

    public virtual void Movement()
    {
        if (currentMovementBehaviour == null) return;

        SetDynamicBehaviourVariables(currentMovementBehaviour);
        currentMovementBehaviour.MovementBehaviour();
    }

    #endregion

    #region Take Damage

    public virtual void TakeDamage(float damage)
    {
    }

    #endregion

    #region Utility

    public void SetTargetDirection()
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 enemyPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 dir = (targetPos - enemyPos).normalized;
        targetDir = dir;
    }

    private void SetBehaviours()
    {
        if (NullOrCero.isArrayNullOrCero(behaviours)) return;

        foreach (var behaviour in behaviours)
        {
            switch (behaviour.behaviourType)
            {
                case BehaviourType.Movement:
                    currentMovementBehaviour = behaviour;
                    SetStaticBehaviourVariables(behaviour);
                    break;
                case BehaviourType.Attack:
                    currentAttackBehaviour = behaviour;
                    SetStaticBehaviourVariables(behaviour);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetDynamicBehaviourVariables(BehaviourBase bhBase)
    {
        switch (bhBase.behaviourType)
        {
            case BehaviourType.Movement:
                if (bhBase is WitchMovementBh)
                {
                    WitchMovementBh movementBehaviour = (WitchMovementBh)bhBase;
                    movementBehaviour.SetDynamicVariables(targetDir);
                }
                break;
            case BehaviourType.Attack:
                break;
            default:
                break;
        }
    }

    private void SetStaticBehaviourVariables(BehaviourBase bhBase)
    {
        switch (bhBase.behaviourType)
        {
            case BehaviourType.Movement:
                break;
            case BehaviourType.Attack:
                break;
            default:
                break;
        }
    }

    public void ReplaceBehaviour(BehaviourBase bhBase)
    {
        switch (bhBase.behaviourType)
        {
            case BehaviourType.Movement:
                currentMovementBehaviour = bhBase;
                break;
            case BehaviourType.Attack:
                currentAttackBehaviour = bhBase;
                break;
            default:
                break;
        }
    }

    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationsHolder>();
        animHolder.Initialize();
        animationIDs = animHolder.GetAnimationsIDs();
    }

    #endregion
}

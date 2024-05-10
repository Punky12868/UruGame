public enum AllPlayerActions
{
    None,
    Start,
    End,
    LowHealth,
    Heal,
    LowStamina,
    HitEnemy,
    KilledEnemy,
    NotKilling,
    RandomNoise,
    RandomPausedNoise,
    StartBoss,
    DieToFirstFaseBoss,
    MidBoss,
    DieToMidFaceBoss,
    ParryBoss,
    HitBoss,
    EndBoss,

    Attack,
    Parry,
    Dodge,
    useItem,
    useEmptyItem,
    pickUpItem,
    dropItem,
    dropEmptyItem,
    Hit,
    Die,
    Victory,
    Defeat,
}

public enum AllEnemyActions
{
    None,
    Spawned,
    LightAttack,
    HeavyAttack,
    Avoiding,
    Parried,
    Hit,
    Die,
}

public enum AllBossActions
{
    None,
    Spawned,
    LightAttack,
    HeavyAttack,
    Avoiding,
    Parried,
    Hit,
    Die,
    Start,
    End,
}

public enum SelectedDowngrade
{
    None,
    FatRoll,
    Asthma,
    BadLuck,
    Weakness,
    EnemyBoost,
}

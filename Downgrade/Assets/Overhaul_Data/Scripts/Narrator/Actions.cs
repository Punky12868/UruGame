public enum AllPlayerActions
{
    None,
    Start,
    End,
    LowHealth,
    Heal,
    StaminaChanged,
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
    SuccesfullParry,
    Dodge,
    useAbility,
    useAbilityOnCooldown,
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
    Debil,
    Stamina,
    Esqueleto,
    Daga,
    Dados,
    Paralisis,
    Slime,
    Rodilla,
    Moneda,
}

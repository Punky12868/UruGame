public interface IObserver
{
    public void OnPlayerNotify(AllPlayerActions actions);

    public void OnEnemyNotify(AllEnemyActions actions);

    public void OnBossesNotify(AllBossActions actions);
}

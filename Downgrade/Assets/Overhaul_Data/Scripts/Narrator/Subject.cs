using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private List<IObserver> observers = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyPlayerObservers(AllPlayerActions action)
    {
        observers.ForEach(observer => observer.OnPlayerNotify(action));
    }

    public void NotifyEnemyObservers(AllEnemyActions action)
    {
        observers.ForEach(observer => observer.OnEnemyNotify(action));
    }

    public void NotifyBossesObservers(AllBossActions action)
    {
        observers.ForEach(observer => observer.OnBossesNotify(action));
    }
}

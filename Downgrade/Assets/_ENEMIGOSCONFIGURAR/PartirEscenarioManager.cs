using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine;

public class PartirEscenarioManager : MonoBehaviour
{
    [SerializeField] int enemiesToSpawn = 3;
    [SerializeField] GameObject[] enemyPrefab;
    [SerializeField] float intervalBetweenSpawn;
    [SerializeField] float spawnOffset;
    UnityEvent OnPartirEscenario = new UnityEvent();
    UnityEvent OnJuntarEscenario = new UnityEvent();
    BossRipBases[] bossBases;
    int activeBaseIndex;
    bool isOn;
    int spawnedEnemiesKilled;
    bool spawnEnemies;
    bool spawnedAllEnemies;

    private void Awake() 
    { 
        bossBases = FindObjectsOfType<BossRipBases>(); 
        OnPartirEscenario.AddListener(ScenarioHandler);
        OnJuntarEscenario.AddListener(ScenarioHandler);
        OnJuntarEscenario.AddListener(ScenarioHandler);
    }
    public void PartirEscenario() { OnPartirEscenario.Invoke(); isOn = true; }
    public void JuntarEscenario() { OnJuntarEscenario.Invoke(); isOn = false; }
    private void ScenarioHandler() { foreach (BossRipBases boss in bossBases) boss.RipBase(); }
    public void TransitionCompleted(BossRipBases currentBase, bool spawnEnemies)
    {
        if (!currentBase.HasPlayer()) return;
        for (int i = 0; i < bossBases.Length; i++) { if (bossBases[i] == currentBase) { activeBaseIndex = i; break; } }
        spawnedEnemiesKilled = 0;
        this.spawnEnemies = spawnEnemies;
    }

    private void Update()
    {
        if (isOn && spawnEnemies)
        {
            StartCoroutine(SpawnEnemies());
            spawnEnemies = false;
        }

        if (spawnedAllEnemies && spawnedEnemiesKilled == enemiesToSpawn)
        {
            JuntarEscenario();
            spawnedAllEnemies = false;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomIndex = Random.Range(0, enemyPrefab.Length);
            Vector2 randomPos = new Vector2(Random.Range(-spawnOffset, spawnOffset), Random.Range(-spawnOffset, spawnOffset));
            Vector3 spawnPos = bossBases[activeBaseIndex].GetSpawner().position + new Vector3(randomPos.x, 0, randomPos.y);
            Instantiate(enemyPrefab[randomIndex], spawnPos, Quaternion.identity);
            if (i == enemiesToSpawn - 1) spawnedAllEnemies = true;
            yield return new WaitForSeconds(intervalBetweenSpawn);
        }
    }

    public void EnemyKilled() { spawnedEnemiesKilled++; }
}

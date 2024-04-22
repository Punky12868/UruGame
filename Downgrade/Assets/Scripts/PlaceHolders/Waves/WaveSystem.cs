using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float timerEnemySpawn;
    [SerializeField] private float timer;
    [SerializeField] private List<Wave> waves = new List<Wave>();

    private int currentWave;
    private int currentWaveData;
    private bool isWaveFinished;

    private List<EnemyBase> enemyBases = new List<EnemyBase>();

    private void Start()
    {
        currentWave = 0;
        currentWaveData = 0;
        isWaveFinished = false;
    }

    private void Update()
    {
        if (!isWaveFinished)
        {
            timer += Time.deltaTime;
        }

        if (isWaveFinished)
        {
            timer = 0;
            timerEnemySpawn = 0;
            isWaveFinished = false;
            currentWave++;
            currentWaveData = 0;
        }
        else
        {
            if (currentWave < waves.Count)
            {
                if (currentWaveData < waves[currentWave].waves.Count)
                {
                    if (waves[currentWave].waves[currentWaveData].hasDelay)
                    {
                        timerEnemySpawn += Time.deltaTime;
                        if (timerEnemySpawn >= waves[currentWave].waves[currentWaveData].delayForNextEnemy)
                        {
                            timerEnemySpawn = 0;
                            Vector3 pos = new Vector3(waves[currentWave].waves[currentWaveData].position.x, 0.002f, waves[currentWave].waves[currentWaveData].position.y);
                            GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                            enemyBases.Add(enemy.GetComponent<EnemyBase>());
                            currentWaveData++;
                        }
                    }
                    else
                    {
                        Vector3 pos = new Vector3(waves[currentWave].waves[currentWaveData].position.x, 0.002f, waves[currentWave].waves[currentWaveData].position.y);
                        GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                        enemyBases.Add(enemy.GetComponent<EnemyBase>());
                        currentWaveData++;
                    }
                }
            }
            else
            {
                Debug.Log("All waves are finished");
                AudioManager.instance.PlaySFX(1);
                GameManager.Instance.Victory();
            }
        }
    }

    public void UpdateDeadEnemies()
    {
        for (int i = 0; i < enemyBases.Count; i++)
        {
            if (!enemyBases[i].isDead)
            {
                return;
            }
        }

        isWaveFinished = true;
    }
}

[System.Serializable]
public class Wave
{
    public List<WaveData> waves = new List<WaveData>();
}

[System.Serializable]
public class WaveData
{
    public GameObject enemy;
    public Vector2 position;
    public bool hasDelay;
    [ShowIf("hasDelay", true, true)] public float delayForNextEnemy;
}

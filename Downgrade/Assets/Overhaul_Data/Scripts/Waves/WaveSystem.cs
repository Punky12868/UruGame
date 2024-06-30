using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private bool isBossArea;
    [SerializeField] private float timerEnemySpawn;

    public List<Wave> waves = new List<Wave>();

    private float timer;
    private int currentWave;
    private int currentWaveData;
    private bool isWaveFinished;
    private bool victory;

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
                if (currentWaveData < waves[currentWave].enemiesInWave.Count)
                {
                    if (waves[currentWave].enemiesInWave[currentWaveData].delayForNextEnemy > 0)
                    {
                        timerEnemySpawn += Time.deltaTime;
                        if (timerEnemySpawn >= waves[currentWave].enemiesInWave[currentWaveData].delayForNextEnemy)
                        {
                            timerEnemySpawn = 0;
                            Vector3 pos = new Vector3(waves[currentWave].enemiesInWave[currentWaveData].position.x, 
                                waves[currentWave].enemiesInWave[currentWaveData].yPositionOffset, 
                                waves[currentWave].enemiesInWave[currentWaveData].position.y);
                            //GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                            //enemyBases.Add(enemy.GetComponent<EnemyBase>());

                            // Check the stage and enemy type to spawn the correct enemy
                            SpawnEnemy(pos);
                            currentWaveData++;
                        }
                    }
                    else
                    {
                        Vector3 pos = new Vector3(waves[currentWave].enemiesInWave[currentWaveData].position.x, 
                            waves[currentWave].enemiesInWave[currentWaveData].yPositionOffset, 
                            waves[currentWave].enemiesInWave[currentWaveData].position.y);

                        //GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                        //enemyBases.Add(enemy.GetComponent<EnemyBase>());
                        SpawnEnemy(pos);
                        currentWaveData++;
                    }
                }
            }
            else
            {
                if (victory) return;

                victory = true;
                Debug.Log("All waves are finished");
                AudioManager.instance.PlaySFX(1);
                GameManager.Instance.Victory(isBossArea);
            }
        }
    }

    public void SpawnEnemy(Vector3 pos)
    {
        GameObject enemy = null;
        if (!waves[currentWave].enemiesInWave[currentWaveData].spawnInLastEnemyPosition)
        {
            enemy = Instantiate(waves[currentWave].enemiesInWave[currentWaveData].enemyPrefab, pos, Quaternion.identity);
            Debug.Log("Enemy spawned at: " + enemy.transform.position);
        }
        else
        {
            Vector3 lastEnemyPos = enemyBases[enemyBases.Count - 1].transform.position;
            enemy = Instantiate(waves[currentWave].enemiesInWave[currentWaveData].enemyPrefab, lastEnemyPos, Quaternion.identity);
            Debug.Log("2nd Enemy spawned at: " + enemyBases[enemyBases.Count - 1].transform.position);
        }
        enemyBases.Add(enemy.GetComponent<EnemyBase>());
    }

    public void UpdateDeadEnemies()
    {
        for (int i = 0; i < enemyBases.Count; i++)
        {
            if (!enemyBases[i].GetIsDead()) return;

            /*if (enemyBases[i].GetEnemyType() == EnemyType.Boss)
            {
                if(!FindObjectOfType<BossController>().NextFase()) return;
            }*/
        }

        isWaveFinished = true;
    }
}

[System.Serializable]
public class Wave
{
    public List<WaveData> enemiesInWave = new List<WaveData>();
}

[System.Serializable]
public class WaveData
{
    //public string enemyName;
    public GameObject enemyPrefab;

    public bool spawnInLastEnemyPosition;
    public Vector2 position;
    public float yPositionOffset;
    //public bool hasDelay;
    public float delayForNextEnemy;
}

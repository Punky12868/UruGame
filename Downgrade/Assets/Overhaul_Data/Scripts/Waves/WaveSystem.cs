using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float timerEnemySpawn;
    
    // 0 - Arboles
    // 1 - SlimeBarro
    // 2 - Sapo
    // 3 - ArbolGigante
    // 4 - Ninfa
    // 5 - Ciclope
    // 6 - Macizo

    [SerializeField] private GameObject[] swampEnemies;
    /*[SerializeField] private GameObject[] circusEnemies;
    [SerializeField] private GameObject[] dungeonEnemies;
    [SerializeField] private GameObject[] castleEnemies;*/


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
                if (currentWaveData < waves[currentWave].waves.Count)
                {
                    if (waves[currentWave].waves[currentWaveData].hasDelay)
                    {
                        timerEnemySpawn += Time.deltaTime;
                        if (timerEnemySpawn >= waves[currentWave].waves[currentWaveData].delayForNextEnemy)
                        {
                            timerEnemySpawn = 0;
                            Vector3 pos = new Vector3(waves[currentWave].waves[currentWaveData].position.x, 0.002f, waves[currentWave].waves[currentWaveData].position.y);
                            //GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                            //enemyBases.Add(enemy.GetComponent<EnemyBase>());

                            // Check the stage and enemy type to spawn the correct enemy
                            SwitchStatement(pos);
                            currentWaveData++;
                        }
                    }
                    else
                    {
                        Vector3 pos = new Vector3(waves[currentWave].waves[currentWaveData].position.x, 0.002f, waves[currentWave].waves[currentWaveData].position.y);

                        //GameObject enemy = Instantiate(waves[currentWave].waves[currentWaveData].enemy, pos, Quaternion.identity);
                        //enemyBases.Add(enemy.GetComponent<EnemyBase>());
                        SwitchStatement(pos);
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
                GameManager.Instance.Victory();
            }
        }
    }

    public void SwitchStatement(Vector3 pos)
    {
        switch (waves[currentWave].waves[currentWaveData].stage)
        {
            case WaveData.Stage.Swamp:
                switch (waves[currentWave].waves[currentWaveData].enemyType)
                {
                    case WaveData.EnemyType.Arboles:
                        GameObject arbolEnemy = Instantiate(swampEnemies[0], pos, Quaternion.identity);
                        enemyBases.Add(arbolEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SlimeBarro:
                        GameObject slimeEnemy = Instantiate(swampEnemies[1], pos, Quaternion.identity);
                        enemyBases.Add(slimeEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Sapo:
                        GameObject sapoEnemy = Instantiate(swampEnemies[2], pos, Quaternion.identity);
                        enemyBases.Add(sapoEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.ArbolGigante:
                        GameObject arbolGiganteEnemy = Instantiate(swampEnemies[3], pos, Quaternion.identity);
                        enemyBases.Add(arbolGiganteEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Ninfa:
                        GameObject ninfaEnemy = Instantiate(swampEnemies[4], pos, Quaternion.identity);
                        enemyBases.Add(ninfaEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Ciclope:
                        GameObject ciclopeEnemy = Instantiate(swampEnemies[5], pos, Quaternion.identity);
                        enemyBases.Add(ciclopeEnemy.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Macizo:
                        GameObject macizoEnemy = Instantiate(swampEnemies[6], pos, Quaternion.identity);
                        enemyBases.Add(macizoEnemy.GetComponent<EnemyBase>());
                        break;
                }
                break;
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
    public enum EnemyType
    {
        None,
        Arboles,
        SlimeBarro,
        Sapo,
        ArbolGigante,
        Ninfa,
        Ciclope,
        Macizo
    }
    public enum Stage
    {
        None,
        Swamp
    }
    public EnemyType enemyType;
    public Stage stage;
    public Vector2 position;
    public bool hasDelay;
    public float delayForNextEnemy;
}

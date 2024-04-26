using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private float timerEnemySpawn;

    // 0 = Small, 1 = SmallRanged, 2 = SmallStatic, 3 = Big
    [SerializeField] private GameObject[] swampEnemies;
    [SerializeField] private GameObject[] circusEnemies;
    [SerializeField] private GameObject[] dungeonEnemies;
    [SerializeField] private GameObject[] castleEnemies;


    public List<Wave> waves = new List<Wave>();

    private float timer;
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
                    case WaveData.EnemyType.Small:
                        GameObject enemys = Instantiate(swampEnemies[0], pos, Quaternion.identity);
                        enemyBases.Add(enemys.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallRanged:
                        GameObject enemysr = Instantiate(swampEnemies[1], pos, Quaternion.identity);
                        enemyBases.Add(enemysr.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallStatic:
                        GameObject enemysrt = Instantiate(swampEnemies[2], pos, Quaternion.identity);
                        enemyBases.Add(enemysrt.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Big:
                        GameObject enemyb = Instantiate(swampEnemies[3], pos, Quaternion.identity);
                        enemyBases.Add(enemyb.GetComponent<EnemyBase>());
                        break;
                }
                break;
            case WaveData.Stage.Circus:
                switch (waves[currentWave].waves[currentWaveData].enemyType)
                {
                    case WaveData.EnemyType.Small:
                        GameObject enemys = Instantiate(circusEnemies[0], pos, Quaternion.identity);
                        enemyBases.Add(enemys.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallRanged:
                        GameObject enemysr = Instantiate(circusEnemies[1], pos, Quaternion.identity);
                        enemyBases.Add(enemysr.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallStatic:
                        GameObject enemysrt = Instantiate(circusEnemies[2], pos, Quaternion.identity);
                        enemyBases.Add(enemysrt.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Big:
                        GameObject enemyb = Instantiate(circusEnemies[3], pos, Quaternion.identity);
                        enemyBases.Add(enemyb.GetComponent<EnemyBase>());
                        break;
                }
                break;
            case WaveData.Stage.Dungeon:
                switch (waves[currentWave].waves[currentWaveData].enemyType)
                {
                    case WaveData.EnemyType.Small:
                        GameObject enemys = Instantiate(dungeonEnemies[0], pos, Quaternion.identity);
                        enemyBases.Add(enemys.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallRanged:
                        GameObject enemysr = Instantiate(dungeonEnemies[1], pos, Quaternion.identity);
                        enemyBases.Add(enemysr.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallStatic:
                        GameObject enemysrt = Instantiate(dungeonEnemies[2], pos, Quaternion.identity);
                        enemyBases.Add(enemysrt.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Big:
                        GameObject enemyb = Instantiate(dungeonEnemies[3], pos, Quaternion.identity);
                        enemyBases.Add(enemyb.GetComponent<EnemyBase>());
                        break;
                }
                break;
            case WaveData.Stage.Castle:
                switch (waves[currentWave].waves[currentWaveData].enemyType)
                {
                    case WaveData.EnemyType.Small:
                        GameObject enemys = Instantiate(castleEnemies[0], pos, Quaternion.identity);
                        enemyBases.Add(enemys.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallRanged:
                        GameObject enemysr = Instantiate(castleEnemies[1], pos, Quaternion.identity);
                        enemyBases.Add(enemysr.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.SmallStatic:
                        GameObject enemysrt = Instantiate(castleEnemies[2], pos, Quaternion.identity);
                        enemyBases.Add(enemysrt.GetComponent<EnemyBase>());
                        break;
                    case WaveData.EnemyType.Big:
                        GameObject enemyb = Instantiate(castleEnemies[3], pos, Quaternion.identity);
                        enemyBases.Add(enemyb.GetComponent<EnemyBase>());
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
        Small,
        SmallRanged,
        SmallStatic,
        Big
    }
    public enum Stage
    {
        None,
        Swamp,
        Circus,
        Dungeon,
        Castle
    }
    public EnemyType enemyType;
    public Stage stage;
    public Vector2 position;
    public bool hasDelay;
    [ShowIf("hasDelay", true, true)] public float delayForNextEnemy;
}

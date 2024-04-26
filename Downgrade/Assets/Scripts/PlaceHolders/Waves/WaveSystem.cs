using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;

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
    [Vector2Grid]
    public Vector2 position;
    public bool hasDelay;
    [ShowIf("hasDelay", true, true)] public float delayForNextEnemy;
}

public class Vector2GridAttribute : Attribute { }

public class Vector2GridAttributeDrawer : OdinAttributeDrawer<Vector2GridAttribute, Vector2>
{
    private bool[,] grid;
    private Vector2[] savedPositions; // Array para guardar las posiciones

    public int gridSizeX = 12; // Tamaño del cuadrado de la cuadrícula en el inspector
    public int gridSizeY = 9; // Tamaño del cuadrado de la cuadrícula en el inspector

    private float worldMinX = -5;
    private float worldMaxX = 5;

    private float worldMinY = -4;
    private float worldMaxY = 4;

    protected override void Initialize()
    {
        grid = new bool[gridSizeX, gridSizeY];
        LoadPositions(); // Cargar las posiciones guardadas al inicializar
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Vector2 value = this.ValueEntry.SmartValue;

        GUILayout.BeginVertical();

        for (int y = gridSizeY - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < gridSizeX; x++)
            {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(20), GUILayout.Height(20));

                bool clicked = GUI.Button(rect, grid[x, y] ? "O" : "");
                if (clicked)
                {
                    // Map inspector coordinates to world coordinates
                    float worldX = Mathf.Lerp(worldMinX, worldMaxX, (float)x / (float)(gridSizeX - 1));
                    float worldY = Mathf.Lerp(worldMinY, worldMaxY, (float)y / (float)(gridSizeY - 1));

                    value = new Vector2(worldX, worldY);
                    grid = new bool[gridSizeX, gridSizeY];
                    grid[x, y] = true;

                    // Guardar las posiciones después de hacer clic
                    SavePositions();
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        this.ValueEntry.SmartValue = value;
    }

    // Método para guardar las posiciones en PlayerPrefs
    private void SavePositions()
    {
        string positionsString = "";

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                if (grid[x, y])
                {
                    positionsString += x + "," + y + ";";
                }
            }
        }

        PlayerPrefs.SetString("SavedPositions", positionsString);
        PlayerPrefs.Save();
    }

    // Método para cargar las posiciones desde PlayerPrefs
    private void LoadPositions()
    {
        string positionsString = PlayerPrefs.GetString("SavedPositions", "");

        if (!string.IsNullOrEmpty(positionsString))
        {
            string[] positions = positionsString.Split(';');

            foreach (string position in positions)
            {
                string[] coordinates = position.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);
                grid[x, y] = true;
            }
        }
    }
}

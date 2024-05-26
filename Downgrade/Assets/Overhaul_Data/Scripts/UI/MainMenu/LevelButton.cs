using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelID;
    [SerializeField] private int levelLockID;
    [SerializeField] private string levelName;
    [SerializeField] private bool unlocked;
    [SerializeField] private bool canBeUnlocked;

    private void Awake()
    {
        levelID++;
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadLevel);
    }

    public void LoadLevel()
    {
        // quiza añadir transición antes de cargar el nivel¿¿
        if (unlocked) GameManager.Instance.LoadScene(levelID);
    }

    public int GetLevelID() { return levelID; }
    public int GetLevelLockID() { return levelLockID; }
    public string GetLevelName() { return levelName; }
    public bool IsUnlocked() { return unlocked; }
    public bool CanBeUnlocked() { return canBeUnlocked; }
    public void SetLockState(bool value) { unlocked = value; }
}

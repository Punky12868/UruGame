using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimpleSaveLoad : MonoBehaviour
{
    public static SimpleSaveLoad Instance;

    [SerializeField] private string folderName = "Overhaul";
    [SerializeField] private bool consoleLog;
    private string myDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    private string fullPath;
    private string saveDataName = "";
    private string saveDataExtension = "";

    FileDefinition fileDef = new FileDefinition();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        ES3Settings.defaultSettings.location = ES3.Location.File;
        fullPath = myDocumentsPath + "/" + folderName + "/";
    }

    public void SaveData<T>(FileType type, string key, T value)
    {
        GetType(type);
        CheckFolder();

        ES3.Save<T>(key, value, fullPath + saveDataName + "." + saveDataExtension);
        if(consoleLog) Debug.Log("Saved " + key + " with value " + value + " to " + fullPath + saveDataName + "." + saveDataExtension);
    }

    public T LoadData<T>(FileType type, string key)
    {
        GetType(type);
        if (File.Exists(fullPath + saveDataName + "." + saveDataExtension))
        {
            if (consoleLog) Debug.Log("Loaded " + key + " with value " + ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension) + " from " + fullPath + saveDataName + "." + saveDataExtension);
            T value = ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension);
            return value;
        }
        return default(T);
    }

    public T LoadData<T>(FileType type, string key, T defaultValue)
    {
        GetType(type);
        if (File.Exists(fullPath + saveDataName + "." + saveDataExtension))
        {
            if (consoleLog) Debug.Log("Loaded " + key + " with value " + ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension) + " from " + fullPath + saveDataName + "." + saveDataExtension);
            T value = ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension);
            return value;
        }
        return defaultValue;
    }

    public bool CheckKey(string key)
    {
        if (ES3.KeyExists(key)) return true;
        return false;
    }

    private void GetType(FileType type)
    {
        switch (type)
        {
            case FileType.Config:

                saveDataName = fileDef.config_saveDataName;
                saveDataExtension = fileDef.config_saveDataExtension;

                break;
            case FileType.Gameplay:

                saveDataName = fileDef.gameplay_saveDataName;
                saveDataExtension = fileDef.gameplay_saveDataExtension;

                break;
            default:
                break;
        }
    }

    private void CheckFolder()
    {
        if (!Directory.Exists(myDocumentsPath + folderName)) Directory.CreateDirectory(myDocumentsPath + folderName);
    }
}

public enum FileType { Config, Gameplay }
public class FileDefinition
{
    public readonly string gameplay_saveDataName = "_Player";
    public readonly string gameplay_saveDataExtension = "ovh";

    public readonly string config_saveDataName = "Config";
    public readonly string config_saveDataExtension = "cfg";
}
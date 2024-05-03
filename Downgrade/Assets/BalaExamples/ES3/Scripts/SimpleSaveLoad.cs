using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleSaveLoad : MonoBehaviour
{
    public static SimpleSaveLoad Instance;

    [SerializeField] private string folderName = "Overhaul";
    private string myDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    private string fullPath;
    private string saveDataName = "";
    private string saveDataExtension = "";

    FileDefinition fileDef = new FileDefinition();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        ES3Settings.defaultSettings.location = ES3.Location.File;
        fullPath = myDocumentsPath + "/" + folderName + "/";

        
    }

    public void SaveData<T>(FileType type, string key, T value)
    {
        GetType(type);
        CheckFolder();

        ES3.Save<T>(key, value, fullPath + saveDataName + "." + saveDataExtension);
        Debug.Log("Saved " + key + " with value " + value + " to " + fullPath + saveDataName + "." + saveDataExtension);
    }

    public T LoadData<T>(FileType type, string key)
    {
        if (System.IO.File.Exists(fullPath + saveDataName + "." + saveDataExtension))
        {
            GetType(type);

            Debug.Log("Loaded " + key + " with value " + ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension) + " from " + fullPath + saveDataName + "." + saveDataExtension);
            return ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension);
        }
        else
        {
            return default(T);
        }
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
        if (!System.IO.Directory.Exists(myDocumentsPath + folderName))
        {
            System.IO.Directory.CreateDirectory(myDocumentsPath + folderName);
        }
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
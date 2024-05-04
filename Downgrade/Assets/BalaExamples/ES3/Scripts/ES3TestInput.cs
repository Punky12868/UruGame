using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES3TestInput : MonoBehaviour
{
    [SerializeField] private int testInt = 0;
    [SerializeField] private float testFloat = 0.0f;
    [SerializeField] private string testString = "";
    [SerializeField] private bool testBool = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SimpleSaveLoad.Instance.SaveData<int>(FileType.Gameplay, "testInt", testInt);
            SimpleSaveLoad.Instance.SaveData<float>(FileType.Gameplay, "testFloat", testFloat);
            SimpleSaveLoad.Instance.SaveData<string>(FileType.Gameplay, "testString", testString);
            SimpleSaveLoad.Instance.SaveData<bool>(FileType.Gameplay, "testBool", testBool);

            SimpleSaveLoad.Instance.SaveData<int>(FileType.Config, "testInt", testInt);
            SimpleSaveLoad.Instance.SaveData<float>(FileType.Config, "testFloat", testFloat);
            SimpleSaveLoad.Instance.SaveData<string>(FileType.Config, "testString", testString);
            SimpleSaveLoad.Instance.SaveData<bool>(FileType.Config, "testBool", testBool);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            testInt = SimpleSaveLoad.Instance.LoadData<int>(FileType.Gameplay, "testInt");
            testFloat = SimpleSaveLoad.Instance.LoadData<float>(FileType.Gameplay, "testFloat");
            testString = SimpleSaveLoad.Instance.LoadData<string>(FileType.Gameplay, "testString");
            testBool = SimpleSaveLoad.Instance.LoadData<bool>(FileType.Gameplay, "testBool");

            testInt = SimpleSaveLoad.Instance.LoadData<int>(FileType.Config, "testInt");
            testFloat = SimpleSaveLoad.Instance.LoadData<float>(FileType.Config, "testFloat");
            testString = SimpleSaveLoad.Instance.LoadData<string>(FileType.Config, "testString");
            testBool = SimpleSaveLoad.Instance.LoadData<bool>(FileType.Config, "testBool");
        }
    }
}

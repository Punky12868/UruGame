using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelUnlocker : MonoBehaviour
{
    [SerializeField] UISelector uiSelector;
    [SerializeField] string key = "level_";
    Button[] levels;

    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }
    private void DelayedAwake()
    {
        levels = uiSelector.GetManualButtons();

        for (int i = 0; i < levels.Length; i++)
        {
            LevelButton levelButton = levels[i].GetComponent<LevelButton>();
            GameObject lockImage = levelButton.transform.GetChild(0).gameObject;

            if (!levelButton.CanBeUnlocked()) { levelLockStatus(levelButton, lockImage, true); continue; }
            if (i == 0) { levelLockStatus(levelButton, lockImage); continue; }

            levelButton.SetLockState(SimpleSaveLoad.Instance.LoadData<bool>(FileType.Gameplay, key + i, false));
            levelLockStatus(levelButton, lockImage);
        }
    }

    private void levelLockStatus(LevelButton levelButton, GameObject lockImage, bool unlockable = false)
    {
        if (unlockable)
        {
            lockImage.SetActive(true);
            if (levelButton.IsUnlocked()) levelButton.SetLockState(false);
            return;
        }

        if (levelButton.IsUnlocked()) lockImage.SetActive(false);
        else if (!levelButton.IsUnlocked()) lockImage.SetActive(true);
    }

    public void LockAllLevels()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (i == 0) continue;

            LevelButton levelButton = levels[i].GetComponent<LevelButton>();
            GameObject lockImage = levelButton.transform.GetChild(0).gameObject;

            levelButton.SetLockState(true);
            levelLockStatus(levelButton, lockImage);
            
            SimpleSaveLoad.Instance.SaveData(FileType.Gameplay, key + i, false);
        }
    }
}

using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayerMap : MonoBehaviour
{
    [SerializeField] private bool loadUiMap;
    [SerializeField] private bool consoleLog;
    [SerializeField] private int downgradeSelectionIndex;
    ControllerType lastControllerType;
    string lastCategoryLoaded;

    private void Awake() { LoadMaps(); }

    private void Update()
    {
        if (GetCurrentController.GetLastActiveController() == null) return;

        if (SceneManager.GetActiveScene().buildIndex == 0 && !loadUiMap ||
            SceneManager.GetActiveScene().buildIndex == downgradeSelectionIndex && !loadUiMap) loadUiMap = true;

        if (lastControllerType != GetCurrentController.GetLastActiveController().type) LoadMaps();

        if (lastCategoryLoaded != "UI" && loadUiMap) LoadMaps();
        if (lastCategoryLoaded != "Default" && !loadUiMap) LoadMaps();
    }

    private void LoadMaps()
    {
        LoadAllMaps();

        /*if (GetCurrentController.GetLastActiveController() == null) ChangeControllerType(ControllerType.Joystick);
        else
        {
            LoadAllMaps();

            /*if (GetCurrentController.GetLastActiveController().type == ControllerType.Joystick) ChangeControllerType(ControllerType.Joystick);
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Keyboard) ChangeControllerType(ControllerType.Keyboard);
        }

        Log("Loaded maps for " + lastControllerType + " controller");*/
    }

    private void ChangeControllerType(ControllerType type)
    {
        if (loadUiMap)
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "Default", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "UI", "Default", true);
            lastCategoryLoaded = "UI";
            Log("Loaded UI maps for " + lastControllerType + " controller");
        }
        else
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "UI", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "Default", "Default", true);
            lastCategoryLoaded = "Default";
            Log("Loaded Default maps for " + lastControllerType + " controller");
        }

        lastControllerType = type;
    }

    private void LoadAllMaps()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && !loadUiMap) loadUiMap = true;

        if (loadUiMap)
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "Default", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "UI", "Default", true);

            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", true);
            lastCategoryLoaded = "UI";
            Log("Loaded UI maps for " + lastControllerType + " controller");
        }
        else
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "UI", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "Default", "Default", true);

            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", true);
            lastCategoryLoaded = "Default";
            Log("Loaded Default maps for " + lastControllerType + " controller");
        }
    }

    public void SetLoadUiMap(bool value) { loadUiMap = value; }
    public bool GetLoadUiMap() { return loadUiMap;}
    private void Log(string message) { if (consoleLog) Debug.Log(message); }
}

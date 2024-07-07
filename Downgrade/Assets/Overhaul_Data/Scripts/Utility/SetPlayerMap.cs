using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayerMap : MonoBehaviour
{
    [SerializeField] private bool loadUiMap;
    [SerializeField] private bool consoleLog;
    string lastCategoryLoaded;
    int downgradeSelectionIndex;
    int currentSceneIndex;
    bool sceneChange;

    private void Awake() { downgradeSelectionIndex = SceneManager.sceneCountInBuildSettings - 1; LoadAllMaps(); }

    private void Update()
    {
        if (GetCurrentController.GetLastActiveController() == null) return;

        if (SceneManager.GetActiveScene().buildIndex == 0 && !loadUiMap ||
            SceneManager.GetActiveScene().buildIndex == downgradeSelectionIndex && !loadUiMap) loadUiMap = true;

        if (lastCategoryLoaded != "UI" && loadUiMap) LoadAllMaps();
        if (lastCategoryLoaded != "Default" && !loadUiMap) LoadAllMaps();

        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex && !sceneChange)
        {
            //Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAaa");
            sceneChange = true;
            Invoker.InvokeDelayed(LoadAllMaps, 0.1f);
            //LoadAllMaps();
        }
    }

    public void PlayerStarts()
    {
        currentSceneIndex--;
    }

    private void LoadAllMaps()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneChange = false;
        if (SceneManager.GetActiveScene().buildIndex == 0 && !loadUiMap) loadUiMap = true;

        if (loadUiMap)
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", true);

            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "Default", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "UI", "Default", true);

            lastCategoryLoaded = "UI";
            Log("Loaded UI maps");
        }
        else
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", true);

            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "UI", "Default", false);
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(ControllerType.Joystick, 0, "Default", "Default", true);
            
            lastCategoryLoaded = "Default";
            Log("Loaded Default maps");
        }
    }

    public void SetLoadUiMap(bool value) { loadUiMap = value; }
    public bool GetLoadUiMap() { return loadUiMap;}
    private void Log(string message) { if (consoleLog) Debug.Log(message); }
}

using Rewired;
using UnityEngine;

public class SetPlayerMap : MonoBehaviour
{
    [SerializeField] private bool loadUiMap;
    ControllerType lastControllerType;
    string lastCategoryLoaded;

    private void Awake() { LoadMaps(); }

    private void Update()
    {
        if (GetCurrentController.GetLastActiveController() == null) return;
        if (lastControllerType != GetCurrentController.GetLastActiveController().type) LoadMaps();

        if (lastCategoryLoaded != "UI" && loadUiMap) LoadMaps();
        if (lastCategoryLoaded != "Default" && !loadUiMap) LoadMaps();
    }

    private void LoadMaps()
    {
        if (GetCurrentController.GetLastActiveController() == null) ChangeControllerType(ControllerType.Joystick);
        else
        {
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Joystick) ChangeControllerType(ControllerType.Joystick);
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Keyboard) ChangeControllerType(ControllerType.Keyboard);
        }
    }

    private void ChangeControllerType(ControllerType type)
    {
        if (loadUiMap)
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "UI", "Default", true);
            lastCategoryLoaded = "UI";
        }
        else
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "Default", "Default", true);
            lastCategoryLoaded = "Default";
        }

        lastControllerType = type;
    }

    public void SetLoadUiMap(bool value) { loadUiMap = value; }
}

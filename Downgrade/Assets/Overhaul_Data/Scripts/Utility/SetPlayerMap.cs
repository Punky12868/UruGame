using Rewired;
using UnityEngine;

public class SetPlayerMap : MonoBehaviour
{
    [SerializeField] private bool loadUiMap;
    ControllerType lastControllerType;

    private void Awake() { LoadMaps(); }

    private void Update()
    {
        if (GetCurrentController.GetLastActiveController() == null) return;
        if (lastControllerType != GetCurrentController.GetLastActiveController().type) LoadMaps();
    }

    private void LoadMaps()
    {
        if (GetCurrentController.GetLastActiveController() == null) ChangeControllerType(ControllerType.Keyboard);
        else
        {
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Joystick) ChangeControllerType(ControllerType.Joystick);
            if (GetCurrentController.GetLastActiveController().type == ControllerType.Keyboard) ChangeControllerType(ControllerType.Keyboard);
        }
    }

    private void ChangeControllerType(ControllerType type)
    {
        if (loadUiMap) ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "UI", "Default", true);
        else ReInput.players.GetPlayer(0).controllers.maps.LoadMap(type, 0, "Default", "Default", true);

        lastControllerType = type;
    }
}

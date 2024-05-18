using Rewired;
using UnityEngine;

public class SetPlayerMap : MonoBehaviour
{
    [SerializeField] private bool loadUiMap;
    private void Awake()
    {
        if (loadUiMap) ReInput.players.GetPlayer(0).controllers.maps.LoadMap(0, 0, "UI", "Default", true);
        else ReInput.players.GetPlayer(0).controllers.maps.LoadMap(0, 0, "Default", "Default", true);
    }
}

using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetCurrentController
{
    public static Controller GetLastActiveController()
    {
        return ReInput.players.GetPlayer(0).controllers.GetLastActiveController();
    }
}

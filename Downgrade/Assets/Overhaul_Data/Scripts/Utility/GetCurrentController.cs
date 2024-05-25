using Rewired;
public static class GetCurrentController 
{ public static Controller GetLastActiveController() { return ReInput.players.GetPlayer(0).controllers.GetLastActiveController(); } }

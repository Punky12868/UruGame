using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputDirection : MonoBehaviour
{
    // PlaceHolder for InputDirection

    public static InputDirection instance;
    private Player player;

    [Tooltip("Is used to determine the threshold for the direction when the player is moving upwards or downwards, " +
        "so the returning string is more accurate about the direction the player is moving")]
    [SerializeField] float directionThreshold = 0.2f;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);

        if (instance == null)
        {
            instance = this;
        }
    }

    public Vector2 GetDirection()
    {
        return new Vector2(player.GetAxisRaw("Horizontal"), player.GetAxisRaw("Vertical"));
    }

    public string GetDirectionString()
    {
        Vector2 direction = GetDirection();

        // Up, UpRight, UpLeft, Right, Left, Down, DownRight, DownLeft

        if (direction.y > 0)
        {
            if (direction.x > directionThreshold)
            {
                return "UpRight";
            }
            else if (direction.x < -directionThreshold)
            {
                return "UpLeft";
            }
            else
            {
                return "Up";
            }
        }
        else if (direction.y < 0)
        {
            if (direction.x > directionThreshold)
            {
                return "DownRight";
            }
            else if (direction.x < -directionThreshold)
            {
                return "DownLeft";
            }
            else
            {
                return "Down";
            }
        }
        else
        {
            if (direction.x > 0)
            {
                return "Right";
            }
            else if (direction.x < 0)
            {
                return "Left";
            }
            else
            {
                return "Relax";
            }
        }
    }
}

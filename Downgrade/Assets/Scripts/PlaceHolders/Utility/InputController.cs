using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputController : MonoBehaviour
{
    // PlaceHolder for InputDirection

    public static InputController instance;
    private Player player;

    [Tooltip("Is used to determine the threshold for the direction when the player is moving upwards or downwards, " +
        "so the returning string is more accurate about the direction the player is moving")]
    [SerializeField] float directionThreshold = 0.2f;
    string lastSavedDirection;
    Vector2 lastDirection;

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
        Vector2 direction = new Vector2(player.GetAxisRaw("Horizontal"), player.GetAxisRaw("Vertical"));

        if(direction.sqrMagnitude > directionThreshold)
        {
            if (direction != Vector2.zero)
            {
                lastDirection = direction;
            }

            return direction;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public Vector2 GetLastDirection()
    {
        return lastDirection;
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

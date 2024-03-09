using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    // PlaceHolder for the PlayerAnimations

    Animator anim;
    const string PLAYER_RELAX_UP = "Player_RelaxUp";
    const string PLAYER_RELAX_UP_RIGHT = "Player_RelaxUpRight";
    const string PLAYER_RELAX_UP_LEFT = "Player_RelaxUpLeft";
    const string PLAYER_RELAX_DOWN = "Player_RelaxDown";
    const string PLAYER_RELAX_DOWN_RIGHT = "Player_RelaxDownRight";
    const string PLAYER_RELAX_DOWN_LEFT = "Player_RelaxDownLeft";
    const string PLAYER_RUN_UP = "Player_RunUp";
    const string PLAYER_RUN_UP_RIGHT = "Player_RunUpRight";
    const string PLAYER_RUN_UP_LEFT = "Player_RunUpLeft";
    const string PLAYER_RUN_DOWN = "Player_RunDown";
    const string PLAYER_RUN_DOWN_RIGHT = "Player_RunDownRight";
    const string PLAYER_RUN_DOWN_LEFT = "Player_RunDownLeft";

    string lastSavedDirection;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (InputDirection.instance.GetDirection() != Vector2.zero)
        {
            DirectionSwitch(InputDirection.instance.GetDirectionString());
        }
        else
        {
            DirectionRelax();
        }
    }

    private void DirectionSwitch(string direction)
    {
        switch (direction)
        {
            case "Up":
                anim.Play(PLAYER_RUN_UP);
                break;
            case "UpRight":
                anim.Play(PLAYER_RUN_UP_RIGHT);
                break;
            case "UpLeft":
                anim.Play(PLAYER_RUN_UP_LEFT);
                break;
            case "Down":
                anim.Play(PLAYER_RUN_DOWN);
                break;
            case "DownRight":
                anim.Play(PLAYER_RUN_DOWN_RIGHT);
                break;
            case "DownLeft":
                anim.Play(PLAYER_RUN_DOWN_LEFT);
                break;
            case "Right":
                anim.Play(PLAYER_RUN_DOWN_RIGHT);
                break;
            case "Left":
                anim.Play(PLAYER_RUN_DOWN_LEFT);
                break;
            default:
                Debug.LogWarning("Direction not found");
                break;
        }

        lastSavedDirection = direction;
    }

    private void DirectionRelax()
    {
        switch (lastSavedDirection)
        {
            case "Up":
                anim.Play(PLAYER_RELAX_UP);
                break;
            case "UpRight":
                anim.Play(PLAYER_RELAX_UP_RIGHT);
                break;
            case "UpLeft":
                anim.Play(PLAYER_RELAX_UP_LEFT);
                break;
            case "Down":
                anim.Play(PLAYER_RELAX_DOWN);
                break;
            case "DownRight":
                anim.Play(PLAYER_RELAX_DOWN_RIGHT);
                break;
            case "DownLeft":
                anim.Play(PLAYER_RELAX_DOWN_LEFT);
                break;
            case "Right":
                anim.Play(PLAYER_RELAX_DOWN_RIGHT);
                break;
            case "Left":
                anim.Play(PLAYER_RELAX_DOWN_LEFT);
                break;
            default:
                anim.Play(PLAYER_RELAX_DOWN);
                break;
        }
    }
}

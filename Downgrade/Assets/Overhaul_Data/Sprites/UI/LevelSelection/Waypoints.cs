using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class Waypoints : MonoBehaviour, IAnimController
{
    AnimationHolder animHolder;
    List<AnimationClip> animationIDs;
    [SerializeField] UISelector selector;
    [SerializeField] GameObject eventSystem;
    [SerializeField] float speed = 1f;
    [SerializeField] float gizmoSize = 0.1f;
    [SerializeField] Transform[] waypoints;
    [SerializeField] int[] waypointsForLevels;
    Button currentButton;
    int currentWaypoint = 0;
    bool isMoving = false;

    private void Awake()
    {
        SetAnimHolder();
    }

    private void Update()
    {
        if (!isMoving) { PlayAnimation(0);}
        if (isMoving) { eventSystem.SetActive(false); PlayAnimation(1); return; }
        else if (!eventSystem.activeInHierarchy) { eventSystem.SetActive(true); currentButton.Select(); }

        if (selector == null || selector.GetCurrentButton() == null) return;

        currentButton = selector.GetCurrentButton();

        for (int i = 0; i < selector.GetManualButtons().Length; i++)
        {
            /*if (selector.GetManualButtons()[i].transform.position == selector.transform.position)
            {
                if (waypointsForLevels[i] != currentWaypoint)
                {
                    MoveToWaypoint(waypointsForLevels[i]);
                }
            }*/

            if (selector.GetCurrentButton() == currentButton && selector.GetManualButtons()[i].transform.position == selector.GetCurrentButton().transform.position)
            {
                if (waypointsForLevels[i] != currentWaypoint)
                {
                    MoveToWaypoint(waypointsForLevels[i]);
                }
            }
        }
    }

    private void MoveToWaypoint(int targetWaypoint)
    {
        isMoving = true;
        int nextStep = targetWaypoint > currentWaypoint ? 1 : -1;
        transform.DOMove(waypoints[currentWaypoint + nextStep].position, speed).OnComplete(() =>
        {
            currentWaypoint += nextStep;
            isMoving = false;
        });
    }

    #region AnimationController
    public void SetAnimHolder()
    {
        animHolder = GetComponent<AnimationHolder>();
        animHolder.Initialize(GetComponentInChildren<Animator>());
        animationIDs = animHolder.GetAnimationsIDs();
    }

    protected void PlayAnimation(int index, bool hasExitTime = false, bool bypassExitTime = false, bool canBeBypassed = false)
    {
        animHolder.GetAnimationController().PlayAnimation(animationIDs[index], null, hasExitTime, bypassExitTime, canBeBypassed);
    }

    protected bool IsAnimationDone()
    {
        return animHolder.GetAnimationController().isAnimationDone;
    }
    #endregion

    public Transform[] GetWaypoints() { return waypoints; }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (i == 0)
            {
                Gizmos.color = Color.yellow; Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.color = Color.red; Gizmos.DrawWireSphere(waypoints[i].position, gizmoSize);
            }
            else if (i == waypoints.Length - 1)
            {
                Gizmos.color = Color.green; Gizmos.DrawWireSphere(waypoints[i].position, gizmoSize);
            }
            else
            {
                Gizmos.color = Color.yellow; Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(waypoints[i].position, gizmoSize);
            }
        }
    }
}

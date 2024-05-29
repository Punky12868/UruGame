using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;

public class TransitionsTests : MonoBehaviour
{
    [SerializeField] private TransitionSettings transitionSettings;
    [SerializeField] private float delay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransitionManager.Instance().Transition(transitionSettings, delay);
        }
    }
}

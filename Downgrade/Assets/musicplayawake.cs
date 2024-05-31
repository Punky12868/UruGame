using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicplayawake : MonoBehaviour
{
    [SerializeField] int index;
    private void Awake()
    {
        // delay the awake
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }
    private void DelayedAwake()
    {
        // play music
        FindObjectOfType<AudioManager>().PlayMusic(index);
    }
}

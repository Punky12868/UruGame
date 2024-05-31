using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicplayawake : MonoBehaviour
{
    [SerializeField] int index;
    private void Awake()
    {
        // play music
        FindObjectOfType<AudioManager>().PlayMusic(index);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedText : MonoBehaviour
{
    [SerializeField] float lerpSpeed;
    [SerializeField] float newScale;

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(newScale, newScale, newScale), lerpSpeed * Time.deltaTime);
    }
}

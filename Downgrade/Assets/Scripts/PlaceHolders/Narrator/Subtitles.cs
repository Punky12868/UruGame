using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

public class Subtitles : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] TypewriterByCharacter typewriterByCharacter;
    [SerializeField] float clipLenghtDelay;

    public void DisplaySubtitles(string text, float clipLenght)
    {
        if (typewriterByCharacter == null)
        {
            typewriterByCharacter = GetComponent<TypewriterByCharacter>();
        }

        Destroy(parent, clipLenght + clipLenghtDelay);
        typewriterByCharacter.ShowText(text);
        typewriterByCharacter.StartShowingText();
    }
}

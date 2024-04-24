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

        Invoker.InvokeDelayed(DestroySub, clipLenght + clipLenghtDelay);
        
        typewriterByCharacter.ShowText(text);
        typewriterByCharacter.StartShowingText();
    }

    public void DisplayOnPlayingSubtitles(string text, float clipLenght)
    {
        if (typewriterByCharacter == null)
        {
            typewriterByCharacter = GetComponent<TypewriterByCharacter>();
        }

        Invoker.CancelInvoke(DestroySub);
        Invoker.InvokeDelayed(DestroySub, clipLenght + clipLenghtDelay);

        GetComponent<TMPro.TMP_Text>().text = "";
        typewriterByCharacter.ShowText(text);
        typewriterByCharacter.StartShowingText();
    }

    private void DestroySub()
    {
        Destroy(parent);
    }
}

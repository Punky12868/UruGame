using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueDowngrade : MonoBehaviour
{
    [TextArea]
    [SerializeField] string[] sentences;
    [SerializeField] float timeToWait = 3f;
    [SerializeField] GameObject introPanel;

    [SerializeField] UnityEvent onIntroStart;
    [SerializeField] UnityEvent onIntroEnd;
    int index = 0;

    [SerializeField] TypewriterByCharacter typewriter;
    private void Awake()
    {
        onIntroStart?.Invoke();
        ShowNexSentence();
    }
    public void ShowNexSentence()
    {
        if (index < sentences.Length)
        {
            Invoker.InvokeDelayed(NextSentence, timeToWait);
        }
        else
        {
            Invoker.InvokeDelayed(DeletePanel, timeToWait / 2);
        }
    }

    private void NextSentence()
    {
        typewriter.ShowText(sentences[index]);
        index++;
    }

    private void DeletePanel()
    {
        onIntroEnd?.Invoke();
        Destroy(introPanel);
    }
}

using Rewired;
using Febucci.UI;
using UnityEngine;
using UnityEngine.Events;

public class DialogueDowngrade : MonoBehaviour
{
    [TextArea]
    [SerializeField] string[] sentences;
    [SerializeField] float timeToWait = 3f;
    [SerializeField] GameObject introPanel;

    [SerializeField] TypewriterByCharacter typewriter;
    [SerializeField] UnityEvent onIntroStart;
    [SerializeField] UnityEvent onIntroEnd;
    int index = 0;
    bool skipped;
    Player input;

    private void Awake() { onIntroStart?.Invoke(); ShowNexSentence(); input = ReInput.players.GetPlayer(0); }
    private void NextSentence() { typewriter.ShowText(sentences[index]); index++; }
    private void DeletePanel() { onIntroEnd?.Invoke(); Destroy(introPanel); }
    public void ResetSkipStatus() { skipped = false; }

    private void Update()
    {
        if (input.GetAnyButtonDown() && !skipped && index <= sentences.Length - 2) { typewriter.SkipTypewriter(); skipped = true; }
    }

    public void ShowNexSentence()
    {
        if (index < sentences.Length) Invoker.InvokeDelayed(NextSentence, timeToWait);
        else Invoker.InvokeDelayed(DeletePanel, timeToWait / 2);
    }
}

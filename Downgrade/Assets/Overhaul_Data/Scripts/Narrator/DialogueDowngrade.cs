using Rewired;
using Febucci.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class DialogueDowngrade : MonoBehaviour
{
    [TextArea]
    [SerializeField] string[] sentences;
    [SerializeField] float timeToWait = 3f;
    [SerializeField] bool canSkip = true;
    [SerializeField] bool delayAwake = false;
    [SerializeField] bool hasSpecialEvent = false;
    [SerializeField] int specialEvent;
    [SerializeField] GameObject introPanel;
    [SerializeField] GameObject specialEventObject;

    [SerializeField] TMP_FontAsset oldFont;
    [SerializeField] float oldFontSize = 33;
    GameObject objct;

    [SerializeField] TypewriterByCharacter typewriter;
    [SerializeField] UnityEvent onIntroStart;
    [SerializeField] UnityEvent onIntroEnd;
    int index = 0;
    bool skipped;
    bool specialEventCalled;
    Player input;

    private void Awake() { if (delayAwake) Invoker.InvokeDelayed(DelayedAwake, 0.2f); else { DelayedAwake(); } }
    private void DelayedAwake()
    {
        AudioManager.instance.SetLowPass(true);
        if (GameManager.Instance.GetIntroText()) { DeletePanel(); return; }
        onIntroStart?.Invoke(); ShowNexSentence(); GameManager.Instance.SetIntroText(true); if (canSkip) input = ReInput.players.GetPlayer(0);
    }
    private void NextSentence() { typewriter.ShowText(sentences[index]); index++; }
    private void DeletePanel() { onIntroEnd?.Invoke(); Destroy(introPanel); }
    public void ResetSkipStatus() { skipped = false; }
    public void DestroyGameObject() { Destroy(gameObject); }

    private void Update()
    {
        if(canSkip){ if (input.GetAnyButtonDown() && !skipped && index <= sentences.Length - 2) { typewriter.SkipTypewriter(); skipped = true; } }

        if (!hasSpecialEvent) return;
        if (specialEventCalled) { if (index > specialEvent) { ResetEvent(); } return; }
        if (index == specialEvent) SpecialEventTrigger();
    }

    public void ShowNexSentence()
    {
        if (index < sentences.Length) Invoker.InvokeDelayed(NextSentence, timeToWait);
        else Invoker.InvokeDelayed(DeletePanel, timeToWait / 2);
    }

    private void SpecialEventTrigger()
    {
        specialEventCalled = true;
        typewriter.GetComponent<TMP_Text>().font = oldFont;
        objct = Instantiate(specialEventObject, transform);
        objct.GetComponent<TypewriterByCharacter>().ShowText("<shake>" + sentences[index - 2]);
        //objct.GetComponent<TMPro.TMP_Text>().text = sentences[index];
    }

    private void ResetEvent()
    {
        objct.SetActive(false);
        typewriter.GetComponent<TMP_Text>().fontSize = oldFontSize;
        Debug.Log("Special event reset");
        specialEventCalled = false;
    }
}

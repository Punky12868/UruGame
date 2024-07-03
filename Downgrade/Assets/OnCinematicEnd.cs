using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using UnityEngine.Events;

public class OnCinematicEnd : MonoBehaviour
{
    [SerializeField] bool unPauseOnEnd;
    [SerializeField] int dialogIndex;
    [SerializeField] TypewriterByCharacter typewriter;
    [SerializeField] [TextArea] string [] dialogs;
    [SerializeField] UnityEvent onCinematicEnd;
    public bool cinematicEnded;
    bool cinematicCalled;
    int lastDialogIndex;

    private void Awake()
    {
        Invoker.InvokeDelayed(CustomAwake, 0.1f);
    }

    private void CustomAwake()
    {
        onCinematicEnd.AddListener(SetCinematicEnd);
        if (GameManager.Instance.GetBossCinematic()) onCinematicEnd?.Invoke();
    }

    private void Update()
    {
        if (dialogIndex != lastDialogIndex)
        {
            lastDialogIndex = dialogIndex;
            typewriter.ShowText(dialogs[dialogIndex]);
        }

        if (!cinematicEnded || cinematicCalled) return;
        cinematicCalled = true;
        onCinematicEnd?.Invoke();
    }

    public void SetCinematicEnd()
    {
        GameManager.Instance.SetBossCinematic(true);
    }
}

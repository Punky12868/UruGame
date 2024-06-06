using UnityEngine;
using UnityEngine.Events;

public class MainMenuTransition : MonoBehaviour
{
    [SerializeField] private AnimationClip transitionInClip;
    [SerializeField] private AnimationClip transitionOutClip;

    [SerializeField] private GameObject subject;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private float transitionDuration;

    [SerializeField] private UnityEvent OnTransitionInStart;
    [SerializeField] private UnityEvent OnTransitionInComplete;

    [SerializeField] private UnityEvent OnTransitionOutStart;
    [SerializeField] private UnityEvent OnTransitionOutComplete;

    private Animator animator;
    string clipName;
    //bool isTransitioning;
    bool shoulBeListening;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        FindObjectOfType<MenuController>().OnLoadHistory += TransitionOutOnLoad;
        FindObjectOfType<MenuController>().OnLoadHistory += ResetListening;
    }

    public void SetTransition(bool value)
    {
        //if (isTransitioning) return;

        clipName = value ? transitionInClip.name : transitionOutClip.name;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                animator.Play(clipName);
                OnTransitionStart(value, transitionDuration);
                //isTransitioning = true;
                break;
            }
        }
    }

    private void OnTransitionStart(bool value, float length)
    {
        Invoker.InvokeDelayed(OnTransitionComplete, length);
        Debug.Log("Transition started");

        if (value) OnTransitionInStart.Invoke();
        else OnTransitionOutStart.Invoke();
    }

    private void OnTransitionComplete()
    {
        Debug.Log("Transition completed");
        //isTransitioning = false;

        if (clipName == transitionInClip.name) OnTransitionInComplete.Invoke();
        else OnTransitionOutComplete.Invoke();
    }

    public void ActivateGameObject(GameObject subject) 
    {
        if (!shoulBeListening) return;
        subject.SetActive(subject.activeSelf ? false : true);
    }

    private void TransitionOutOnLoad()
    {
        if (!shoulBeListening) return;
        SetTransition(true);
    }

    public void SetListening(bool value)
    {
        shoulBeListening = value;
    }

    public void ResetListening()
    {
        Invoker.InvokeDelayed(ListeningPatch, transitionDuration + 0.01f);
    }

    private void ListeningPatch()
    {
        if (shoulBeListening) shoulBeListening = false;
    }

    public bool GetListening() { return shoulBeListening; }
}

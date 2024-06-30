using UnityEngine;

public class SongOnAwake : MonoBehaviour
{
    [SerializeField] private bool delayAwake = true;
    [SerializeField] private int songId;
    bool initialized = false;
    bool itsOk = false;
    private void Awake() { if (delayAwake) { Invoker.InvokeDelayed(DelayedAwake, 0.2f); return; } DelayedAwake(); }
    private void DelayedAwake() { if (AudioManager.instance.GetCurrentMusicIndex() != songId) AudioManager.instance.PlayMusic(songId); initialized = true; }

    private void Update()
    {
        if (itsOk) return;
        if (initialized && AudioManager.instance.GetCurrentMusicIndex() != songId) AudioManager.instance.SetCurrentMusicIndex(songId);
        else itsOk = true;
    }
}

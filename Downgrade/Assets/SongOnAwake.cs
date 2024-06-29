using UnityEngine;

public class SongOnAwake : MonoBehaviour
{
    [SerializeField] private bool delayAwake = true;
    [SerializeField] private int songId;
    private void Awake() { if (delayAwake) { Invoker.InvokeDelayed(DelayedAwake, 0.2f); return; } DelayedAwake(); }
    private void DelayedAwake() { if (AudioManager.instance.GetCurrentMusicIndex() != songId) AudioManager.instance.PlayMusic(songId); }
}

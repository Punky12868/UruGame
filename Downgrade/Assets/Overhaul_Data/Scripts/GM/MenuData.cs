using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuData : MonoBehaviour
{
    public SelectedDowngrade dgSelected;

    [SerializeField]AudioVolume audioVolume;
    [SerializeField] AudioMixer mixer;
    public Slider[] sliders;
    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.1f);
    }

    private void DelayedAwake()
    {
        dgSelected = SimpleSaveLoad.Instance.LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade");
    }
}

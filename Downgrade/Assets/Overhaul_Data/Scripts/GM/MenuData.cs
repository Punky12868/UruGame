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
        dgSelected = FindObjectOfType<SimpleSaveLoad>().LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class MenuData : MonoBehaviour
{
    bool subtitles;
    bool lifeBar;

    SelectedDowngrade dgSelected;
    string savePath;
    [SerializeField]AudioVolume audioVolume;
    [SerializeField] AudioMixer mixer;
    [SerializeField] SimpleSaveLoad simpleSave;
    DowngradeSystem dgSys;
    public Slider[] sliders;
    private void Start()
    {
        audioVolume.SetMixer(mixer);
        Invoke("SetSavedOptions", 0.2f);
        DowngradeSystem dgSys = FindObjectOfType<DowngradeSystem>();
        if (dgSys != null)
        {
           // dgSelected = dgSys.dg;
        }


    }

    void SetSavedOptions()
    {
        audioVolume.SetMusicVolume(sliders[0].value);
        audioVolume.SetSFXVolume(sliders[1].value);
        audioVolume.SetVoiceVolume(sliders[2].value);
    }
    public void SaveMusic()
    {
        audioVolume.SetMusicVolume(sliders[0].value);
    }
    public void SaveSFX()
    {
        audioVolume.SetSFXVolume(sliders[1].value);
    }
    public void SaveVoice()
    {
        audioVolume.SetVoiceVolume(sliders[2].value);
    }
}

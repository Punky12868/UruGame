    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuData : MonoBehaviour
{
    bool subtitles;
    bool lifeBar;

    public SelectedDowngrade dgSelected;

    [SerializeField]AudioVolume audioVolume;
    [SerializeField] AudioMixer mixer;
    public Slider[] sliders;
    private void Start()
    {
        audioVolume.SetMixer(mixer);
        Invoke("SetSavedOptions", 0.2f);

        dgSelected = FindObjectOfType<SimpleSaveLoad>().LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade");
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

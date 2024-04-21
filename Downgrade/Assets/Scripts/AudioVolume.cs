using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolume : MonoBehaviour
{
    AudioMixer mixer;

    float masterVol;
    float musicVol;
    float sfxVol;
    float voiceVol;

    public void SetMixer(AudioMixer mixer)
    {
        this.mixer = mixer;

        masterVol = PlayerPrefs.GetFloat("MasterVol", 1);
        musicVol = PlayerPrefs.GetFloat("MusicVol", 1);
        sfxVol = PlayerPrefs.GetFloat("SFXVol", 1);
        voiceVol = PlayerPrefs.GetFloat("VoiceVol", 1);
    }

    public void SetMasterVolume(float sliderValue)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
        masterVol = sliderValue;
        PlayerPrefs.SetFloat("MasterVol", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        musicVol = sliderValue;
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        sfxVol = sliderValue;
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }

    public void SetVoiceVolume(float sliderValue)
    {
        mixer.SetFloat("VoiceVol", Mathf.Log10(sliderValue) * 20);
        voiceVol = sliderValue;
        PlayerPrefs.SetFloat("VoiceVol", sliderValue);
    }
}

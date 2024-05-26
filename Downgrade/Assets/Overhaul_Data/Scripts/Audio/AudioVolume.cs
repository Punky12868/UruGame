using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;

    [SerializeField] private int lowestDeciblesBeforeMute = -20;
    private bool listening;
    private void Awake() { Invoker.InvokeDelayed(CustomAwake, 0.1f); }

    private void CustomAwake()
    {
        musicSlider.value = LoadVolume("MusicVol");
        sfxSlider.value = LoadVolume("SFXVol");
        voiceSlider.value = LoadVolume("VoiceVol");

        mixer.SetFloat("MusicVol", SetVolume((int)musicSlider.value));
        mixer.SetFloat("SFXVol", SetVolume((int)sfxSlider.value));
        mixer.SetFloat("VoiceVol", SetVolume((int)voiceSlider.value));

        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider); });
        sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider); });
        voiceSlider.onValueChanged.AddListener(delegate { SetVoiceVolume(voiceSlider); });

        listening = true;
    }

    public void SetMusicVolume(Slider sliderValue)
    {
        mixer.SetFloat("MusicVol", SetVolume((int)sliderValue.value));
        SaveVolume("MusicVol", (int)sliderValue.value);
    }

    public void SetSFXVolume(Slider sliderValue)
    {
        mixer.SetFloat("SFXVol", SetVolume((int)sliderValue.value));
        SaveVolume("SFXVol", (int)sliderValue.value);
    }

    public void SetVoiceVolume(Slider sliderValue)
    {
        mixer.SetFloat("VoiceVol", SetVolume((int)sliderValue.value));
        SaveVolume("VoiceVol", (int)sliderValue.value);
    }

    private float SetVolume(int sliderVolume)
    {
        int volume = sliderVolume * 10;
        float adjustedVolume = lowestDeciblesBeforeMute + (-lowestDeciblesBeforeMute / 5 * volume / 20);
        if (volume == 0) adjustedVolume = -100;
        return adjustedVolume;
    }

    private void SaveVolume(string key, int value)
    {
        if (!listening) return;
        FindObjectOfType<SimpleSaveLoad>().SaveData(FileType.Config, key, value);
    }

    private float LoadVolume(string key)
    {
        return FindObjectOfType<SimpleSaveLoad>().LoadData<int>(FileType.Config, key, 5);
    }
}

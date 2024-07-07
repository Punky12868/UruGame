using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private bool playOnAwake;
    [SerializeField] private float musicLoopFadeThreshold = 0.5f;
    [SerializeField] private int musicIndex;

    [SerializeField] private float normalLowPassFreq = 22000.00f;
    [SerializeField] private float pausedLowPassFreq = 500.00f;
    [SerializeField] private float lowPassFreqLerpSpeed = 0.5f;
    private float lowPassValue;
    bool lowpass;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource voice;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] voiceClips;

    private int currentMusicIndex;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        music.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        sfx.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        voice.outputAudioMixerGroup = mixer.FindMatchingGroups("Voice")[0];

        if (playOnAwake) { PlayMusic(musicIndex); currentMusicIndex = musicIndex; }
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        lowPassValue = Mathf.Lerp(lowPassValue, lowpass ? pausedLowPassFreq : normalLowPassFreq, lowPassFreqLerpSpeed * Time.unscaledDeltaTime);
        mixer.SetFloat("MusicLowPassFreq", lowPassValue);

        if (music.clip == null) return;
        if (music.time >= music.clip.length - musicLoopFadeThreshold) StartCoroutine(FadeOutMusic(currentMusicIndex));
    }

    public void PlayMusic(int musicIndex)
    {
        if (music.isPlaying)
        {
            if (music.clip == musicClips[musicIndex]) return;
            StartCoroutine(FadeOutMusic(musicIndex));
        }
        else
        {
            music.clip = musicClips[musicIndex];
            currentMusicIndex = musicIndex;
            music.Play();
        }
    }

    public void StopMusic() { StartCoroutine(FadeOutStopMusic()); }
    public void PlaySFX(int sfxIndex) { sfx.PlayOneShot(sfxClips[sfxIndex]); }
    public void PlayCustomSFX(AudioClip clip) { sfx.PlayOneShot(clip); }
    public void PlayCustomSFX(AudioClip clip, AudioSource source)
    { source.outputAudioMixerGroup = sfx.outputAudioMixerGroup; source.PlayOneShot(clip); }

    public void PlayVoice(AudioClip voiceClip)
    {
        if (voice.isPlaying) voice.Stop();
        voice.PlayOneShot(voiceClip);
    }

    public void StopSFX() { sfx.Stop(); }

    public void StopAllAudio() { music.Stop(); sfx.Stop(); }

    private IEnumerator FadeOutMusic(int musicIndex)
    {
        while (music.volume > 0)
        {
            music.volume -= Time.unscaledDeltaTime;
            yield return null;
        }

        music.clip = musicClips[musicIndex];
        currentMusicIndex = musicIndex;
        music.Play();

        while (music.volume < 1)
        {
            music.volume += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutStopMusic()
    {
        while (music.volume > 0)
        {
            music.volume -= Time.unscaledDeltaTime;
            yield return null;
        }

        music.Stop();
        music.clip = null;
        music.volume = 1;
    }

    public float GetCurrentMusicIndex()
    {
        if (music.clip == null) return 999;
        return currentMusicIndex;
    }

    public void SetCurrentMusicIndex(int index) { currentMusicIndex = index; PlayMusic(index); }
    private void DoLowPass() 
    { 
        if (GameManager.Instance.IsGamePaused())
        {
            lowpass = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex != GameManager.Instance.GetDowngradeSceneIndex() && !GameManager.Instance.IsGamePaused())
        {
            lowpass = false;
        }
    }
    public void SetLowPass(bool value) { lowpass = value; }
}

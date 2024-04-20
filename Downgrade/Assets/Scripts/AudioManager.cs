using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private bool playOnAwake;
    [ShowIf("playOnAwake", true , true)][SerializeField] private int musicIndex;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sfx;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        music.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        sfx.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];

        if (playOnAwake)
        {
            PlayMusic(musicIndex);
        }

        DontDestroyOnLoad(gameObject);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (music.clip == musicClips[0])
            {
                PlayMusic(1);
            }
            else
            {
                PlayMusic(0);
            }
        }
    }*/

    public void PlayMusic(int musicIndex)
    {
        if (music.isPlaying)
        {
            if (music.clip == musicClips[musicIndex])
            {
                return;
            }
            StartCoroutine(FadeOutMusic(musicIndex));
        }
        else
        {
            music.clip = musicClips[musicIndex];
            music.Play();
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOutStopMusic());
    }

    public void PlaySFX(int sfxIndex)
    {
        sfx.PlayOneShot(sfxClips[sfxIndex]);
    }

    public void PlayCustomSFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlayCustomSFX(AudioClip clip, AudioSource source)
    {
        source.outputAudioMixerGroup = sfx.outputAudioMixerGroup;
        source.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfx.Stop();
    }

    public void StopAllAudio()
    {
        music.Stop();
        sfx.Stop();
    }

    private IEnumerator FadeOutMusic(int musicIndex)
    {
        while (music.volume > 0)
        {
            music.volume -= Time.deltaTime;
            yield return null;
        }

        music.clip = musicClips[musicIndex];
        music.Play();

        while (music.volume < 1)
        {
            music.volume += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutStopMusic()
    {
        while (music.volume > 0)
        {
            music.volume -= Time.deltaTime;
            yield return null;
        }

        music.Stop();
        music.clip = null;
        music.volume = 1;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AGOUtils;

public class SoundBoard : ExtensibleSingleton<SoundBoard> {
    
    [SerializeField] List<Audio> audios;
    private Dictionary<Audio.AudioEnum, Coroutine> fadeCoroutines = new Dictionary<Audio.AudioEnum, Coroutine>();
    
    void Start() {
        audios = audios.Select(audio => {
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.volume = audio.volume;
            tempSource.clip = audio.clip;
            tempSource.loop = true;
            audio.source = tempSource;
            return audio;
        }).ToList();

        FadeIn(Audio.AudioEnum.LonelinessSFX, 5f);
    }

    public void Play(Audio.AudioEnum name) {
        Audio song = Get(name);
        if (song.clip != null && !song.source.isPlaying) {
            song.source.Play();
        }
    }

    public void Stop(Audio.AudioEnum name) {
        Audio song = Get(name);
        if (song.clip != null && song.source.isPlaying) {
            song.source.Pause();
        }

    }

    public void PlayOneShot(Audio.AudioEnum name) {
        Audio audio = Get(name);
        if (audio.clip != null) {
            audio.source.PlayOneShot(audio.clip, audio.volume);
        }
    }

    public void BlendBetween(Audio.AudioEnum from, Audio.AudioEnum to, float duration) {
        FadeOut(from, duration);
        FadeIn(to, duration);
    }
    public void FadeIn(Audio.AudioEnum name, float duration) {
        Audio audio = Get(name);
        if (audio.fadeInCr == null) { // Ensure no FadeIn is running
            if (audio.fadeOutCr != null) {
                StopCoroutine(audio.fadeOutCr);
                audio.fadeOutCr = null;
            }
            audio.fadeInCr = StartCoroutine(FadeInCR(name, duration));
        }
    }

    public void FadeOut(Audio.AudioEnum name, float duration) {
        Audio audio = Get(name);
        if (audio.fadeOutCr == null) { // Ensure no FadeOut is running
            if (audio.fadeInCr != null) {
                StopCoroutine(audio.fadeInCr);
                audio.fadeInCr = null;
            }
            audio.fadeOutCr = StartCoroutine(FadeOutCR(name, duration));
        }
    }

    IEnumerator FadeOutCR(Audio.AudioEnum name, float duration) {
        Audio audio = Get(name);
        float startVolume = audio.source.volume;
        float fadeSpeed = startVolume / duration;

        while (audio.source.volume > 0f) {
            audio.source.volume = Mathf.Clamp(audio.source.volume - fadeSpeed * Time.deltaTime, 0f, startVolume);
            yield return null;
        }
        Stop(name);
        audio.fadeOutCr = null; // Explicitly set to null when finished
    }

    IEnumerator FadeInCR(Audio.AudioEnum name, float duration) {
        Audio audio = Get(name);
        float targetVolume = audio.volume;
        float fadeSpeed = targetVolume / duration;
        Play(name);

        while (audio.source.volume < targetVolume) {
            audio.source.volume = Mathf.Clamp(audio.source.volume + fadeSpeed * Time.deltaTime, 0f, targetVolume);
            yield return null;
        }
        audio.fadeInCr = null; // Explicitly set to null when finished
    }

    Audio Get(Audio.AudioEnum name) {
        return audios.Find(a => a.name == name);
    }
}

[Serializable]
public class Audio {
    public float volume;
    public AudioClip clip;
    public AudioEnum name;
    [HideInInspector] public AudioSource source;
    [HideInInspector] public Coroutine fadeOutCr;
    [HideInInspector] public Coroutine fadeInCr;
    public enum AudioEnum {
        LazerHackSFX,
        EnemyShootSFX,
        LonelinessSFX,
        ShutDownSFX,
        WarningSFX,
    }
}
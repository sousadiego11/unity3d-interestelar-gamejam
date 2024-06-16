using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundBoard : ExtensibleSingleton<SoundBoard> {
    
    [SerializeField] List<Audio> audios;
    
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

    public void BlendBetween(Audio.AudioEnum from, Audio.AudioEnum to, float speed) {
        Singleton.FadeOut(from, speed);
        Singleton.FadeIn(to, speed);
    }

    public void FadeIn(Audio.AudioEnum name, float speed) {
        StartCoroutine(FadeInCR(name, speed));
    }

    public void FadeOut(Audio.AudioEnum name, float speed) {
        StartCoroutine(FadeOutCR(name, speed));
    }

    IEnumerator FadeOutCR(Audio.AudioEnum name, float speed) {
        Audio audio = Get(name);
        if (!audio.isFadingOut) {
            while (audio.source.volume > 0f) {
                audio.source.volume = Mathf.Clamp(audio.source.volume - speed * Time.deltaTime, 0f, audio.volume);
                yield return null;
            }
            Stop(name);
            audio.isFadingOut = false;
        }
    }

    IEnumerator FadeInCR(Audio.AudioEnum name, float speed) {
        Audio audio = Get(name);
        if (!audio.source.isPlaying) {
            audio.source.volume = 0f;
            Play(name);
            while (audio.source.volume < audio.volume) {
                audio.source.volume = Mathf.Clamp(audio.source.volume + speed * Time.deltaTime, 0f, audio.volume);
                yield return null;
            }
        }
    }

    Audio Get(Audio.AudioEnum name) {
        return audios.Find(a => a.name == name);
    }
}

[Serializable]
public struct Audio {
    public float volume;
    public AudioClip clip;
    public AudioEnum name;
    [HideInInspector] public AudioSource source;
    [HideInInspector] public bool isFadingOut;
    public enum AudioEnum {
        LazerHackSFX,
        EnemyShootSFX,
        LonelinessSFX,
        ShutDownSFX,
        WarningSFX,
    }
}
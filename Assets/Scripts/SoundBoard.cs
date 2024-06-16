using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : ExtensibleSingleton<SoundBoard> {
    
    [SerializeField] List<Audio> audios;
    
    void Start() {
        List<Audio> temp = new();
        foreach (Audio audio in audios) {
            temp.Add(new Audio {
                clip = audio.clip,
                name = audio.name,
                source = gameObject.AddComponent<AudioSource>(),
                volume = audio.volume
            });
        }
        audios = temp;
        Play(Audio.AudioEnum.LonelinessSFX);
    }

    public void Play(Audio.AudioEnum name) {
        Audio song = Get(name);
        if (song.clip != null && !song.source.isPlaying) {
            song.source.clip = song.clip;
            song.source.loop = true;
            song.source.Play();
        }
    }

    public void Stop(Audio.AudioEnum name) {
        Audio song = Get(name);
        if (song.source.isPlaying) {
            song.source.Pause();
        }

    }

    public void PlayOneShot(Audio.AudioEnum name) {
        Audio audio = Get(name);
        if (audio.clip != null) {
            audio.source.PlayOneShot(audio.clip, audio.volume);
        }
    }

    Audio Get(Audio.AudioEnum name) {
        return audios.Find(a => a.name == name);
    }
}

[Serializable]
public struct Audio {
    public AudioClip clip;
    [HideInInspector] public AudioSource source;
    public AudioEnum name;
    public float volume;
    public enum AudioEnum {
        LazerHackSFX,
        EnemyShootSFX,
        LonelinessSFX,
        ShutDownSFX,
    }
}
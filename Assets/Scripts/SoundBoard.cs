using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : MonoBehaviour {
    
    [SerializeField] List<Audio> audioClips;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource lazerHackSource;

    public static SoundBoard Instance {get; private set;}

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    
    private void Start() {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic() {
        Audio backgroundMusic = audioClips.Find(a => a.name == Audio.AudioEnum.LonelinessSFX);
        if (backgroundMusic.clip != null) {
            musicSource.clip = backgroundMusic.clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayOneShot(Audio.AudioEnum name) {
        Audio audio = audioClips.Find(a => a.name == name);
        if (audio.clip != null) {
            sfxSource.PlayOneShot(audio.clip, audio.volume);
        }
    }

    public void PlayLazerHackSFX() {
        Audio lazerHackAudio = audioClips.Find(a => a.name == Audio.AudioEnum.LazerHackSFX);
        if (lazerHackAudio.clip != null && !lazerHackSource.isPlaying) {
            lazerHackSource.clip = lazerHackAudio.clip;
            lazerHackSource.volume = lazerHackAudio.volume;
            lazerHackSource.Play();
        }
    }

    public void PauseLazerHackSFX() {
        if (lazerHackSource.isPlaying) {
            lazerHackSource.Pause();
        }
    }
}

[Serializable]
public struct Audio {
    public AudioClip clip;
    public AudioEnum name;
    public float volume;
    public enum AudioEnum
    {
        LazerHackSFX,
        EnemyShootSFX,
        LonelinessSFX
    }
}
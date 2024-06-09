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
        if (backgroundMusic.audioClip != null) {
            musicSource.clip = backgroundMusic.audioClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayEnemyShootSFX() {
        Audio enemyShootAudio = audioClips.Find(a => a.name == Audio.AudioEnum.EnemyShootSFX);
        if (enemyShootAudio.audioClip != null) {
            sfxSource.PlayOneShot(enemyShootAudio.audioClip, 1);
        }
    }

    public void PlayLazerHackSFX() {
        Audio lazerHackAudio = audioClips.Find(a => a.name == Audio.AudioEnum.LazerHackSFX);
        if (lazerHackAudio.audioClip != null && !lazerHackSource.isPlaying) {
            lazerHackSource.clip = lazerHackAudio.audioClip;
            lazerHackSource.volume = 1;
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
    public AudioClip audioClip;
    public AudioEnum name;
    public enum AudioEnum
    {
        LazerHackSFX,
        EnemyShootSFX,
        LonelinessSFX
    }
}
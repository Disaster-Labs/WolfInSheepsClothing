// ---------------------------------------
// Creation Date: 04/02/24
// Author: Jason Perez
// Modified By: Jason Perez
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public static enum WolfStatus { Undetected, Suspicious, Identified };

public class SoundManager : MonoBehaviour
{
    // Events
    //[SerializeField] private WolfMovement wolf;
    public event EventHandler<Boolean> WolfWalking;
    public event EventHandler<Boolean> WolfRunning;
    public class MusicEventArgs { public WolfStatus currentState; public WolfStatus newState; }
    public event EventHandler<MusicEventArgs> UpdateBGMusic;
    public event EventHandler<Boolean> WolfNearSheep;
    public event EventHandler SheepEaten;
    public event EventHandler ShepherdSuspicious;
    public event EventHandler ShepherdAlerted;
    public event EventHandler ShepherdHunting;

    // Audio Fade
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float duration;
    private String undetectedExposedParam = "UndetectedVol";
    private String suspiciousExposedParam = "SuspiciousVol";

    // Audio Sources
    private AudioSource audioSrc;
    [SerializeField] private AudioSource suspiciousAudioSrc;
    [SerializeField] private AudioSource backgroundSrc;
    [SerializeField] private AudioSource wolfAudio;

    // Audio Clips
    // Background Music
    [SerializeField] private AudioClip undetected;

    // Wolf
    [SerializeField] private AudioClip wolfWalkingAudio;

    void Awake() {
        PlayBGMusic += UpdateBGMusic;
        WolfWalking += PlayWolfWalkingAudio;
    }

    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    void Update() {
        if (Input.GetKeyDown("w")) {
            Debug.Log("Wolf started walking");
            WolfWalking.Invoke(this, true);
        }

        if (Input.GetKeyUp("w")) {
            Debug.Log("Wolf stopped walking");
            WolfWalking.Invoke(this, false);
        }

        if (Input.GetKeyDown("u")) {
            Debug.Log("Playing undetected Music");
            PlayBGMusic.Invoke(this, true);
        }

        if (Input.GetKeyDown("f")) {
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                    undetectedExposedParam,
                                                    duration,
                                                    0.0f));

            suspiciousAudioSrc.time = 25.0f;
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                    suspiciousExposedParam,
                                                    duration,
                                                    1.0f));
        }
    }

    public void UpdateBGMusic(object sender, Boolean playMusic) {
        if (playMusic) {
            backgroundSrc.clip = undetected;
            backgroundSrc.Play();
        }
    }

    public void PlayWolfWalkingAudio(object sender, Boolean isWalking ) {
        if (isWalking) {
            wolfAudio.clip = wolfWalkingAudio;
            wolfAudio.Play();
        } else {
            wolfAudio.Stop();
        }
    }
}

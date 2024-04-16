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

public enum WolfStatus { Undetected, Suspicious, Identified };

public class SoundManager : MonoBehaviour
{
    // Events
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
    [Header("Audio Fade")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float duration;
    private String undetectedExposedParam = "UndetectedVol";
    private String suspiciousExposedParam = "SuspiciousVol";
    private String identifiedExposedParam = "IdentifiedVol";

    // Audio Sources
    private AudioSource audioSrc;
    [Header("Background Music")]
    [SerializeField] private AudioSource undetectedAudioSrc;
    [SerializeField] private AudioSource suspiciousAudioSrc;
    [SerializeField] private AudioSource identifiedAudioSrc;
    [SerializeField] private AudioSource wolfAudio;

    // Audio Clips
    // Wolf
    [Header("Wolf Audio")]
    [SerializeField] private AudioClip wolfWalkingAudio;
    [SerializeField] private AudioClip wolfRunningAudio;

    // Sheep
    [Header("Sheep Audio")]
    [SerializeField] private AudioClip[] sheepBaahhs;
    [SerializeField] private AudioClip sheepDeath;

    // Shepherd
    [Header("Shepherd Audio")]
    [SerializeField] private AudioClip shepherdSuspicious;

    private bool playSheepSound = false;
    private bool wolfBeingHunted = false;

    void Awake() {
        UpdateBGMusic += PlayBGMusic;

        WolfWalking += PlayWolfWalkingAudio;
        WolfRunning += PlayWolfRunningAudio;
WolfNearSheep += PlaySheepBaahhAudio;
        SheepEaten += PlaySheepEatenAudio;

        ShepherdSuspicious += PlayShepherdSuspiciousAudio;
        ShepherdAlerted += PlayShepherdAlertedAudio;
        ShepherdHunting += PlayShepherdHuntingAudio;
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            Debug.Log("Wolf started running");
            WolfRunning.Invoke(this, true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            Debug.Log("Wolf stopped running");
            WolfRunning.Invoke(this, false);
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

    public void PlayBGMusic(object sender, MusicEventArgs e) {

    }

    public void PlayWolfWalkingAudio(object sender, Boolean isWalking ) {
        if (isWalking) {
            wolfAudio.clip = wolfWalkingAudio;
            wolfAudio.Play();
        } else {
            wolfAudio.Stop();
        }
    }

    public void PlayWolfRunningAudio(object sender, Boolean isRunning ) {
        if (isRunning) {
            wolfAudio.clip = wolfRunningAudio;
            wolfAudio.Play();
        } else {
            wolfAudio.Stop();
        }
    }

    public void PlaySheepBaahhAudio(object sender, Boolean wolfNearSheep) {
        playSheepSound = wolfNearSheep;
        if (playSheepSound) {
            int i = UnityEngine.Random.Range(0, sheepBaahhs.Length);
            audioSrc.PlayOneShot(sheepBaahhs[i]);
        }
    }

    public void PlaySheepEatenAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(sheepDeath);
    }

    public void PlayShepherdSuspiciousAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(shepherdSuspicious);

    }

    public void PlayShepherdAlertedAudio(object sender, EventArgs e) {

    }

    public void PlayShepherdHuntingAudio(object sender, EventArgs e) {

    }
}

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
    public class WolfMoveEventArgs { public bool isWalking = false; public bool isRunning = false; }
    public event EventHandler<WolfMoveEventArgs> WolfMoving;
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
    [SerializeField] private AudioClip shepherdAlerted;
    [SerializeField] private AudioClip shepherdHunting;

    private bool isWalking = false;
    private bool playSheepSound = false;
    private bool wolfBeingHunted = false;

    void Awake() {
        UpdateBGMusic += PlayBGMusic;

        WolfMoving += PlayWolfMovingAudio;
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
            isWalking = true;
            WolfMoving.Invoke(this, new WolfMoveEventArgs {isWalking = true});
        }

        if (Input.GetKeyUp("w")) {
            Debug.Log("Wolf stopped walking");
            isWalking = false;
            WolfMoving.Invoke(this, new WolfMoveEventArgs {});
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            Debug.Log("Wolf started running");
            WolfMoving.Invoke(this, new WolfMoveEventArgs {isRunning = true});
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            Debug.Log("Wolf stopped running");
            WolfMoving.Invoke(this, new WolfMoveEventArgs {isWalking = isWalking});
        }

        if (Input.GetKeyDown("1")) {
            Debug.Log("Undetected to Suspicious");
            UpdateBGMusic.Invoke(this, new MusicEventArgs {currentState = WolfStatus.Undetected, newState = WolfStatus.Suspicious});
        }
    }

    public void PlayBGMusic(object sender, MusicEventArgs e) {
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                e.currentState.ToString(),
                                                duration,
                                                0.0f));

        //suspiciousAudioSrc.time = 25.0f;
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                e.newState.ToString(),
                                                duration,
                                                1.0f));
    }

    public void PlayWolfMovingAudio(object sender, WolfMoveEventArgs e) {
        if (e.isWalking) {
            wolfAudio.clip = wolfWalkingAudio;
            wolfAudio.Play();
        } else if (e.isRunning) {
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
        audioSrc.PlayOneShot(shepherdAlerted);
    }

    public void PlayShepherdHuntingAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(shepherdHunting);
    }
}

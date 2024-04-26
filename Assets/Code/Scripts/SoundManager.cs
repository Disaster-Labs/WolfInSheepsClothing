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
    private PauseManager pauseManager;
    private WolfMovement wolfMovement; // wolfMovement.WolfMoving
    private Wolf wolf; // wolf.UpdateBGMusic wolf.SheepEaten
    private AlertSheep alertSheep; //alertSheep.WolfNearSheep

    // public event EventHandler ShepherdSuspicious;
    // public event EventHandler ShepherdAlerted;
    private Shepherd[] shepherds; //shepherds.ShepherdHunting

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
    [SerializeField] private AudioClip wolfEatingAudio;

    // Sheep
    [Header("Sheep Audio")]
    [SerializeField] private AudioClip[] sheepBaahhs;
    [SerializeField] private AudioClip sheepDeath;
    private float sheepAudioTimer = 3.0f;
    private float sheepMinDelay = 3.0f;
    private float sheepMaxDelay = 5.0f;

    // Shepherd
    [Header("Shepherd Audio")]
    [SerializeField] private AudioClip shepherdSuspicious;
    [SerializeField] private AudioClip shepherdAlerted;
    [SerializeField] private AudioClip shepherdHunting;
    [SerializeField] private AudioClip rifleShot;
    private float shepherdAudioTimer = 0.0f;
    private float shepherdMinDelay = 10.0f;
    private float shepherdMaxDelay = 15.0f;

    // private bool isWalking = false;
    private WolfStatus currentState = WolfStatus.Undetected;
    private bool playSheepSound = false;
    private bool wolfBeingHunted = false;
    private float oneShotVol = 0.5f;

    // private bool testToggle = false;

    void Awake() {
        pauseManager = FindObjectOfType<PauseManager>();
        wolf = FindObjectOfType<Wolf>();
        wolfMovement = FindObjectOfType<WolfMovement>();
        alertSheep = FindObjectOfType<AlertSheep>();
        shepherds = FindObjectsOfType<Shepherd>();

        pauseManager.PauseAudio += PauseSounds;

        wolf.UpdateBGMusic += PlayBGMusic;

        wolfMovement.WolfMoving += PlayWolfMovingAudio;
        alertSheep.WolfNearSheep += PlaySheepBaahhAudio;
        wolf.SheepEaten += PlaySheepEatenAudio;

        // ShepherdSuspicious += PlayShepherdSuspiciousAudio;
        // ShepherdAlerted += PlayShepherdAlertedAudio;

        foreach (Shepherd shepherd in shepherds) {
            shepherd.ShepherdHunting += PlayShepherdHuntingAudio;
            shepherd.transform.GetChild(2).GetComponent<ShepherdGun>().OnShot += PlayHuntingRifleAudio;
        }
    }

    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    void Update() {
        // Audio
        if (playSheepSound && sheepAudioTimer > 0) {
            sheepAudioTimer -= Time.deltaTime;
        } else if (playSheepSound) {
            int i = UnityEngine.Random.Range(0, sheepBaahhs.Length);
            audioSrc.PlayOneShot(sheepBaahhs[i], oneShotVol);
            sheepAudioTimer = UnityEngine.Random.Range(sheepMinDelay, sheepMaxDelay);
        }

        if (wolfBeingHunted && shepherdAudioTimer > 0) {
            shepherdAudioTimer -= Time.deltaTime;
        } else if (wolfBeingHunted) {
            audioSrc.PlayOneShot(shepherdHunting, oneShotVol);
            shepherdAudioTimer = UnityEngine.Random.Range(shepherdMinDelay, shepherdMaxDelay);
        }
        
        // Testing
        // if (Input.GetKeyDown("w")) {
        //     Debug.Log("Wolf started walking");
        //     isWalking = true;
        //     WolfMoving.Invoke(this, new WolfMoveEventArgs {isWalking = true});
        // }

        // if (Input.GetKeyUp("w")) {
        //     Debug.Log("Wolf stopped walking");
        //     isWalking = false;
        //     WolfMoving.Invoke(this, new WolfMoveEventArgs {});
        // }

        // if (Input.GetKeyDown(KeyCode.LeftShift)) {
        //     Debug.Log("Wolf started running");
        //     WolfMoving.Invoke(this, new WolfMoveEventArgs {isRunning = true});
        // }

        // if (Input.GetKeyUp(KeyCode.LeftShift)) {
        //     Debug.Log("Wolf stopped running");
        //     WolfMoving.Invoke(this, new WolfMoveEventArgs {isWalking = isWalking});
        // }

        //if (Input.GetKeyDown("1")) {
        //     Debug.Log("Undetected to Suspicious");
        //     PlayBGMusic(this, new Wolf.MusicEventArgs {currentState = WolfStatus.Undetected, newState = WolfStatus.Suspicious});
        //}

        // if (Input.GetKeyDown("s")) {
        //     testToggle = !testToggle;
        //     WolfNearSheep.Invoke(this, testToggle);
        // }

        // if (Input.GetKeyDown("h")) {
        //     testToggle = !testToggle;
        //     ShepherdHunting.Invoke(this, testToggle);
        // }
    }

    public void PlayBGMusic(object sender, Wolf.MusicEventArgs e) {
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                currentState.ToString(),
                                                duration,
                                                0.0f));

        //suspiciousAudioSrc.time = 25.0f;
        StartCoroutine(FadeMixerGroup.StartFade(audioMixer,
                                                e.newState.ToString(),
                                                duration,
                                                1.0f));
        currentState = e.newState;
    }

    public void PauseSounds(object sender, Boolean pauseAudio) {
        if (pauseAudio) {
            audioSrc.Pause();
            wolfAudio.Pause();
            undetectedAudioSrc.Pause();
            suspiciousAudioSrc.Pause();
            identifiedAudioSrc.Pause();
        } else {
            audioSrc.UnPause();
            wolfAudio.UnPause();
            undetectedAudioSrc.UnPause();
            suspiciousAudioSrc.UnPause();
            identifiedAudioSrc.UnPause();
        }

    }

    public void PlayWolfMovingAudio(object sender, WolfMovement.WolfMoveEventArgs e) {
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
    }

    public void PlaySheepEatenAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(sheepDeath, oneShotVol);
        audioSrc.PlayOneShot(wolfEatingAudio, oneShotVol);
    }

    public void PlayShepherdSuspiciousAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(shepherdSuspicious, oneShotVol);
    }

    public void PlayShepherdAlertedAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(shepherdAlerted, oneShotVol);
    }

    public void PlayShepherdHuntingAudio(object sender, bool isHunting) {
        wolfBeingHunted = isHunting;
    }

    public void PlayHuntingRifleAudio(object sender, EventArgs e) {
        audioSrc.PlayOneShot(rifleShot, oneShotVol);
    }
}

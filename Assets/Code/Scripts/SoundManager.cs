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

public class SoundManager : MonoBehaviour
{
    // Events
    //[SerializeField] private WolfMovement wolf;
    public event EventHandler<Boolean> WolfWalking;

    // Audio Sources
    private AudioSource audioSrc;
    [SerializeField] private AudioSource backgroundSrc;
    [SerializeField] private AudioSource wolfAudio;

    // Audio Clips
    [SerializeField] private AudioClip wolfWalkingAudio;

    void Awake() {
        WolfWalking += PlayWalkingAudio;
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
    }

    public void PlayWalkingAudio(object sender, Boolean isWalking ) {
        if (isWalking) {
            wolfAudio.clip = wolfWalkingAudio;
            wolfAudio.Play();
        } else {
            wolfAudio.Stop();
        }
    }
}

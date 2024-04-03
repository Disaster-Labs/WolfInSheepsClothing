// ---------------------------------------
// Creation Date: 04/02/24
// Author: Jason Perez
// Modified By: Jason Perez
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Events
    [SerializeField] private WolfMovement wolf;

    // Audio Sources
    private AudioSource audioSrc;
    [SerializeField] private AudioSource backgroundSrc;

    void Awake() {
        wolf.PlayBGMusic += PlayBGMusic;
    }

    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlayBGMusic(object sender, EventArgs e) {
        backgroundSrc.Play();
        Debug.Log("Play BG Music");
    }
}

// ---------------------------------------
// Creation Date:04/04/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance {get; private set; }
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject PauseCanvas;

    public event EventHandler<Boolean> PauseAudio;
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (GameIsPaused){
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume(){
        PauseAudio.Invoke(this, false);
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    public void Pause(){
        PauseAudio.Invoke(this, true);
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}

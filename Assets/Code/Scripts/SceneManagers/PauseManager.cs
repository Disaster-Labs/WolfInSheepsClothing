// ---------------------------------------
// Creation Date:04/04/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance {get; private set; }
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject PauseCanvas;

    // [SerializeField] private GameSoundManager soundManager;
    
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
        // soundManager.ResumeAudio();
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause(){
        // soundManager.PauseAudio();
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}

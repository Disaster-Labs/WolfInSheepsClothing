// ---------------------------------------
// Creation Date: 4/18/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;


public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI messageText; 
    public GameObject gameOverCanvas;

    public SoundManager soundManager;

    void Start()
    {
        gameOverCanvas.SetActive(false);  
        GameManager.OnGameEnd += HandleGameEnd;  
    }

    void OnDestroy()
    {
        GameManager.OnGameEnd -= HandleGameEnd;  
    }

    private void HandleGameEnd(bool won)
    {   
        soundManager.PauseSounds(null, true);
        
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);  
        if (won)
        {
            messageText.text = "You win!";
            scoreText.text = "Your Score is: " + GameManager.score.ToString();
        }
        else
        {
            messageText.text = "You died!";
            scoreText.text = "Your Current Score is: "+ GameManager.score.ToString(); 
        }
    }

}

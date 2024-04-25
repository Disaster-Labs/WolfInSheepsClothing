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


public class StatusBoardManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI timerText; 
    public GameObject[] meatIcons;

    public GameObject[] healthBar;
    
    private TimeSpan timeLeft = TimeSpan.FromMilliseconds(GameManager.gameTime);

    void Update()
    {
        ScoreBoard(GameManager.score); 
        UpdateTimerDisplay();
        UpdateHunger(GameManager.hunger);
        UpdateHealth(GameManager.HealthCurrent);
    }

    public void ScoreBoard(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score:    " + GameManager.score; 
    }
    public void UpdateHunger(float hungerLevel)
    {
        for (int i = 0; i < meatIcons.Length; i++)
        {
            meatIcons[i].transform.Find("FullVisual").gameObject.SetActive(i < hungerLevel);
        }
    }
    public void UpdateHealth(int health)
    {
        for (int i = 0; i < healthBar.Length; i++)
        {
            healthBar[i].transform.Find("FullVisual").gameObject.SetActive(i < health);
        }
    }
    public void UpdateTimerDisplay()
    {   
        if (timeLeft.TotalMilliseconds > 0)
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));

            if (timeLeft.TotalSeconds <= 10)
            {
                timerText.color = Color.red; 
            }

            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                timeLeft.Minutes, timeLeft.Seconds, timeLeft.Milliseconds / 10);
        }
        else
        {
            timerText.text = "00:00:00";
            enabled = false; // Stop updating
        }
    }  
}

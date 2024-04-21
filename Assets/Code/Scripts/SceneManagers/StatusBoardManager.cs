// ---------------------------------------
// Creation Date: 4/18/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;


public class StatusBoardManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 
    public Image[] meatIcons;

    public Image healthBar;

    void Update()
    {
        ScoreBoard(GameManager.score); 
        
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
            meatIcons[i].enabled = i < hungerLevel; // 显示或隐藏图标
        }
    }
    public void UpdateHealth(int health)
    {
        healthBar.fillAmount = (float) health / 3.0f;
    }
}

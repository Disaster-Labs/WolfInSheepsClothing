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
    public GameObject[] meatIcons;

    public GameObject[] healthBar;

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
}

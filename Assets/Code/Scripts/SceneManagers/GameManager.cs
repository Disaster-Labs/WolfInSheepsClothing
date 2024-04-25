// ---------------------------------------
// Creation Date: 04/04/2024
// Author: 
// Modified By: Boyi
// ---------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set; }
    
    // Scene Names
    public const string MAIN_MENU_SCENE = "MainMenuScene";
    public const string GAME_SCENE = "GameScene_Abby";
    public const string GAME_OVER_SCENE = "GameOverScene";
    public const string CREDITS_SCENE = "CreditsScene";
    
    public static event System.Action OnSheepEaten;    
    public static event System.Action OnWolfHit ;    
    public static event System.Action<bool> OnGameEnd; 
    public static int score = 0; 

    public static float hunger = 3;
    public float hungerDecayRate = 0.1f;

    public static int HealthCurrent;
    public int HealthMax = 3;

    public static float gameTime = 15000f;
    public static TimeSpan timeLeft;
    
    public void ResetGame()
    {
        score = 0;
        hunger = 3;
        HealthCurrent = HealthMax;
        timeLeft = TimeSpan.FromMilliseconds(gameTime);
    } 

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); 
    }
    void Start()
    {
        Time.timeScale = 1f;
        OnSheepEaten += SheepEaten;
        OnWolfHit += WolfHit;
        HealthCurrent = HealthMax;
        timeLeft = TimeSpan.FromMilliseconds(gameTime);
    }

    void Update()
    {   
        timeLeft = timeLeft.Subtract(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
        if (hunger > 0)
        {
            hunger -= Time.deltaTime * hungerDecayRate;
            // StatusBoardManager.UpdateHunger(Mathf.Ceil(hunger)); 
            Debug.Log(hunger);
        }

        if (HealthCurrent <= 0)
        {
            OnGameEnd?.Invoke(false);  
        }
        if (timeLeft.TotalMilliseconds <= 0)
        {
            OnGameEnd?.Invoke(true);  
        }
    }

    void OnDestroy()
    {

        OnSheepEaten -= SheepEaten;
        OnWolfHit -= WolfHit;
    }
    private void WolfHit()
    {
        HealthCurrent -= 1;
        Debug.Log("Wolf been hit");

    }
    private void SheepEaten()
    {
        score += 1;
        hunger += 0.5f;
        Debug.Log("A sheep has been eaten! Updating game state...");

    }
    public static void TriggerSheepEaten()
    {
        OnSheepEaten?.Invoke();

    }
    public static void TriggerWolfHit()
    {
        OnWolfHit?.Invoke();
    }
}

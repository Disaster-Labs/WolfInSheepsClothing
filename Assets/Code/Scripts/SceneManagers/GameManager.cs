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
    
    public static int score = 0; 

    public static float hunger = 0;
    public float hungerDecayRate = 0.1f;
    [SerializeField] private WolfMovement wolfMovement;
    [SerializeField] private Wolf wolf;
    private bool isRunning;

    public static int HealthCurrent;
    public int HealthMax = 3;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
    }
    
    void Start()
    {
        wolfMovement.WolfRunning += (_, e) => isRunning = e;
        wolf.SheepEaten += SheepEaten;
        wolfMovement.OnWolfHit += WolfHit;
        HealthCurrent = HealthMax;
    }

    void Update()
    {
        if (hunger > 0 && isRunning)
        {
            hunger -= Time.deltaTime * hungerDecayRate;
            // StatusBoardManager.UpdateHunger(Mathf.Ceil(hunger)); 
        }
    }

    private void WolfHit()
    {
        HealthCurrent -= 1;
        // Debug.Log("Wolf been hit");

    }
    private void SheepEaten(object sender, EventArgs e)
    {
        score += 1;
        hunger += 0.5f;
        // Debug.Log("A sheep has been eaten! Updating game state...");
    }
}

// ---------------------------------------
// Creation Date: 04/04/2024
// Author: 
// Modified By: Boyi
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set; }
    
    // Scene Names
    public const string MAIN_MENU_SCENE = "MainMenuScene";
    public const string GAME_SCENE = "Wolf_Abby";
    public const string GAME_OVER_SCENE = "GameOverScene";
    public const string CREDITS_SCENE = "CreditsScene";
    
    public static event System.Action OnSheepEaten;    
      public static event System.Action OnWolfHit ;    
    public static int score = 0; 

    public static float hunger = 3;
    public float hungerDecayRate = 0.1f;

    public static int HealthCurrent;
    public int HealthMax = 3;
    
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

        OnSheepEaten += SheepEaten;
        OnWolfHit += WolfHit;
        HealthCurrent = HealthMax;
    }

    void Update()
    {
        if (hunger > 0)
        {
            hunger -= Time.deltaTime * hungerDecayRate;
            // StatusBoardManager.UpdateHunger(Mathf.Ceil(hunger)); 
            Debug.Log(hunger);
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

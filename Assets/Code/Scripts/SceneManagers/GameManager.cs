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
    public static int score = 0; 
    
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
    }

    void OnDestroy()
    {

        OnSheepEaten -= SheepEaten;
    }
    
    private void SheepEaten()
    {
        score += 1;
        Debug.Log("A sheep has been eaten! Updating game state...");

    }
    public static void TriggerSheepEaten()
    {
        OnSheepEaten?.Invoke();
    }
}

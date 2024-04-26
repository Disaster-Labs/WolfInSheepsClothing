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
    public const string GAME_SCENE = "GameScene";
    public const string GAME_OVER_SCENE = "GameOverScene";
    public const string CREDITS_SCENE = "CreditsScene";

    private const string KILLED = "Killed";
    
    public static event Action<bool> OnGameEnd; 
    public static int score = 0; 

    public static float hunger = 3;
    public float hungerDecayRate;
    [SerializeField] private WolfMovement wolfMovement;
    [SerializeField] private Wolf wolf;
    private bool isRunning;

    public static int HealthCurrent;
    public int HealthMax = 3;

    public static float gameTime = 150000f;
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
        Time.timeScale = 1f;
        HealthCurrent = HealthMax;
        timeLeft = TimeSpan.FromMilliseconds(gameTime);
    }

    void Update()
    {   
        timeLeft = timeLeft.Subtract(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
        if (hunger > 0 && isRunning)
        {
            hunger -= Time.deltaTime * hungerDecayRate;
            // StatusBoardManager.UpdateHunger(Mathf.Ceil(hunger)); 
        }

        if (HealthCurrent <= 0)
        {
            FindObjectOfType<WolfMovement>().transform.GetChild(0).GetComponent<Animator>().SetBool(KILLED, true);
            Shepherd[] shepherds = FindObjectsByType<Shepherd>(FindObjectsSortMode.None);
            foreach (Shepherd shepherd in shepherds) {
                shepherd.ChangeState(new Capturing());
            }
            
            StartCoroutine(WaitForAnim());
        }

        if (timeLeft.TotalMilliseconds <= 0)
        {
            OnGameEnd?.Invoke(true);  
        }
    }

    private IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(2);
        OnGameEnd?.Invoke(false);  
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

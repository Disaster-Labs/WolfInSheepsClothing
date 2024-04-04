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
    
}

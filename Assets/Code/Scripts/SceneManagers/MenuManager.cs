// ---------------------------------------
// Creation Date: 04/04/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "MainMenuScene";
    public const string GAME_SCENE = "GameScene";
    public const string GAME_OVER_SCENE = "GameOverScene";
    public const string CREDITS_SCENE = "CreditsScene";
    // public GameObject HowToCanvas;
        
    public void Btn_QuitGame()
    {
        Application.Quit();
    }

    public void Btn_StartGame()
    {
        // GameManager.Instance.ResetGame(); 
        SceneManager.LoadScene(GAME_SCENE);
        Time.timeScale = 1f;
    }

    public void Btn_Credits()
    {
        SceneManager.LoadScene(CREDITS_SCENE);
    }

    public void Btn_BackMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MAIN_MENU_SCENE);
        
    }

    // public void Btn_HowTo(){
    //     HowToCanvas.SetActive(true);
    // }

    // public void Btn_CloseHowTo(){
    //     HowToCanvas.SetActive(false);
    // }


    
}

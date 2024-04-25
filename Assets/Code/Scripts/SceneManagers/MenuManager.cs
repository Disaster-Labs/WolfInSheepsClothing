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
    // public GameObject HowToCanvas;
        
    public void Btn_QuitGame()
    {
        Application.Quit();
    }

    public void Btn_StartGame()
    {
        GameManager.Instance.ResetGame(); 
        SceneManager.LoadScene(GameManager.GAME_SCENE);
        Time.timeScale = 1f;
    }

    public void Btn_Credits()
    {
        SceneManager.LoadScene(GameManager.CREDITS_SCENE);
    }

    public void Btn_BackMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameManager.MAIN_MENU_SCENE);
        
    }

    // public void Btn_HowTo(){
    //     HowToCanvas.SetActive(true);
    // }

    // public void Btn_CloseHowTo(){
    //     HowToCanvas.SetActive(false);
    // }


    
}

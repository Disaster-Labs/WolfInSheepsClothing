// ---------------------------------------
// Creation Date: 4/18/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class StatusBoardManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score:    " + GameManager.score; 
    }
}

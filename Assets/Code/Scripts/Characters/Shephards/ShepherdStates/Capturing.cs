// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capturing : ShepherdState
{
    public void OnEnter(Shepherd shepherd)
    {
        shepherd.StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        // play some animation 
        // show game over menu
        yield return null;
    }

    public void OnUpdate() {}

    public void OnExit() {}
}

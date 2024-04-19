// ---------------------------------------
// Creation Date: 04/18/2024
// Author: Boyi Qian
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : MonoBehaviour
{
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {

            GameManager.TriggerSheepEaten();
        }
    }
}

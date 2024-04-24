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
            Debug.Log("Dummy eat trigger!");
            GameManager.TriggerSheepEaten();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Dummy hit trigger!");
            GameManager.TriggerWolfHit();
        }
    }
}

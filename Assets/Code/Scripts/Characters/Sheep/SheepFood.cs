// ---------------------------------------
// Creation Date: 4/15/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepFood : MonoBehaviour
{
    private int sheepFoodAmount = 3;

    public bool EatSheepFood() {
        if (sheepFoodAmount > 0) {
            sheepFoodAmount--;
            return true;
        }

        return false;
    }
}

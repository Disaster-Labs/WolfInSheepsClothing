// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : SheepState {
    public void OnEnter(SheepHerd herd, Sheep sheep) {

        if (sheep.inHerd) {
            foreach (Sheep herdSheep in herd.sheeps) {
                if (herdSheep.sheepState.GetType() == typeof(Grazing)) {
                    herd.ChangeState(herdSheep, new Fleeing()); 
                } 
            }
        }

        Object.Destroy(sheep.gameObject);
    }

    public void OnExit() {}

    public void OnUpdate() {}
}

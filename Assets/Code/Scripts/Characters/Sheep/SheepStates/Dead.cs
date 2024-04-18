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

            Shepherd closestShepherd = FindNearestShepherdToWolf();
            closestShepherd.ChangeState(closestShepherd.hunting);

            Wolf wolf = Object.FindFirstObjectByType<Wolf>();
            wolf.SetBeingChased(true);
        }

        Object.Destroy(sheep.gameObject);
    }

    private Shepherd FindNearestShepherdToWolf() {
        Wolf wolf = Object.FindFirstObjectByType<Wolf>();
        Shepherd[] shepherds = Object.FindObjectsByType<Shepherd>(FindObjectsSortMode.None);
        Shepherd closestShepherd = shepherds[0];

        foreach (Shepherd shepherd in shepherds) {
            Vector2 curPos = closestShepherd.transform.position;
            Vector2 newPos = shepherd.transform.position;
            Vector2 wolfPos = wolf.transform.position;

            // comparing distances without doing a square root which causes frame drops
            if (Vector2.SqrMagnitude(newPos - wolfPos) < Vector2.SqrMagnitude(curPos - wolfPos)) {
                closestShepherd = shepherd;
            }
        }

        return closestShepherd;
    }

    public void OnExit() {}

    public void OnUpdate() {}
}

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
        Object.Destroy(sheep.gameObject);
    }

    public void OnExit() {}

    public void OnUpdate() {}
}

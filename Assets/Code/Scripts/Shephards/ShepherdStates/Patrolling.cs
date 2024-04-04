// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrolling : ShepherdState
{
    private Shepherd shepherd;

    public void OnEnter(Shepherd shepherd) {
        this.shepherd = shepherd;
    }

    public void OnUpdate() {
        throw new System.NotImplementedException();
    }

    public void OnExit() {
        throw new System.NotImplementedException();
    }
}

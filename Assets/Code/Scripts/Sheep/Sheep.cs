// ---------------------------------------
// Creation Date: 4/1/23
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float herdCenter;
    public float moveRadius;
}

public interface SheepState {
    public void OnEnter();
    public void OnUpdate(Sheep sheep);
    public void OnExit();
}

public class Grazing : SheepState
{
    public void OnEnter() {}

    public void OnUpdate(Sheep sheep) {
        
    }

    public void OnExit() {}

}
// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Shepherd : MonoBehaviour
{
    [SerializeField] private Sprite visual_forward;
    [SerializeField] private Sprite visual_side;
    [SerializeField] private Sprite visual_back;

    public AstarPath astar;

    private Patrolling patrolling = new Patrolling();

    private ShepherdState shepherdState;

    private void Start() {
        ChangeState(patrolling);
    }

    private void Update() {
        shepherdState.OnUpdate();
    }

    public void ChangeState(ShepherdState state) {
        if (shepherdState != null) shepherdState.OnExit();
        shepherdState = state;
        shepherdState.OnEnter(this);
    }
}

public interface ShepherdState {
    public void OnEnter(Shepherd shepherd);
    public void OnUpdate();
    public void OnExit();
}
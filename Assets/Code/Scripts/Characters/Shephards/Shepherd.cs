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
    [System.NonSerialized] public Wolf wolf;

    public AstarPath astar;
    public WolfDetection wolfDetection;
    public ShepherdGun shepherdGun;

    public Patrolling patrolling = new Patrolling();
    public Hunting hunting = new Hunting();
    public HuntingAround huntingAround = new HuntingAround();

    private ShepherdState shepherdState;

    private void Start() {
        wolf = FindObjectOfType<Wolf>();
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
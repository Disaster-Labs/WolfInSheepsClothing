// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public enum ShepherdPathType {
    Vertical,
    Horizontal,
    Turn,
    Circular
}

public class Shepherd : MonoBehaviour
{
    [System.NonSerialized] public Wolf wolf;

    [SerializeField] public ShepherdPathType shepherdPathType;
    [SerializeField] public Sprite shepherdUp;
    [SerializeField] public Sprite shepherdDown;
    [SerializeField] public Sprite shepherdSide;
    [SerializeField] public SpriteRenderer visual;

    public AstarPath astar;
    public WolfDetection wolfDetection;
    public ShepherdGun shepherdGun;
    public Vector2 startPos;

    public Patrolling patrolling = new Patrolling();
    public Hunting hunting = new Hunting();
    public HuntingAround huntingAround = new HuntingAround();

    private ShepherdState shepherdState;

    // Sound
    public event EventHandler<Boolean> ShepherdHunting;

    private void Start() {
        wolf = FindObjectOfType<Wolf>();
        startPos = transform.position;
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

    public void InvokeHunting(bool isHunting) {
        ShepherdHunting?.Invoke(this, isHunting);
    }
}

public interface ShepherdState {
    public void OnEnter(Shepherd shepherd);
    public void OnUpdate();
    public void OnExit();
}
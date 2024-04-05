// ---------------------------------------
// Creation Date: 3/31/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class WolfMovement : MonoBehaviour
{
    // Events
    public event EventHandler PlayBGMusic;

    // State 
    private WolfState currentState;
    private DefaultState defaultState = new DefaultState();

    // Input
    private WolfInput input;
    private Vector2 moveDirection;
    private bool isSprinting;

    // Movement
    private Rigidbody2D rb;
    private float walkSpeed;
    private float sprintSpeed;

    // Look Direction
    private float startScaleX;

    private void Awake() {
        input = GetComponent<WolfInput>();
        rb = GetComponent<Rigidbody2D>();

        input.OnMove += (_, e) => moveDirection = e.direction;
        input.OnSprint += (_, e) => isSprinting = e.isSprinting;

        startScaleX = transform.localScale.x;
    }
    
    private void Start() {
        ChangeState(defaultState);
    }

    public void ChangeState(WolfState state) {
        currentState = state;

        currentState.OnEnter();
        walkSpeed = currentState.walkSpeed;
        sprintSpeed = currentState.sprintSpeed;
    }

    private void Update() {
        if (currentState != null) currentState.OnUpdate(this);

        if (Input.GetKeyDown("space")) {
            PlayBGMusic.Invoke(this, EventArgs.Empty);
        }
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleLookDirection();
    }

    private void HandleMovement() {
        if (isSprinting) rb.velocity = moveDirection * sprintSpeed;
        else rb.velocity = moveDirection * walkSpeed;
    }

    private void HandleLookDirection() {
        if (rb.velocity.x < 0) transform.localScale = new Vector3(-startScaleX, transform.localScale.y, transform.localScale.z);
        else if (rb.velocity.x > 0) transform.localScale = new Vector3(startScaleX, transform.localScale.y, transform.localScale.z);
    }
}

// States that affect movement
public class WolfState {
    public float walkSpeed;
    public float sprintSpeed;

    public virtual void OnEnter() {}
    // for changing in between states
    public virtual void OnUpdate(WolfMovement wolf) {}
}

public class DefaultState : WolfState
{
    public override void OnEnter() {
        walkSpeed = 5f;
        sprintSpeed = 8f;

        // start some animation
    }
}

// States to add: Hungry, Damaged ... 

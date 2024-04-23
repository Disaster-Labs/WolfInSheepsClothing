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
    // State 
    private WolfMovementState currentState;
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

    // Animation
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string DIRECTION = "Direction";
    private enum Direction {
        Side,
        Up,
        Down
    }

    // Sound Events
    public class WolfMoveEventArgs { public bool isWalking = false; public bool isRunning = false; }
    public event EventHandler<WolfMoveEventArgs> WolfMoving;
    
    private Animator anim;

    private void Awake() {
        input = GetComponent<WolfInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        input.OnMove += (_, e) => moveDirection = e.direction;
        input.OnSprint += (_, e) => isSprinting = e.isSprinting;

        startScaleX = transform.localScale.x;
    }
    
    private void Start() {
        ChangeState(defaultState);
    }

    public void ChangeState(WolfMovementState state) {
        currentState = state;

        currentState.OnEnter();
        walkSpeed = currentState.walkSpeed;
        sprintSpeed = currentState.sprintSpeed;
    }

    private void Update() {
        if (currentState != null) currentState.OnUpdate(this);
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleLookDirection();
    }

    private void HandleMovement() {
        Vector2 prevVel = rb.velocity;
        if (isSprinting) rb.velocity = moveDirection * sprintSpeed;
        else rb.velocity = moveDirection * walkSpeed;

        anim.SetBool(IS_RUNNING, isSprinting && rb.velocity != Vector2.zero);
        anim.SetBool(IS_WALKING, !isSprinting && rb.velocity != Vector2.zero);

        if (prevVel == Vector2.zero && rb.velocity != Vector2.zero) WolfMoving?.Invoke(this, new WolfMoveEventArgs {isWalking = !isSprinting, isRunning = isSprinting});
        else if (prevVel != Vector2.zero && rb.velocity == Vector2.zero) WolfMoving?.Invoke(this, new WolfMoveEventArgs {isWalking = false, isRunning = false});
    }

    private void HandleLookDirection() {
        if (rb.velocity.x < 0) transform.localScale = new Vector3(-startScaleX, transform.localScale.y, transform.localScale.z);
        else if (rb.velocity.x > 0) transform.localScale = new Vector3(startScaleX, transform.localScale.y, transform.localScale.z);

        if (Mathf.Approximately(rb.velocity.x, 0) && rb.velocity.y < 0) {
            anim.SetInteger(DIRECTION, (int) Direction.Down);
        } else if (Mathf.Approximately(rb.velocity.x, 0) && rb.velocity.y > 0) {
            anim.SetInteger(DIRECTION, (int) Direction.Up);
        } else {
            anim.SetInteger(DIRECTION, (int) Direction.Side);
        }
    }
}

// States that affect movement
public class WolfMovementState {
    public float walkSpeed;
    public float sprintSpeed;

    public virtual void OnEnter() {}
    // for changing in between states
    public virtual void OnUpdate(WolfMovement wolf) {}
}

public class DefaultState : WolfMovementState
{
    public override void OnEnter() {
        walkSpeed = 7f;
        sprintSpeed = 12f;

        // start some animation
    }
}

// States to add: Hungry, Damaged ... 

// ---------------------------------------
// Creation Date: 3/31/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class WolfMovement : MonoBehaviour
{
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
        float scaleX;
        float angleZ;

        if (moveDirection.x < 0) {
            scaleX = -startScaleX;
            angleZ = 0;
        } else if (moveDirection.x > 0) {
            scaleX = startScaleX;
            angleZ = 0;
        } else if (moveDirection.y > 0) {
            scaleX = startScaleX;
            angleZ = 90;
        } else if (moveDirection.y < 0) {
            scaleX = startScaleX;
            angleZ = -90;
        } else {
            scaleX = startScaleX;
            angleZ = 0;
        }

        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleZ);

        // if (moveDirection.x < 0 && moveDirection.y >= 0) transform.localScale = new Vector3(-startScaleX, transform.localScale.y, transform.localScale.z);
        // else if (moveDirection.x > 0) transform.localScale = new Vector3(startScaleX, transform.localScale.y, transform.localScale.z);

        // // θ = arctan(y/x)
        // // float lookAngle = Mathf.Rad2Deg * Mathf.Atan(moveDirection.y/moveDirection.x);
        // float lookAngle;
        // if (moveDirection.x == 0) {
        //     lookAngle = moveDirection.y == 0 ? transform.eulerAngles.x : -90 * moveDirection.y;
        // } else {
        //     lookAngle = 0;
        // }

        // transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, lookAngle);
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

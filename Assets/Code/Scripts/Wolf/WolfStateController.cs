// ---------------------------------------
// Creation Date: 3/31/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfStateController : MonoBehaviour
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

    private void Awake() {
        input = GetComponent<WolfInput>();
        rb = GetComponent<Rigidbody2D>();

        input.OnMove += (_, e) => moveDirection = e.direction;
        input.OnSprint += (_, e) => isSprinting = e.isSprinting;
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
        // Handling Movement
        if (isSprinting) rb.velocity = moveDirection * sprintSpeed;
        else rb.velocity = moveDirection * walkSpeed;

        Debug.Log(moveDirection);
    }
}

// States that affect movement
public class WolfState {
    public float walkSpeed;
    public float sprintSpeed;

    public virtual void OnEnter() {}
    // for changing in between states
    public virtual void OnUpdate(WolfStateController stateController) {}
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

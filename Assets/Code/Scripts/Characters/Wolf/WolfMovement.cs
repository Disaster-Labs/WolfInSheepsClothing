// ---------------------------------------
// Creation Date: 3/31/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovement : MonoBehaviour
{
    // State 
    private WolfMovementState currentState;
    public Default defaultState = new Default();
    public Hungry hungry = new Hungry();
    public Eating eating = new Eating();
    public DeadWolf dead = new DeadWolf();

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

    private Animator anim;

    // Sound Events
    public event EventHandler<bool> WolfRunning;
    public class WolfMoveEventArgs { public bool isWalking = false; public bool isRunning = false; }
    public event EventHandler<WolfMoveEventArgs> WolfMoving;
    
    // Damaging
    [SerializeField] private LayerMask bullet;
    public event Action OnWolfHit;

    private void Awake() {
        input = GetComponent<WolfInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        input.OnMove += (_, e) => moveDirection = e.direction;
        input.OnSprint += (_, e) => isSprinting = e.isSprinting;
        GetComponent<Wolf>().SheepEaten += (_, _) => ChangeState(eating);

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

        if (GameManager.HealthCurrent == 0) ChangeState(dead);
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleLookDirection();
    }

    private void HandleMovement() {
        Vector2 prevVel = rb.velocity;
        if (isSprinting) rb.velocity = moveDirection * sprintSpeed;
        else rb.velocity = moveDirection * walkSpeed;

        anim.SetBool(IS_RUNNING, isSprinting && rb.velocity != Vector2.zero && currentState != hungry);
        anim.SetBool(IS_WALKING, (!isSprinting || currentState == hungry) && rb.velocity != Vector2.zero);

        if (isSprinting && rb.velocity != Vector2.zero) WolfRunning?.Invoke(this, true);
        else WolfRunning?.Invoke(this, false);

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

    private float invulnerableTime = 0;
    private float timeSinceHit = 0;

    private void OnTriggerEnter2D(Collider2D col) {
        if (bullet == (bullet | (1 << col.gameObject.layer)) && timeSinceHit == 0) {
            OnWolfHit?.Invoke();
            Destroy(col.gameObject);
            StartCoroutine(Invulnerable());
        }
    }

    private IEnumerator Invulnerable()
    {
        while (timeSinceHit < invulnerableTime) {
            timeSinceHit += Time.deltaTime;
            yield return null;
        }

        timeSinceHit = 0;
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

public class Default : WolfMovementState
{
    public override void OnEnter() {
        walkSpeed = 7f;
        sprintSpeed = 12f;
    }

    public override void OnUpdate(WolfMovement wolf) {
        if (GameManager.hunger <= 0) {
            wolf.ChangeState(wolf.hungry);
        }
    }
}

public class Hungry : WolfMovementState
{
    public override void OnEnter() {
        walkSpeed = 7f;
        sprintSpeed = 7f;
    }

    public override void OnUpdate(WolfMovement wolf) {
        if (GameManager.hunger > 0) {
            wolf.ChangeState(wolf.defaultState);
        }
    }
}

public class Eating : WolfMovementState
{
    float startTime;
    float animTime = 2;

    public override void OnEnter() {
        walkSpeed = 0f;
        sprintSpeed = 0f;

        startTime = Time.time;
    }

    public override void OnUpdate(WolfMovement wolf)
    {
        if (Time.time - startTime > animTime) {
            wolf.ChangeState(wolf.defaultState);
        }
    }
}

public class DeadWolf : WolfMovementState
{
    public override void OnEnter() {
        walkSpeed = 0f;
        sprintSpeed = 0f;
    }
}

// States to add: Damaged ... 

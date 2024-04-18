// ---------------------------------------
// Creation Date: 3/31/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfInput : MonoBehaviour
{
    public class DirectionEventArgs { public Vector2 direction; }
    public event EventHandler<DirectionEventArgs> OnMove;
    public class SprintingEventArgs { public bool isSprinting; }
    public event EventHandler<SprintingEventArgs> OnSprint;
    public event EventHandler OnInteract;

    private InputActions inputActions;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();

        inputActions.Wolf.Sprint.started += ctx => OnSprint?.Invoke(this, new SprintingEventArgs {isSprinting = true});
        inputActions.Wolf.Sprint.canceled += ctx => OnSprint?.Invoke(this, new SprintingEventArgs {isSprinting = false});
        inputActions.Wolf.Interact.started += ctx => OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void FixedUpdate() {
        Vector2 direction = inputActions.Wolf.Move.ReadValue<Vector2>();
        OnMove?.Invoke(this, new DirectionEventArgs {direction = direction});
    }
}

// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public event EventHandler OnEnterForest;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            OnEnterForest?.Invoke(this, EventArgs.Empty);
        }
    }

    [SerializeField] private LayerMask sheepLayerMask;
    [SerializeField] private AlertSheep alertSheep;
    private WolfInput wolfInput;

    private GameObject eatenSheep;

    public void SetBeingChased(bool beingChased) {
        alertSheep.CanAlertSheep(beingChased);
    }

    private void Start() {
        wolfInput = GetComponent<WolfInput>();
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (sheepLayerMask == (sheepLayerMask | (1 << col.gameObject.layer))) {
            wolfInput.OnEat += HandleEating;
            eatenSheep = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (sheepLayerMask == (sheepLayerMask | (1 << col.gameObject.layer))) {
            wolfInput.OnEat -= HandleEating;
            eatenSheep = null;
        }
    }

    private void HandleEating(object sender, EventArgs e) {
        if (eatenSheep != null) {
            eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
            eatenSheep = null;
        }
    }
}

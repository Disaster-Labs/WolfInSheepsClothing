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
    [SerializeField] private LayerMask sheepLayerMask;
    private WolfInput wolfInput;

    private GameObject eatenSheep;

    private void Start() {
        wolfInput = GetComponent<WolfInput>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
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

    private void HandleEating(object sender, EventArgs e)
    {
        if (eatenSheep != null) {
            eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
            eatenSheep = null;
        }
    }
}

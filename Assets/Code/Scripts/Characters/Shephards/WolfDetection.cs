// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfDetection : MonoBehaviour {
    public event EventHandler OnWolfDetected;
    [SerializeField] private LayerMask wolfLayerMask;

    private void OnTriggerEnter2D(Collider2D col) {
        if (wolfLayerMask == (wolfLayerMask | (1 << col.gameObject.layer))) {
            OnWolfDetected?.Invoke(this, EventArgs.Empty);
        }   
    }
}

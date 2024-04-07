// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSheep : MonoBehaviour
{
    [SerializeField] private LayerMask sheepLayerMask;

    public void CanAlertSheep(bool canAlertSheep) {
        if (canAlertSheep) {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 5;
        } else {
            Destroy(GetComponent<CircleCollider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (sheepLayerMask == (sheepLayerMask | (1 << col.gameObject.layer))) {
            col.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(col.gameObject, new Fleeing());
        }
    }
}

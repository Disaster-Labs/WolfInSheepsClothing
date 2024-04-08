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

    private bool canAlertSheep = false;
    public void CanAlertSheep(bool canAlertSheep) {
        this.canAlertSheep = canAlertSheep;
    }

    private bool sheepFollowing = false;
    public void SheepCanFollow() {
        sheepFollowing = true;
    }

    public void StopSheepFollowing(GameObject sheep) {
        if (sheep == null) return;
        sheepFollowing = false;
        sheep.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(sheep, new WanderAlone());
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (sheepLayerMask == (sheepLayerMask | (1 << col.gameObject.layer))) {
            if (canAlertSheep) {
                col.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(col.gameObject, new Fleeing());
            } else if (sheepFollowing) {
                transform.parent.GetComponent<Wolf>().followingSheep = col.gameObject;
                col.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(col.gameObject, new Following());
                sheepFollowing = false;
            }
        }
    }
}
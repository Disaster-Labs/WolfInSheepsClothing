// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSheep : MonoBehaviour
{
    [SerializeField] private LayerMask sheepLayerMask;
    private GameObject followingSheep;

    private bool canAlertSheep = false;
    public void CanAlertSheep(bool canAlertSheep) {
        this.canAlertSheep = canAlertSheep;
    }

    private bool sheepFollowing = false;
    public void SheepCanFollow() {
        sheepFollowing = true;
    }

    public void StopSheepFollowing() {
        if (followingSheep == null) return;
        sheepFollowing = false;
        followingSheep.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(followingSheep, new WanderAlone());
    }

    public event EventHandler<bool> WolfNearSheep;

    private void OnTriggerStay2D(Collider2D col) {
        if (sheepLayerMask == (sheepLayerMask | (1 << col.gameObject.layer))) {
            WolfNearSheep?.Invoke(this, true);
            if (canAlertSheep) {
                col.transform.parent.GetComponent<SheepHerd>().SheepFlee();
                canAlertSheep = false;
            } else if (sheepFollowing) {
                followingSheep = col.gameObject;
                col.transform.parent.GetComponent<SheepHerd>().ChangeStateByGameObject(col.gameObject, new Following());
                sheepFollowing = false;
            }
        }
    }

    public void InvokeNearSheep(bool nearSheep) {
        WolfNearSheep?.Invoke(this, nearSheep);
    }

    private void OnTriggerExit2D(Collider2D col) {
        WolfNearSheep?.Invoke(this, false);
    }
}
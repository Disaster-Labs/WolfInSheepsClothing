// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class HuntingAround : ShepherdState
{
    private Shepherd shepherd;
    private AIMovement aIMovement;
    private Wolf wolf;
    private GridGraph graph;

    private float shepherdSpeed = 3;
    private float radius;

    private Coroutine cor;

    public void OnEnter(Shepherd shepherd) {
        this.shepherd = shepherd;
        wolf = shepherd.wolf;
        
        shepherd.wolfDetection.gameObject.SetActive(false);
        wolf.OnExitForest += ChangeState;

        Animator anim = shepherd.transform.GetChild(0).GetComponent<Animator>();
        aIMovement = new AIMovement(shepherd.GetComponent<Seeker>(), shepherdSpeed, shepherd.gameObject, anim);

        Vector3 scale = shepherd.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = shepherd.gridGraph;

        radius = 10;

        GoToBush();
        cor = shepherd.StartCoroutine(GiveUp());
    }

    private void ChangeState(object sender, EventArgs e) {
        shepherd.ChangeState(shepherd.hunting);
    }

    private void GoToBush() {
        Vector2 targetPos = wolf.hidingInObject.GetComponent<CircleCollider2D>().ClosestPoint(shepherd.transform.position);
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private void UpdatePath() {
        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
    }

    private Vector2 CalculateTargetPos() {
        Vector2 center = wolf.hidingInObject.transform.position;
        int points = 10;
        float x_i = shepherd.gameObject.transform.position.x - center.x;
        float y_i = shepherd.gameObject.transform.position.y - center.y;
        float angle_i = Mathf.Atan2(y_i, x_i);
        float dAngle = 2 * Mathf.PI / points;
        float angle_f = angle_i + dAngle;
        Vector2 target = new Vector2(radius * Mathf.Cos(angle_f) + center.x, radius * Mathf.Sin(angle_f) + center.y);

        return target;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        if (aIMovement.reachedEndOfPath) {
            UpdatePath();
        }

        aIMovement.UpdateMovement();
    }

    private IEnumerator GiveUp() {
        yield return new WaitForSeconds(15);
        shepherd.ChangeState(shepherd.patrolling);
    }

    public void OnExit() {
        shepherd.wolfDetection.gameObject.SetActive(false);
        wolf.OnExitForest -= ChangeState;
        shepherd.StopCoroutine(cor);
        aIMovement.OnExit();
    }
}

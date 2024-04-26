// ---------------------------------------
// Creation Date:
// Author: 
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Hunting : ShepherdState {
    private Shepherd shepherd;
    private AIMovement aIMovement;
    private Wolf wolf;
    private GridGraph graph;

    private Coroutine cor;

    private float shepherdSpeed = 7;
    private float shepherdWolfRange = 10;

    public void OnEnter(Shepherd shepherd) {
        shepherd.InvokeHunting(true);

        this.shepherd = shepherd;
        wolf = shepherd.wolf;
        wolf.SetBeingChased(true);
        wolf.OnEnterForest += ChangeState;

        Animator anim = shepherd.transform.GetChild(0).GetComponent<Animator>();
        aIMovement = new AIMovement(shepherd.GetComponent<Seeker>(), shepherdSpeed, shepherd.gameObject, anim);

        Vector3 scale = shepherd.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        shepherd.shepherdGun.gameObject.SetActive(true);

        // graph = shepherd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
        // graph.SetDimensions(shepherdHuntRange, shepherdHuntRange, 1);
        // graph.center = wolf.transform.localPosition;
        // graph.is2D = true;
        // graph.collision.use2D = true;
        // AstarPath.active.Scan();
        graph = shepherd.gridGraph;

        UpdatePath();
    }

    private void ChangeState(object sender, EventArgs e)
    {
        shepherd.ChangeState(shepherd.huntingAround);
        wolf.OnEnterForest -= ChangeState;
    }

    private void ShootWolf() {
        shepherd.shepherdGun.ShootAtPosition(wolf.transform.position);
    }

    private IEnumerator InvokeUpdatePath() {
        yield return new WaitForSeconds(1);
        UpdatePath();
    }

    private void UpdatePath() {
        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
    }

    private Vector2 CalculateTargetPos()
    {
        return wolf.transform.position;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;

            cor = shepherd.StartCoroutine(InvokeUpdatePath());
        }
    }

    public void OnUpdate() {
        ShootWolf();

        float rangeToPatrol = 1600;
        float distToWolf = (shepherd.transform.position - wolf.transform.position).sqrMagnitude;
        if (distToWolf >= rangeToPatrol) {
            shepherd.ChangeState(shepherd.patrolling);
            return;
        }
        
        if ((wolf.transform.position - shepherd.transform.position).sqrMagnitude <= shepherdWolfRange * shepherdWolfRange) {
            aIMovement.reachedEndOfPath = true;
            shepherd.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        aIMovement.UpdateMovement();
    }

    public void OnExit() {
        shepherd.InvokeHunting(false);
        wolf.SetBeingChased(false);
        wolf.OnEnterForest -= ChangeState;
        shepherd.shepherdGun.gameObject.SetActive(false);
        // shepherd.astar.data.RemoveGraph(graph);
        shepherd.StopCoroutine(cor);
        aIMovement.OnExit();
    }
}

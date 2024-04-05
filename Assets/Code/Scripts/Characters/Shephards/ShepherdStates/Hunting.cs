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
    private AIMovement aIMovement = new AIMovement();
    private Wolf wolf;
    private GridGraph graph;
    private Vector3 scale;

    private int shepherdHuntRange = 60;
    private float shepherdSpeed = 4;
    private float shepherdWolfRange = 10;

    public void OnEnter(Shepherd shepherd) {
        this.shepherd = shepherd;
        wolf = shepherd.wolf;

        aIMovement.seeker = shepherd.GetComponent<Seeker>();
        aIMovement.speed = shepherdSpeed;
        aIMovement.gameObject = shepherd.gameObject;

        shepherd.shepherdGun.gameObject.SetActive(true);

        scale = shepherd.transform.localScale;
        scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = shepherd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
        graph.SetDimensions(shepherdHuntRange, shepherdHuntRange, 1);
        graph.center = wolf.transform.localPosition;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        UpdatePath();
        shepherd.StartCoroutine(InvokeUpdatePath());
    }

    private void ShootWolf() {
        shepherd.shepherdGun.ShootAtPosition(wolf.transform.position);
    }

    private IEnumerator InvokeUpdatePath()
    {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            UpdatePath();
            yield return null;
        }
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
        }
    }

    public void OnUpdate() {
        ShootWolf();

        Bounds shepherdBounds = new Bounds(shepherd.transform.position, graph.size);
        if (!shepherdBounds.Contains(wolf.transform.position)) {
            shepherd.ChangeState(shepherd.patrolling);
            return;
        }

        if (aIMovement.path == null) return;
        
        if (Vector2.Distance(wolf.transform.position, shepherd.transform.position) <= shepherdWolfRange) {
            aIMovement.reachedEndOfPath = true;
            shepherd.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        aIMovement.UpdateMovement();

        Vector3 dir = (aIMovement.path.vectorPath[aIMovement.currentWaypoint] - shepherd.transform.position).normalized;
        if (dir.x < 0) shepherd.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else if (dir.x > 0) shepherd.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    public void OnExit() {
        shepherd.astar.data.RemoveGraph(graph);
    }
}

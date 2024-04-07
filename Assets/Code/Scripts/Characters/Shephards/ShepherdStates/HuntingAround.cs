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

    private int shepherdHuntRange = 20;
    private float shepherdSpeed = 3;
    private float radius;

    public void OnEnter(Shepherd shepherd) {
        this.shepherd = shepherd;
        wolf = shepherd.wolf;
        
        shepherd.wolfDetection.gameObject.SetActive(true);
        shepherd.wolfDetection.OnWolfDetected += ChangeState;

        aIMovement = new AIMovement(shepherd.GetComponent<Seeker>(), shepherdSpeed, shepherd.gameObject);

        Vector3 scale = shepherd.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = shepherd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
        graph.SetDimensions(shepherdHuntRange, shepherdHuntRange, 1);
        graph.center = wolf.transform.localPosition;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        float x_i = shepherd.gameObject.transform.position.x - graph.center.x;
        float y_i = shepherd.gameObject.transform.position.y - graph.center.y;
        radius = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(x_i, y_i)));

        UpdatePath();
        shepherd.StartCoroutine(GiveUp());
    }

    private void ChangeState(object sender, EventArgs e) {
        shepherd.ChangeState(shepherd.hunting);
    }

    private void UpdatePath() {
        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
    }

    private Vector2 CalculateTargetPos()
    {
        int points = 10;
        float x_i = shepherd.gameObject.transform.position.x - graph.center.x;
        float y_i = shepherd.gameObject.transform.position.y - graph.center.y;
        float angle_i = Mathf.Atan2(y_i, x_i);
        float dAngle = 2 * Mathf.PI / points;
        float angle_f = angle_i + dAngle;
        Vector2 target = new Vector2(radius * Mathf.Cos(angle_f) + graph.center.x, radius * Mathf.Sin(angle_f) + graph.center.y);

        Debug.Log(target);
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
        yield return new WaitForSeconds(60);
        shepherd.ChangeState(shepherd.patrolling);
    }

    public void OnExit() {
        shepherd.astar.data.RemoveGraph(graph);
        shepherd.wolfDetection.gameObject.SetActive(false);
    }
}

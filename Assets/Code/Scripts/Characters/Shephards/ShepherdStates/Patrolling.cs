// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Patrolling : ShepherdState {
    private Shepherd shepherd;
    private AIMovement aIMovement = new AIMovement();
    private GridGraph graph;
    private Vector3 scale;

    public void OnEnter(Shepherd shepherd) {
        shepherd.wolfDetection.gameObject.SetActive(true);
        shepherd.shepherdGun.gameObject.SetActive(false);
        shepherd.wolfDetection.OnWolfDetected += (_, _) => shepherd.ChangeState(shepherd.hunting);

        this.shepherd = shepherd;
        aIMovement.seeker = shepherd.GetComponent<Seeker>();
        aIMovement.speed = 4;
        aIMovement.gameObject = shepherd.gameObject;
        scale = shepherd.transform.localScale;
        scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = shepherd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
        graph.SetDimensions(20, 1 ,1);
        graph.center = shepherd.transform.position;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        UpdatePath();
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        Vector2 target;
        if (shepherd.transform.position.x >= graph.center.x) {
            target = (Vector3) graph.nodes[0].position;
        } else {
            target = (Vector3) graph.nodes[graph.nodes.Length - 1].position;
        }

        return target;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        if (aIMovement.path == null) return;
        else if (aIMovement.reachedEndOfPath) UpdatePath();

        aIMovement.UpdateMovement();

        Vector3 dir = (aIMovement.path.vectorPath[aIMovement.currentWaypoint] - shepherd.transform.position).normalized;
        if (dir.x < 0) shepherd.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else if (dir.x > 0) shepherd.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    public void OnExit() {
        shepherd.astar.data.RemoveGraph(graph);
        shepherd.wolfDetection.gameObject.SetActive(false);
    }
}

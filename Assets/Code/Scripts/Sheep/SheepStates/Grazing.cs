// ---------------------------------------
// Creation Date: 4/3/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Grazing : SheepState
{
    private SheepHerd herd;
    private Sheep sheep;
    private Seeker seeker;  
    private Path path;
    private float speed = 2;
    private float nextWaypointDistance = 1;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = true;
    private bool waiting = false;

    private Vector3 scale;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;
        scale = sheep.gameObject.transform.localScale;
        seeker = sheep.gameObject.GetComponent<Seeker>();

        UpdatePath();
        herd.StartCoroutine(Stray());
    }

    private void UpdatePath() {
        if (!seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(herd.gridGraph));
        reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        GridGraph grid = herd.gridGraph;
        GraphNode node = grid.nodes[Random.Range(0, grid.nodes.Length)];
        return node.RandomPointOnSurface();
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    private IEnumerator WaitForNewMovement()
    {
        waiting = true;
        float waitTime = Random.Range(5, 20);
        yield return new WaitForSeconds(waitTime);
        waiting = false;
        UpdatePath();
    }

    public void OnUpdate() {
        if (path == null) return;
        else if (reachedEndOfPath && !waiting) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            herd.StartCoroutine(WaitForNewMovement());  
            return;
        } else if (reachedEndOfPath) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        float distanceToWaypoint;
        reachedEndOfPath = false;
        while (true) {
            distanceToWaypoint = Vector3.Distance(sheep.gameObject.transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else break;
        } 

        Vector3 dir = (path.vectorPath[currentWaypoint] - sheep.gameObject.transform.position).normalized;
        Vector3 velocity = dir * speed;
        sheep.gameObject.GetComponent<Rigidbody2D>().velocity = velocity;
        if (dir.x < 0) sheep.gameObject.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else if (dir.x > 0) sheep.gameObject.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    private IEnumerator Stray()
    {
        // actual [30, 120]
        float waitTime = Random.Range(30, 120);
        yield return new WaitForSeconds(waitTime);
        if (!herd.IsAStray()) {
            sheep.inHerd = false;
            herd.ChangeState(sheep, new Straying());
        }

    }

    public void OnExit() {}
}

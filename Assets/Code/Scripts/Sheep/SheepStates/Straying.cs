// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Straying : SheepState
{
    private GridGraph graph;
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
    private bool isFirstStray = true;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;
        scale = sheep.gameObject.transform.localScale;
        scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        seeker = sheep.gameObject.GetComponent<Seeker>();

        graph = herd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;

        graph.SetDimensions(40, 40 ,1);
        graph.center = herd.transform.localPosition;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        UpdatePath();
    }

    private void UpdatePath() {
        if (!seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos() {
        if (isFirstStray) {
            float minDist = 30;
            float maxDist = 40;

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDist, maxDist);
            Vector2 point = (Vector2) sheep.gameObject.transform.position + randomDirection * randomDistance;

            return point;
        } else {
            GraphNode node = graph.nodes[Random.Range(0, graph.nodes.Length)];
            return node.RandomPointOnSurface();
        }
    }

    private IEnumerator WaitForNewMovement()
    {
        if (isFirstStray) {
            herd.astar.data.RemoveGraph(graph);
            graph = herd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
            graph.SetDimensions(10, 10,1);
            graph.center = sheep.gameObject.transform.position;
            graph.is2D = true;
            graph.collision.use2D = true;
            AstarPath.active.Scan();
            isFirstStray = false;
        }

        waiting = true;
        float waitTime = Random.Range(5, 20);
        yield return new WaitForSeconds(waitTime);
        waiting = false;
        UpdatePath();
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
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

    public void OnExit() {}
}

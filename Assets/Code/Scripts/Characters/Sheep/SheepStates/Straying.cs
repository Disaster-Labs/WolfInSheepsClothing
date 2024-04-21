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
    private AIMovement aIMovement;
    private bool waiting = false;
    private bool isFirstStray = true;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;
        
        Animator anim = sheep.gameObject.GetComponent<Animator>();

        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 2, sheep.gameObject, anim);
        Vector3 scale = sheep.gameObject.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = herd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;

        graph.SetDimensions(40, 40 ,1);
        graph.center = herd.transform.localPosition;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        UpdatePath();
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
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
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        if (aIMovement.reachedEndOfPath && !waiting) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            herd.StartCoroutine(WaitForNewMovement());  
            return;
        } else if (aIMovement.reachedEndOfPath) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            sheep.inHerd = false;
            return;
        }

        aIMovement.UpdateMovement();
    }

    public void OnExit() {
        herd.astar.data.RemoveGraph(graph);
        herd.StopAllCoroutines();
    }
}

// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class WanderAlone : SheepState
{
    private GridGraph graph;
    private SheepHerd herd;
    private Sheep sheep;
    private AIMovement aIMovement;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;

        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 2, sheep.gameObject);
        Vector3 scale = sheep.gameObject.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = herd.astar.data.AddGraph(typeof(GridGraph)) as GridGraph;

        graph.SetDimensions(15, 15 ,1);
        graph.center = sheep.gameObject.transform.localPosition;
        graph.is2D = true;
        graph.collision.use2D = true;
        AstarPath.active.Scan();

        CheckHerds();

        UpdatePath();
        herd.StartCoroutine(InvokeUpdatePath());
    }

    private void CheckHerds()
    {
        SheepHerd[] sheepHerds = Object.FindObjectsByType<SheepHerd>(FindObjectsSortMode.None);

        foreach (SheepHerd sheepHerd in sheepHerds) {
            if (Vector2.SqrMagnitude(sheepHerd.transform.position - sheep.gameObject.transform.position) < 225) {
                sheep.inHerd = true;
            } else {
                sheep.inHerd = false;
            }
        }
    }

    private IEnumerator InvokeUpdatePath()
    {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5,15));
            UpdatePath();
            yield return null;
        }
    }

    private void UpdatePath() {
        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
    }

    private Vector2 CalculateTargetPos()
    {
        GraphNode node = graph.nodes[Random.Range(0, graph.nodes.Length)];
        return node.RandomPointOnSurface();
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        if (aIMovement.reachedEndOfPath) {
            aIMovement.speed = 0;
        } else {
            aIMovement.speed = 2;
        }
        
        aIMovement.UpdateMovement();
    }

    public void OnExit() {
        herd.astar.data.RemoveGraph(graph);
        herd.StopAllCoroutines();
    }
}

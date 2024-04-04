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
    private AIMovement aIMovement = new AIMovement();
    private bool waiting = false;

    private Vector3 scale;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;
        scale = sheep.gameObject.transform.localScale;
        aIMovement.seeker = sheep.gameObject.GetComponent<Seeker>();
        aIMovement.speed = 2;
        aIMovement.gameObject = sheep.gameObject;

        UpdatePath();
        herd.StartCoroutine(Stray());
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(herd.gridGraph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        GridGraph grid = herd.gridGraph;
        GraphNode node = grid.nodes[Random.Range(0, grid.nodes.Length)];
        return node.RandomPointOnSurface();
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
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
        if (aIMovement.path == null) return;
        else if (aIMovement.reachedEndOfPath && !waiting) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            herd.StartCoroutine(WaitForNewMovement());  
            return;
        } else if (aIMovement.reachedEndOfPath) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        aIMovement.UpdateMovement();

        Vector3 dir = (aIMovement.path.vectorPath[aIMovement.currentWaypoint] - sheep.gameObject.transform.position).normalized;
        if (dir.x < 0) sheep.gameObject.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        else if (dir.x > 0) sheep.gameObject.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    private IEnumerator Stray()
    {
        // actual [30, 120]
        float waitTime = Random.Range(0, 5);
        yield return new WaitForSeconds(waitTime);
        if (!herd.IsAStray()) {
            sheep.inHerd = false;
            herd.ChangeState(sheep, new Straying());
        }
    }

    public void OnExit() {
        herd.StopAllCoroutines();
    }
}

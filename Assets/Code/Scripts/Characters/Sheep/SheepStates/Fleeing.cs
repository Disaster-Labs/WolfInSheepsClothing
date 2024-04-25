// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using Pathfinding;
using UnityEngine;

public class Fleeing : SheepState
{
    private SheepHerd herd;
    private Sheep sheep;
    private AIMovement aIMovement;
    private GridGraph graph;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;

        Animator anim = sheep.gameObject.GetComponent<Animator>();

        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 6, sheep.gameObject, anim);

        graph = herd.graph;
        
        UpdatePath();
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        Vector2 dir = sheep.gameObject.transform.position - herd.transform.position;
        float dist = Random.Range(10, 15);
        Vector2 target = (Vector2) sheep.gameObject.transform.position + (dist * dir.normalized);
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
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            herd.StartCoroutine(WaitToGraze());
            return;
        }

        aIMovement.UpdateMovement();
    }

    private IEnumerator WaitToGraze() {
        yield return new WaitForSeconds(5);
        herd.ChangeState(sheep, new Grazing());
    }

    public void OnExit() {
        herd.StopAllCoroutines();
    }
}

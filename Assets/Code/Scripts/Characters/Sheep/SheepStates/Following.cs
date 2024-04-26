// ---------------------------------------
// Creation Date: 4/7/24
// Author: Abigail Andam
// Modified By: 
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Following : SheepState
{
    private GridGraph graph;
    private SheepHerd herd;
    private Sheep sheep;
    private AIMovement aIMovement;
    private Wolf wolf;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;

        wolf = Object.FindFirstObjectByType<Wolf>();

        Animator anim = sheep.gameObject.GetComponent<Animator>();

        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 2, sheep.gameObject, anim);
        Vector3 scale = sheep.gameObject.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = herd.graph;
        UpdatePath();
        herd.StartCoroutine(InvokeUpdatePath());
    }

    private IEnumerator InvokeUpdatePath()
    {
        while (true) {
            yield return new WaitForSeconds(1);
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
        return wolf.transform.position;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        // distance between sheep and herd should be at least ten to be able to eat the sheep
        if (Vector2.SqrMagnitude(sheep.gameObject.transform.position - herd.transform.position) > 100) {
            sheep.inHerd = false;
        } else {
            sheep.inHerd = true;
        }

        if ((wolf.transform.position - sheep.gameObject.transform.position).sqrMagnitude <= 4) {
            aIMovement.reachedEndOfPath = true;
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        aIMovement.UpdateMovement();
    }

    public void OnExit() {
        herd.StopAllCoroutines();
        aIMovement.OnExit();
    }
}

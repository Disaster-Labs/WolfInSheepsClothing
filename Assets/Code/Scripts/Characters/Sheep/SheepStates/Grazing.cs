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
    private GridGraph graph;
    private SheepHerd herd;
    private Sheep sheep;
    private AIMovement aIMovement;
    private bool waiting = false;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;

        if (sheep.gameObject == null) {
            herd.ChangeState(sheep, new Dead());
            return;
        }

        graph = herd.graph;

        Animator anim = sheep.gameObject.GetComponent<Animator>();
        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 2, sheep.gameObject, anim);

        UpdatePath();
        herd.StartCoroutine(Stray());
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        float sheepGrazeRange = 7.5f;
        float randomX = Random.Range(-sheepGrazeRange, sheepGrazeRange);
        float randomY = Random.Range(-sheepGrazeRange, sheepGrazeRange);

        return new Vector2(randomX, randomY) + (Vector2) herd.transform.position;
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
        if (aIMovement.reachedEndOfPath && !waiting) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            herd.StartCoroutine(WaitForNewMovement());  
            return;
        } else if (aIMovement.reachedEndOfPath) {
            sheep.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        aIMovement.UpdateMovement();
    }

    private IEnumerator Stray()
    {
        // time range for stray sheeps [30, 120]
        float waitTime = Random.Range(30, 120);
        yield return new WaitForSeconds(waitTime);
        if (!herd.IsAStray()) {
            herd.ChangeState(sheep, new Straying());
        }
    }

    public void OnExit() {
        herd.StopAllCoroutines();
        aIMovement.OnExit();
    }
}

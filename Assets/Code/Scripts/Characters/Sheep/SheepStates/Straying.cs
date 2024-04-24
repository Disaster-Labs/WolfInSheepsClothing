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

        graph = herd.graph;

        UpdatePath();
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(sheep.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 strayCenter;

    private Vector2 CalculateTargetPos() {
        if (isFirstStray) {
            float minDist = 25;
            float maxDist = 30;

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDist, maxDist);
            Vector2 point = (Vector2) sheep.gameObject.transform.position + randomDirection * randomDistance;

            return point;
        } else {
            float sheepGrazeRange = 7.5f;
            float randomX = Random.Range(-sheepGrazeRange, sheepGrazeRange);
            float randomY = Random.Range(-sheepGrazeRange, sheepGrazeRange);

            return new Vector2(randomX, randomY) + strayCenter;
        }
    }

    private IEnumerator WaitForNewMovement()
    {
        if (isFirstStray) {
            strayCenter = sheep.gameObject.transform.position;
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
        herd.StopAllCoroutines();
    }
}

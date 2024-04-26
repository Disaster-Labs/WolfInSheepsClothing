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
    
    private Vector2 strayCenter;

    public void OnEnter(SheepHerd herd, Sheep sheep) {
        this.herd = herd;
        this.sheep = sheep;

        Animator anim = sheep.gameObject.GetComponent<Animator>();
        aIMovement = new AIMovement(sheep.gameObject.GetComponent<Seeker>(), 2, sheep.gameObject, anim);
        Vector3 scale = sheep.gameObject.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = herd.graph;
        strayCenter = sheep.gameObject.transform.position;

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
        float sheepGrazeRange = 7.5f;
            float randomX = Random.Range(-sheepGrazeRange, sheepGrazeRange);
            float randomY = Random.Range(-sheepGrazeRange, sheepGrazeRange);

        return new Vector2(randomX, randomY) + strayCenter;
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
        herd.StopAllCoroutines();
        aIMovement.OnExit();
    }
}

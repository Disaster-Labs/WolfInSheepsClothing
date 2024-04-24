// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

public class Patrolling : ShepherdState {
    private Shepherd shepherd;
    private AIMovement aIMovement;
    private GridGraph graph;
    private bool onFirstPathReached = false;
    private float startTime;
    private float startRot = -720;

    public void OnEnter(Shepherd shepherd) {
        startTime = Time.time;

        if (startRot == -720) {
            startRot = shepherd.wolfDetection.transform.parent.transform.rotation.eulerAngles.z - 360;
        }

        shepherd.wolfDetection.gameObject.SetActive(false);
        shepherd.wolfDetection.OnWolfDetected += (_, _) => shepherd.ChangeState(shepherd.hunting);

        this.shepherd = shepherd;

        Animator anim = shepherd.transform.GetChild(0).GetComponent<Animator>();
        aIMovement = new AIMovement(shepherd.GetComponent<Seeker>(), 4, shepherd.gameObject, anim);

        Vector3 scale = shepherd.transform.localScale;
        aIMovement.scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

        graph = shepherd.gridGraph;

        DoFirstPath();
    }

    private void DoFirstPath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = shepherd.startPos;
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private void UpdatePath() {
        if (!aIMovement.seeker.IsDone()) return;

        Vector2 targetPos = CalculateTargetPos();
        aIMovement.seeker.StartPath(shepherd.gameObject.transform.position, targetPos, OnPathComplete, GraphMask.FromGraph(graph));
        aIMovement.reachedEndOfPath = false;
    }

    private Vector2 CalculateTargetPos()
    {
        float range = 8;
        Vector2 target;

        switch (shepherd.shepherdPathType) {
            case ShepherdPathType.Vertical:
                if (shepherd.transform.position.y >= shepherd.startPos.y + range) {
                    target = new Vector2(shepherd.transform.position.x, shepherd.startPos.y - range);
                } else {
                    target = new Vector2(shepherd.transform.position.x, shepherd.startPos.y + range);
                }
                break;
            case ShepherdPathType.Horizontal:
                if (shepherd.transform.position.x >= shepherd.startPos.x + range) {
                    target = new Vector2(shepherd.startPos.x - range, shepherd.transform.position.y);
                } else {
                    target = new Vector2(shepherd.startPos.x + range, shepherd.transform.position.y);
                }
                break;
            case ShepherdPathType.Turn:
                target = shepherd.transform.position;
                break;
            default:
                target = shepherd.transform.position;;
                break;
        }

        return target;
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            aIMovement.path = p;
            aIMovement.currentWaypoint = 0;
        }
    }

    public void OnUpdate() {
        if (aIMovement.path == null) return;
        else if (aIMovement.reachedEndOfPath && !onFirstPathReached) {
            onFirstPathReached = true;
        }
        else if (aIMovement.reachedEndOfPath) UpdatePath();

        aIMovement.UpdateMovement();

        if (onFirstPathReached) HandleRotation();
    }
    
    private bool reverse = false;

    private void HandleRotation() {
        Rigidbody2D rb = shepherd.GetComponent<Rigidbody2D>();
        shepherd.wolfDetection.gameObject.SetActive(true);
        
        if (shepherd.shepherdPathType != ShepherdPathType.Turn) {
            if (Mathf.Abs(rb.velocity.x) < 0.2f && rb.velocity.y < 0) {
                shepherd.visual.sprite = shepherd.shepherdDown;
            } else if (Mathf.Abs(rb.velocity.x) < 0.2f && rb.velocity.y > 0) {
                shepherd.visual.sprite = shepherd.shepherdUp;
            } else {
                shepherd.visual.sprite = shepherd.shepherdSide;
            }
        } else {
            shepherd.visual.sprite = shepherd.shepherdDown;
            shepherd.transform.localScale = new Vector3(1, 1, 1);

            float panTime = 4;
            if (Time.time - startTime > panTime) {
                startTime = Time.time;
                reverse = !reverse;
            }

            // have an animation for turning
            float rotationZ = Mathf.Lerp(startRot, -startRot, (Time.time - startTime) / panTime);
            rotationZ *= reverse ? -1 : 1;
            shepherd.wolfDetection.transform.parent.transform.rotation = Quaternion.Euler(0, 0 ,rotationZ);
        }
    }

    public void OnExit() {
        shepherd.wolfDetection.gameObject.SetActive(false);
        onFirstPathReached = false;
        shepherd.visual.sprite = shepherd.shepherdSide;
    }
}

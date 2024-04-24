// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AIMovement
{
    public Seeker seeker;  
    public Path path;
    public float speed;
    public float nextWaypointDistance = 1;
    public int currentWaypoint = 0;
    public bool reachedEndOfPath = true;
    public GameObject gameObject;

    public Vector3 scale;

    private float timePassedSinceFlip = 0.21f;

    // Animations
    private Animator anim;

    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string DIRECTION = "Direction";
    private enum Direction {
        Side,
        Up,
        Down
    }

    public AIMovement(Seeker seeker, float speed, GameObject gameObject, Animator anim) {
        this.seeker = seeker;
        this.speed = speed;
        this.gameObject = gameObject;
        this.anim = anim;
        scale = gameObject.transform.localScale;
    }

    public void UpdateMovement() {
        timePassedSinceFlip += Time.deltaTime;

        if (path == null) return;

        float distanceToWaypoint;
        reachedEndOfPath = false;
        while (true) {
            distanceToWaypoint = Vector3.SqrMagnitude(gameObject.transform.position - path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < Mathf.Pow(nextWaypointDistance, 2)) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    anim.SetBool(IS_WALKING, false);
                    anim.SetBool(IS_RUNNING, false);
                    reachedEndOfPath = true;
                    break;
                }
            } else break;
        } 

        Vector3 dir = (path.vectorPath[currentWaypoint] - gameObject.transform.position).normalized;
        Vector3 velocity = dir * speed;

        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = velocity;

        if (dir.x < 0 && timePassedSinceFlip > 0f) {
            gameObject.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
            timePassedSinceFlip = 0;
        } else if (dir.x > 0 && timePassedSinceFlip > 0f) {
            gameObject.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
            timePassedSinceFlip = 0;
        }

        if (!reachedEndOfPath && speed == 2) {
            anim.SetBool(IS_WALKING, dir != Vector3.zero);
        } else if (!reachedEndOfPath) {
            anim.SetBool(IS_RUNNING, dir != Vector3.zero);
        }

        if (Mathf.Approximately(rb.velocity.x, 0) && rb.velocity.y < 0) {
            anim.SetInteger(DIRECTION, (int)Direction.Down);
        } else if (Mathf.Approximately(rb.velocity.x, 0) && rb.velocity.y > 0) {
            anim.SetInteger(DIRECTION, (int)Direction.Up);
        } else {
            anim.SetInteger(DIRECTION, (int)Direction.Side);
        }

    }
}

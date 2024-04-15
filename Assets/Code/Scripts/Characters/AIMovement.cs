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

    public AIMovement(Seeker seeker, float speed, GameObject gameObject) {
        this.seeker = seeker;
        this.speed = speed;
        this.gameObject = gameObject;
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
                    reachedEndOfPath = true;
                    break;
                }
            } else break;
        } 

        Vector3 dir = (path.vectorPath[currentWaypoint] - gameObject.transform.position).normalized;
        Vector3 velocity = dir * speed;
        gameObject.GetComponent<Rigidbody2D>().velocity = velocity;

        if (dir.x < 0 && timePassedSinceFlip > 0.2f) {
            gameObject.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            timePassedSinceFlip = 0;
        } else if (dir.x > 0 && timePassedSinceFlip > 0.2f) {
            gameObject.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
            timePassedSinceFlip = 0;
        }
    }
}

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

    public void UpdateMovement() {
        float distanceToWaypoint;
        reachedEndOfPath = false;
        while (true) {
            distanceToWaypoint = Vector3.Distance(gameObject.transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
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
    }
}

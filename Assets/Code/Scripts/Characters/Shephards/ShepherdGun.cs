// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using UnityEngine;

public class ShepherdGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private LineRenderer lineRenderer;

    private Vector2 wolfPos;
    private GameObject bullet;

    public void ShootAtPosition(Vector2 pos) {
        wolfPos = pos;
    }

    private void OnEnable() {
        lineRenderer = GetComponent<LineRenderer>();

        InvokeRepeating("ShootWolf", 2, 4);
    }

    private void OnDisable() {
        CancelInvoke();
        Destroy(bullet);
    }

    private void ShootWolf() {
        // within 10 meters
        if ((wolfPos - (Vector2) transform.position).sqrMagnitude < 400) {
            StartCoroutine(IncomingShot());
            CancelInvoke();
            InvokeRepeating("ShootWolf", 4, 4);
            Debug.Log("Within 10 meters");
        } else {
            CancelInvoke();
            InvokeRepeating("ShootWolf", 0.2f, 0.2f);
            Debug.Log("Not within 10 meters");
        }
    }

    private IEnumerator IncomingShot()
    {
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.numCapVertices = 3;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        float startTime = Time.time;
        float shotPrepTime = 2f;
        // Queue<Vector2> queue = new Queue<Vector2>();
        // while (Time.time - startTime < shotPrepTime) {
        //     queue.Enqueue(wolfPos);

        //     if (Time.time - startTime > 0.2f) {
        //         lineRenderer.SetPosition(0, transform.position);
        //         lineRenderer.SetPosition(1, queue.Dequeue());
        //     }

        //     yield return null;
        // }

        Vector2 offset = Vector2.zero;
        float dir = 1;
        while (Time.time - startTime < shotPrepTime) {
            lineRenderer.SetPosition(0, transform.position);
            Vector2 target = wolfPos + offset;
            lineRenderer.SetPosition(1, target);

            Vector2 shepherdToWolf = wolfPos - (Vector2) transform.position;
            Vector2 dOffset = new Vector2(-shepherdToWolf.y, shepherdToWolf.x).normalized * 0.01f * dir;

            if (offset.sqrMagnitude >= 4) {
                dir *= -1;
            } 

            offset += dOffset;

            yield return null;
        }
        
        // play shooting sound
        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;
        bullet = Instantiate(bulletPrefab);
        // Vector2 target = wolfPos;
         
        // startTime = Time.time;
        // float bulletTravelTime = 0.35f;
        // while (Time.time - startTime < bulletTravelTime) {
        //     Quaternion bulletRotation = Quaternion.FromToRotation(Vector3.right, (Vector3) wolfPos - transform.position);
        //     bullet.transform.rotation = bulletRotation;
        //     bullet.transform.position = Vector2.Lerp(transform.position, wolfPosOnShot, (Time.time - startTime) / bulletTravelTime);
        //     yield return null;
        // }

        // startTime = Time.time;
        // float bulletDestroyTime = 0.5f;
        // while (Time.time - startTime < bulletDestroyTime) {
        //     bullet.transform.position += ((Vector3) wolfPos - transform.position).normalized * Time.deltaTime * 20f;
        //     yield return null;
        // }

        Destroy(bullet);
    }
}

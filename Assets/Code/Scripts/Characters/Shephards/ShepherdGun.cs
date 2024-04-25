// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShepherdGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private LineRenderer lineRenderer;

    private Vector2 wolfPosOnShot;
    private Vector2 wolfPos;
    private GameObject bullet;
    private Animator anim;

    private const string IS_SHOOTING = "IsShooting";

    public void ShootAtPosition(Vector2 pos) {
        wolfPos = pos;
    }

    private void Start() {
        anim = transform.parent.GetChild(0).GetComponent<Animator>();
    }

    private void OnEnable() {
        lineRenderer = GetComponent<LineRenderer>();

        InvokeRepeating("ShootWolf", 4, 4);
    }

    private void OnDisable() {
        CancelInvoke();
        Destroy(bullet);
    }

    private void ShootWolf() {
        StartCoroutine(IncomingShot());
    }

    private IEnumerator IncomingShot()
    {
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.numCapVertices = 3;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        float startTime = Time.time;
        float shotPrepTime = 1f;
        while (Time.time - startTime < shotPrepTime) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, wolfPos);

            if (Time.time - startTime >= shotPrepTime - 0.2f) anim.SetBool(IS_SHOOTING, true);
            yield return null;
        }

        wolfPosOnShot = wolfPos;
        
        // play shooting sound
        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;
        bullet = Instantiate(bulletPrefab);
         
        startTime = Time.time;
        float bulletTravelTime = 0.35f;
        while (Time.time - startTime < bulletTravelTime) {
            Quaternion bulletRotation = Quaternion.FromToRotation(Vector3.right, (Vector3) wolfPos - transform.position);
            bullet.transform.rotation = bulletRotation;
            bullet.transform.position = Vector2.Lerp(transform.position, wolfPosOnShot, (Time.time - startTime) / bulletTravelTime);
            yield return null;
        }

        startTime = Time.time;
        float bulletDestroyTime = 0.5f;
        while (Time.time - startTime < bulletDestroyTime) {
            bullet.transform.position += ((Vector3) wolfPos - transform.position).normalized * Time.deltaTime * 20f;
            yield return null;
        }

        Destroy(bullet);
        anim.SetBool(IS_SHOOTING, false);
    }
}

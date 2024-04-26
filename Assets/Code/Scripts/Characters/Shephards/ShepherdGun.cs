// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using UnityEngine;

public class ShepherdGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private LineRenderer lineRenderer;

    private Vector2 wolfPos;
    private GameObject bullet;
    private Animator anim;

    private const string IS_SHOOTING = "IsShooting";

    // Sound
    public event EventHandler OnShot;

    public void ShootAtPosition(Vector2 pos) {
        wolfPos = pos;
    }

    private void Start() {
        anim = transform.parent.GetChild(0).GetComponent<Animator>();
    }

    private void OnEnable() {
        lineRenderer = GetComponent<LineRenderer>();

        InvokeRepeating("ShootWolf", 2, 4);
    }

    private void OnDisable() {
        CancelInvoke();
        StopAllCoroutines();
        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;
        Destroy(bullet);
    }

    private void ShootWolf() {
        // within 10 meters
        if ((wolfPos - (Vector2) transform.position).sqrMagnitude < 400) {
            StartCoroutine(IncomingShot());
            CancelInvoke();
            InvokeRepeating("ShootWolf", 4, 4);
        } else {
            CancelInvoke();
            InvokeRepeating("ShootWolf", 0.2f, 0.2f);
        }
    }

    private IEnumerator IncomingShot()
    {
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.numCapVertices = 3;
        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;

        float startTime = Time.time;
        float shotPrepTime = 2f;

        // Queue based system
        // Queue wolfPoses = new Queue();

        // while (Time.time - startTime < shotPrepTime) {
        //     if (Time.time - startTime > 0.3f) {
        //         lineRenderer.SetPosition(0, transform.position);
        //         Vector2 pointAtDir = -((Vector2) transform.position - (Vector2) wolfPoses.Dequeue()).normalized;
        //         Vector2 pointAt = (Vector2) transform.position + pointAtDir * 40;

        //         bool hitWolf = Physics2D.Linecast(transform.position, pointAt, wolf);

        //         lineRenderer.SetPosition(1, hitWolf ? wolfPos : pointAt);
        //     } 
            
        //     wolfPoses.Enqueue(wolfPos);
            
        //     yield return null;
        // }


        while (Time.time - startTime < shotPrepTime) {
            lineRenderer.SetPosition(0, transform.position);
            // Vector2 pointAtDir = -((Vector2) transform.position - wolfPos).normalized;
            // Vector2 pointAt = (Vector2) transform.position + pointAtDir * 40;
            lineRenderer.SetPosition(1, wolfPos);

            if (Time.time - startTime < 0.2) {
                lineRenderer.startColor = new Color(1, 0, 0, 0.7f);
                lineRenderer.endColor = new Color(1, 0, 0, 0.7f);
            }

            if (Time.time - startTime >= shotPrepTime - 0.2f) anim.SetBool(IS_SHOOTING, true);
            yield return null;
        }
        
        // play shooting sound
        lineRenderer.startColor = Color.clear;
        lineRenderer.endColor = Color.clear;

        Vector2 dir = (wolfPos - (Vector2) transform.position).normalized;
        Quaternion bulletRotation = Quaternion.FromToRotation(Vector3.right, dir);
        bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
        OnShot?.Invoke(this, EventArgs.Empty);
         
        startTime = Time.time;
        float bulletLifeSpan = 3f;
        while (Time.time - startTime < bulletLifeSpan) {
            if (Time.time - startTime >= 0.5) anim.SetBool(IS_SHOOTING, false);

            // movement
            bullet.transform.position += (Vector3) dir * 30 * Time.deltaTime;
            yield return null;
        }

        Destroy(bullet);
    }
}

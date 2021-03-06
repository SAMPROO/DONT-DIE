﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform projectileOrigin;
    public GameObject projectilePrefab;

    public float initialVelocity;
    public float accuracy;
    public float lifetime;

    public float roundPerSecond = 3;
    float timer = 0;

    [Space]
    public bool debug;
    public int resolution;
    public float reticleDistance;

    private void Update()
    {
        if (timer <= 0)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)) // left mouse button = semiauto, right mouse button = fullauto
            {
                GameObject bullet = Instantiate(projectilePrefab, projectileOrigin.position, Quaternion.identity);

                Vector3 direction = (Vector3)Random.insideUnitCircle + Vector3.forward * accuracy;
                direction = Quaternion.FromToRotation(Vector3.forward, projectileOrigin.forward) * direction;

                bullet.GetComponent<Rigidbody>().velocity = direction.normalized * initialVelocity;

                Destroy(bullet, lifetime);

                timer = 1f / roundPerSecond;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (debug)
        {
            AccuracyReticle(resolution, reticleDistance);
        }        
    }

    void AccuracyReticle(int CircleVertexCount, float reticleDistance)
    {
        float segmentWidth = Mathf.PI * 2f / CircleVertexCount;
        float angle = 0f;
        float distanceMultiplier = reticleDistance / accuracy;

        Vector3 v0, v1, r0, r1;

        v0 = GetPointOnCircle(angle, distanceMultiplier);
        r0 = Raycast(v0);

        for (int i = 1; i < CircleVertexCount + 1; i++)
        {
            angle -= segmentWidth;

            v1 = GetPointOnCircle(angle, distanceMultiplier);
            r1 = Raycast(v1);

            Debug.DrawLine(v0, v1, Color.green);
            Debug.DrawLine(r0, r1, Color.blue);

            v0 = v1;
            r0 = r1;
        }
    }

    Vector3 GetPointOnCircle(float angle, float distanceMultiplier)
    {
        Vector3 vertex = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        vertex += Vector3.up * accuracy;
        vertex = Quaternion.FromToRotation(Vector3.up, projectileOrigin.forward) * vertex;
        vertex = distanceMultiplier * vertex;
        vertex += projectileOrigin.position;

        return vertex;
    }

    Vector3 Raycast(Vector3 vertex)
    {
        Vector3 origin = projectileOrigin.position;
        Vector3 direction = (vertex - projectileOrigin.position).normalized;

        float distance = initialVelocity * lifetime;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~(1 << 9)))
        {
            return hit.point;
        }

        return origin + direction * distance;
    }
}

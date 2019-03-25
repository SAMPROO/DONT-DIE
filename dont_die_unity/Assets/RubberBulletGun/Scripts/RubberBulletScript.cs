using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBulletScript : MonoBehaviour
{
    Rigidbody rb;
    float speed;
    Vector3 heading;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = rb.velocity.magnitude;
        heading = rb.velocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 averageNormal = Vector3.zero;

        foreach (var contact in collision.contacts)
        {
            averageNormal += contact.normal;
        }
        averageNormal = averageNormal / collision.contactCount;

        heading = Vector3.Reflect(heading, averageNormal);

        rb.velocity = heading * speed;
    }
}

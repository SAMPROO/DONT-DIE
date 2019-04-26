using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchGun : Equipment
{
    public float initialSpeed, waitTime, reelSpeed, minReelDistance;
    public Rigidbody boxingGlove;

    private Vector3 startPosition;
    private bool isLaunched, beingReeled, drawLine;

    private FixedJoint gloveJoint;
    private LineRenderer lineRenderer;

    private void Start()
    {
        startPosition = boxingGlove.transform.localPosition;

        // seperate boxing glove from the gun so that if boxing glove stops it doesn't follow the parrent physics.
        boxingGlove.transform.parent = null;

        // make a new fixed joint between gun and glove
        gloveJoint = gameObject.AddComponent<FixedJoint>();
        gloveJoint.connectedBody = boxingGlove.GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        if (drawLine)
        {
            Vector3[] positions = { transform.TransformPoint(startPosition), boxingGlove.position };

            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (beingReeled)
        {
            Vector3 direction = transform.TransformPoint(startPosition) - boxingGlove.position;

            if (direction.magnitude > minReelDistance)
            {
                // zero velocity in reeling direction
                boxingGlove.velocity -= Vector3.Project(boxingGlove.velocity, direction);

                // move glove towards start position
                boxingGlove.MovePosition(boxingGlove.transform.position + direction.normalized * reelSpeed * Time.deltaTime);

                // point glove away from start position
                boxingGlove.MoveRotation(Quaternion.LookRotation(-direction.normalized));
            }
            else
            {
                // make glove temporary kinematic to ignore collisions
                boxingGlove.isKinematic = true;

                // once start position is reached, fix glove back to start position
                beingReeled = false;
                isLaunched = false;
                boxingGlove.transform.position = transform.TransformPoint(startPosition);
                boxingGlove.transform.rotation = Quaternion.LookRotation(transform.forward);

                // make a new fixed joint between gun and glove
                gloveJoint = gameObject.AddComponent<FixedJoint>();
                gloveJoint.connectedBody = boxingGlove.GetComponent<Rigidbody>();

                boxingGlove.isKinematic = false;

                drawLine = false;
                lineRenderer.enabled = false;
            }
        }
    }

    public override void Use()
    {
        if (!isLaunched && Ammo > 0)
        {
            isLaunched = true;

            // destroy the fixed joint between gun and glove
            Destroy(gloveJoint);
            gloveJoint = null;

            // set initial velocity
            boxingGlove.velocity = transform.forward * initialSpeed;

            if (Ammo > 1)
            {
                drawLine = true;
                Invoke("ReturnToSender", waitTime);
            }
            else
            {
                // TODO: play a sound as line snaps on last punch...
            }

            Ammo--;
        }
        else
        {
            // Projectile is already launched
        }
    }

    private void ReturnToSender()
    {
        // start reeling the glove back in
        beingReeled = true;
    }

    public override void Destroy()
    {
        // destroy the boxing glove
        Destroy(boxingGlove.gameObject);

        // continue with the base destroy funtion from Equipment
        base.Destroy();
    }
}

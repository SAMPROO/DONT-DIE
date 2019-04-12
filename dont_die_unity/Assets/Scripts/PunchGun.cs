using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchGun : Equipment
{
    public float force;
    public Rigidbody BoxingGlove;

    public float waitTime;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = BoxingGlove.transform.localPosition;
    }

    public override void Use()
    {
        if (BoxingGlove.isKinematic)
        {
            Debug.Log("Punch!!!");

            BoxingGlove.isKinematic = false;
            BoxingGlove.AddForce(transform.forward * force);

            Invoke("ReturnToSender", waitTime);
        }
        else
        {
            Debug.Log("Can't punch while puncing...");
        }
    }

    private void ReturnToSender()
    {
        // reel the glove back in...

        BoxingGlove.isKinematic = true;
        BoxingGlove.transform.localPosition = startPosition;
        BoxingGlove.transform.localRotation = Quaternion.identity;
    }
}

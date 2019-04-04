using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchGun : MonoBehaviour, IWeapon
{
    public float force;
    public Rigidbody BoxingGlove;

    public float waitTime;

    private Vector3 startPosition;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = BoxingGlove.transform.localPosition;
    }

    public void Use()
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

    public void StartCarrying(Transform carrier)
    {
        transform.SetParent(carrier);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rb.isKinematic = true;

        //isCarried = true;

        Debug.Log("PunchGun hops on");
    }

    public void StopCarrying()
    {
        transform.SetParent(null);
        rb.isKinematic = false;

        //isCarried = false;

        Debug.Log("PunchGun thrown away");
    }

    private void ReturnToSender()
    {
        // reel the glove back in...

        BoxingGlove.isKinematic = true;
        BoxingGlove.transform.localPosition = startPosition;
        BoxingGlove.transform.localRotation = Quaternion.identity;
    }
}

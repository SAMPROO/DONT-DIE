using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PunchGun : MonoBehaviour, IWeapon
{
    public float force;
    public Rigidbody boxingGlove;

    public float waitTime;

    private Vector3 startPosition;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = boxingGlove.transform.localPosition;
    }

    public void Use()
    {
        if (boxingGlove.isKinematic)
        {
            Debug.Log("Punch!!!");

            boxingGlove.isKinematic = false;
            boxingGlove.AddForce(transform.forward * force);

            Invoke(nameof(ReturnToSender), waitTime);
        }
        else
        {
            Debug.Log("Can't punch while punching...");
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

        boxingGlove.isKinematic = true;
        boxingGlove.transform.localPosition = startPosition;
        boxingGlove.transform.localRotation = Quaternion.identity;
    }
}

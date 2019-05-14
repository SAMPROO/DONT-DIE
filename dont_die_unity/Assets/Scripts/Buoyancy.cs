using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float buoyancy;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().AddForce(-Physics.gravity * buoyancy, ForceMode.Acceleration);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInParent<Rigidbody>().AddForce(-Physics.gravity * buoyancy * 2.5f, ForceMode.Acceleration);
        }
    }
}

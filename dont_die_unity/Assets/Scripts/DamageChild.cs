using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageChild : MonoBehaviour
{
    // multiply all damage done to this specific part
    public float damageMultiplier = 1;

    public bool isHead;

    [HideInInspector]
    public DamageController damageController;

    private void OnCollisionEnter(Collision collision)
    {
        // Do not hit itself
        if (collision.collider.transform.root == transform.root)
            return;

        damageController.CalculateImpactDamage(collision, damageMultiplier);

        OnTriggerEnter(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnTriggerExit(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TouchDamage>())
        {
            var td = other.GetComponent<TouchDamage>();

            if (!td.headOnly || isHead)
            {
                Debug.Log(gameObject.name);

                // add to any existing touch damage incase player is touching multiple touch damage bearing gameobjects
                damageController.AddTouchDamage(td.touchDamage * damageMultiplier);
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TouchDamage>())
        {
            var td = other.GetComponent<TouchDamage>();

            if (!td.headOnly || isHead)
            {
                // remove touch damage caused by the gameobject the player is no longer touching
                damageController.AddTouchDamage(-td.touchDamage * damageMultiplier);
            }
        }
    }
}

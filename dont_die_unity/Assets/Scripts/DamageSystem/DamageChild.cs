using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageChild : MonoBehaviour
{
    // multiply all damage done to this specific part
    public float damageMultiplier = 1;

    // if negative use default in DamageController
    public float minimumFallHeight = -1;

    // velocity at which this specific part needs to hit something to take damage calculated from fall height
    [HideInInspector] public float minimumVelocity;

    [HideInInspector]
    public DamageController damageController;

    private void Start()
    {
        // calculation to get minimumVelocity from minimumFallHeight
        if (minimumFallHeight >= 0)
            minimumVelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * minimumFallHeight);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Do not hit itself
        if (collision.collider.transform.root == transform.root)
            return;

        damageController.CalculateImpactDamage(collision, this);

        if (collision.gameObject.GetComponent<TouchDamage>())
        {
            // add to any existing touch damage incase player is touching multiple touch damage bearing gameobjects
            damageController.AddTouchDamage(collision.gameObject.GetComponent<TouchDamage>().touchDamage * damageMultiplier);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Do not unhit itself
        if (collision.collider.transform.root == transform.root)
            return;

        if (collision.gameObject.GetComponent<TouchDamage>())
        {
            // remove touch damage caused by the gameobject the player is no longer touching
            damageController.AddTouchDamage(- collision.gameObject.GetComponent<TouchDamage>().touchDamage * damageMultiplier);
        }
    }
}

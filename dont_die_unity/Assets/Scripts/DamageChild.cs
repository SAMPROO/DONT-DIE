using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageChild : MonoBehaviour
{
    // multiply all damage done to this specific part
    public float damageMultiplier = 1;

    [HideInInspector]
    public DamageController damageController;

    private void OnCollisionEnter(Collision collision)
    {
        // Do not hit itself
        if (collision.collider.transform.root == transform.root)
            return;

        damageController.CalculateImpactDamage(collision, damageMultiplier);

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

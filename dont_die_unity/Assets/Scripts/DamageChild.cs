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
        damageController.CalculateImpactDamage(collision, damageMultiplier);

        if (collision.gameObject.GetComponent<TouchDamage>())
        {
            Debug.Log("Enter: " + collision.collider.name);

            // add to any existing touch damage incase player is touching multiple touch damage bearing gameobjects
            damageController.AddTouchDamage(collision.gameObject.GetComponent<TouchDamage>().touchDamage * damageMultiplier);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<TouchDamage>())
        {
            Debug.Log("Exit: " + collision.collider.name);

            // remove touch damage caused by the gameobject the player is no longer touching
            damageController.AddTouchDamage(- collision.gameObject.GetComponent<TouchDamage>().touchDamage * damageMultiplier);
        }
    }
}

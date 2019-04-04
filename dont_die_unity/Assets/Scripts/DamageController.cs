using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float>{}

public class DamageController : MonoBehaviour
{
    // minimum heigh/distance at 1g acceleration before damage
    public float minimumFallHeight;

    // minimum relative velocity before any impact damage
    private float minimumVelocity;

    // any touchDamage is repeated every tick
    public float tickRate;

    [Header("(Use \"Dynamic float\" option)")]
    public FloatEvent TakeDamage = new FloatEvent();

    private float touchDamage = 0;
    private List<float> touchDamages = new List<float>();

    private void Start()
    {
        // pass this script to all damageChilds
        foreach (var child in GetComponentsInChildren<DamageChild>())
        {
            child.damageController = this;
        }

        // calculation to get minimumVelocity from minimumFallHeight
        minimumVelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * minimumFallHeight);
    }

    public void CalculateImpactDamage(Collision collision, float damageMultiplier)
    {
        // velocity relative to other gameobject at the time of the collision
        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > minimumVelocity)
        {
            float impactDamage = 1;

            int impactDamageType = 1;

            if (collision.gameObject.GetComponent<ImpactDamage>())
            {
                var id = collision.gameObject.GetComponent<ImpactDamage>();
                impactDamageType = (int)id.type;
                impactDamage *= id.damageAmount;
            }

            if (impactDamageType == 1) // damage is based on relative velocity
            {
                // consider velocity over minimum as damage
                impactDamage *= impactVelocity - minimumVelocity;
            }

            // multiply damage depending on which part got hit
            impactDamage *= damageMultiplier;

            // if there is any damage. Invoke custom unityEvent "FloatEvent" and pass damage (float) as an argument
            if (impactDamage > 0)
            {
                TakeDamage.Invoke(impactDamage);
            }
        }
    }

    // add or remove touch damage
    public void AddTouchDamage(float amount)
    {
        if (amount > 0)
        {
            touchDamages.Add(amount);
        }
        else
        {
            touchDamages.Remove(-amount);
        }

        if (touchDamages.Count > 0)
        {
            touchDamage = Mathf.Max(touchDamages.ToArray());
        }
        else touchDamage = 0;

        // if there is any touchDamage it is repeatedly applied to player
        if (!IsInvoking("TakeTouchDamage") && touchDamage > 0)
        {
            InvokeRepeating("TakeTouchDamage", 0, tickRate);
        }
        else if (IsInvoking("TakeTouchDamage") && touchDamage == 0)
        {
            CancelInvoke("TakeTouchDamage");
        }
    }

    private void TakeTouchDamage()
    {
        TakeDamage.Invoke(touchDamage);
    }
}

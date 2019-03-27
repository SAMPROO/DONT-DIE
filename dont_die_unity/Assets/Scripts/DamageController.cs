using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float>{}

public class DamageController : MonoBehaviour
{
    // minimum relative velocity before any impact damage
    public float minVelocity;

    // any touchDamage is repeated with delay
    public float touchDamageRepeatDelay;

    [Header("(Use \"Dynamic float\" option)")]
    public FloatEvent TakeDamage = new FloatEvent();

    float touchDamage = 0;

    private void Start()
    {
        foreach (var child in GetComponentsInChildren<DamageChild>())
        {
            child.damageController = this;
        }
    }

    public void CalculateImpactDamage(Collision collision, float damageMultiplier)
    {
        // velocity relative to other gameobject at the time of the collision
        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > minVelocity)
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
                impactDamage *= impactVelocity - minVelocity;
            }

            // multiply damage depending on which part got hit
            impactDamage *= damageMultiplier;

            // if there is any damage. Invoke custom unityEvent "FloatEvent" and pass damage (float) as an argument
            if (impactDamage > 0)
            {
                TakeDamage.Invoke(impactDamage);
                Debug.Log("Impact damage: " + impactDamage);
            }

            // could be later just hard coded similar to:
            // player.GetComponent<PlayerController>().TakeDamage(impactDamage);
        }
    }

    // add or remove touch damage
    public void AddTouchDamage(float amount)
    {
        touchDamage += amount;

        // clamp minimum to 0
        touchDamage = Mathf.Max(touchDamage, 0);

        // if there is any touchDamage it is repeatedly applied to player
        if (!IsInvoking("TakeTouchDamage") && touchDamage > 0)
        {
            InvokeRepeating("TakeTouchDamage", 0, touchDamageRepeatDelay);
        }
        else if (IsInvoking("TakeTouchDamage") && touchDamage == 0)
        {
            CancelInvoke("TakeTouchDamage");
        }
    }

    void TakeTouchDamage()
    {
        TakeDamage.Invoke(touchDamage);
        Debug.Log("Touch damage: " + touchDamage);
    }
}

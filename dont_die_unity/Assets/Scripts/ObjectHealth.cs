using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float blastRadius = 5f;
    public float blastForce = 100f;
    public float upwardsModifier = 0;
    public float health;
    public float damageThreshold;
    public GameObject effect;
    public float explosionDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        CalculateImpact(collision.relativeVelocity);
    }

    public void CalculateImpact(Vector3 impact)
    {
        float forceTotal = impact.x + impact.y + impact.z;
        forceTotal *= forceTotal;
        if(forceTotal>damageThreshold)
            health -= forceTotal;
        if (health < 0)
            Explode();

    }
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            RagdollRig ragdollRig = nearbyObject.GetComponent<RagdollRig>();

            if (rb != null)
            {
                //if (ragdollRig != null)
                //ragdollRig.DoConcussion();
                rb.AddExplosionForce(blastForce, transform.position, blastRadius, upwardsModifier, ForceMode.VelocityChange);
                //Play Explosion SFX
            }
            
        }
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                Debug.Log("FOUND IT!!");
                nearbyObject.GetComponentInParent<StatusHelper>().pc.Hurt(explosionDamage);
            }
            break;
        }
        
        Destroy(Instantiate(effect, transform.position, Quaternion.identity),3);
        Destroy(this.gameObject,0);
    }
}

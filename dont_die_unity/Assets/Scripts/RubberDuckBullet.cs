using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class RubberDuckBullet : MonoBehaviour
{
    
    public GameObject explosionEffect;
    public float delay = 3f;
    public float blastRadius = 5f;
    public float blastForce = 100f;
    public float upwardsModifier = 0;
    
    public bool exploadOnCollision = true;
    public bool destroyAfterScale = true;
    private bool hasCollided = false;
    
    public float scaleMultiplier = 1f;
    public float timeToScale = 1f;
    
    //private Vector3 startScale;
    //private Vector3 endScale;

    private void Start()
    {
        //startScale = transform.localScale;
        //endScale = startScale * scaleMultiplier;
        
        if (exploadOnCollision == false)
            StartCoroutine(Expload(delay));
    }

    private IEnumerator Expload(float delay = 0f)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        
        //float i = 0f;
//
        //while (i < timeToScale)
        //{
        //    i += Time.deltaTime;
        //    //transform.localScale = Vector3.Lerp(startScale, endScale, i / timeToScale);
        //    yield return null;
        //}

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            RagdollRig ragdollRig = nearbyObject.GetComponent<RagdollRig>();

            if (rb != null)
            {
                if (ragdollRig != null)
                    //ragdollRig.DoConcussion();
                rb.AddExplosionForce(blastForce, transform.position, blastRadius, upwardsModifier);
            }
        }

        if (destroyAfterScale)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (exploadOnCollision && hasCollided == false)
        {
            hasCollided = true;
            StartCoroutine(Expload());
        }
    }

    //private Animator animator;
    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //}
    //private void OnCollisionEnter(Collision other)
    //{
    //    animator.SetTrigger("RubberDuckExplosion");
    //}
}

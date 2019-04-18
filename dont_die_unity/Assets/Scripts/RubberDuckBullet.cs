using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class RubberDuckBullet : MonoBehaviour
{
    public bool exploadOnCollision = true;
    public bool destroyAfterScale = true;
    private bool hasCollided = false;
    
    public float scaleMultiplier = 1f;
    public float timeToScale = 1f;
    
    [Tooltip("Time before scaling starts if exploadOnCollision is false.")]
    public float timeBeforeScale = 1f;

    private Vector3 startScale;
    private Vector3 endScale;

    private void Start()
    {
        startScale = transform.localScale;
        endScale = startScale * scaleMultiplier;
        
        if (exploadOnCollision == false)
            StartCoroutine(Expload(timeBeforeScale));
    }

    private IEnumerator Expload(float delay = 0f)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        
        float i = 0f;

        while (i < timeToScale)
        {
            i += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, i / timeToScale);
            yield return null;
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

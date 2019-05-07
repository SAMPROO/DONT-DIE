using System.Collections;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{
    [Header("Properties")] 
    public float delay = 3f;
    public bool scaleOnCollision = true;
    public bool destoyAfterSeconds = false;
    public float secondsToDestroy = 0f;

    [Header("Scale")] 
    public float scaleMultiplier = 1f;
    public float timeToScale = 1f;
    
    private Vector3 startScale;
    private Vector3 endScale;
    private bool hasCollided = false;    

    private void Start()
    {
        startScale = transform.localScale;
        endScale = startScale * scaleMultiplier;
        
        if (scaleOnCollision == false)
            StartCoroutine(Expload(delay));
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

        if (destoyAfterSeconds)
            Destroy(gameObject, secondsToDestroy);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (scaleOnCollision && hasCollided == false)
        {
            hasCollided = true;
            StartCoroutine(Expload());
        }
    }
}

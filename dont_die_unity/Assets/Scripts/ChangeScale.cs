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
    public float timeToScale = 1f;

    public float startScale = 0.1f;
    public float endScale = 2.0f;
    
    private bool hasCollided = false;    

    private void Start()
    {
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
            // transform.localScale = Vector3.Lerp(startScale, endScale, i / timeToScale);
            transform.localScale = Mathf.Lerp(startScale, endScale, i / timeToScale) * Vector3.one;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleEffect : MonoBehaviour
{

    public GameObject effect;
    public Vector3 positionOffset;
    
    public void DoEffect()
    {
        Destroy(Instantiate(effect, transform.position + positionOffset, transform.rotation), 2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(transform.position + positionOffset), .05f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttacher : MonoBehaviour
{
    private bool isAttached;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I hitted " + collision.gameObject);

        if(isAttached == false)
            DoAttach(collision.gameObject, collision.GetContact(0).point);
    }

    private void DoAttach(GameObject go,Vector3 hitPos)
    {
        FixedJoint instance = gameObject.AddComponent<FixedJoint>();
        if(go.GetComponent<Rigidbody>()==null)
        {
            instance.connectedBody = go.GetComponent<Rigidbody>();
        }
        else
        {
            instance.connectedAnchor = hitPos;
        }
        isAttached = true;
        
    }
}

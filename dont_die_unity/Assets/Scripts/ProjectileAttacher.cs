using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttacher : MonoBehaviour
{
    private bool isAttached;
    private bool ready;
    public float primeTime=0.3f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I hitted " + collision.gameObject);

        if(isAttached == false && ready && !collision.gameObject.CompareTag("weapon") && !collision.gameObject.CompareTag("projectile"))
            DoAttach(collision.gameObject, collision.GetContact(0).point);
    }

    private void DoAttach(GameObject go,Vector3 hitPos)
    {
        Debug.Log("I got attached to "+go);
        FixedJoint instance = gameObject.AddComponent<FixedJoint>();
        if(go.GetComponent<Rigidbody>()!=null)
        {
            instance.connectedBody = go.GetComponent<Rigidbody>();
        }
        else
        {
            instance.connectedAnchor = hitPos;
        }
        isAttached = true;
        
    }

    private void Update()
    {
        if (primeTime > 0)
        {
            primeTime -= 0.5f * Time.deltaTime;
        }
        else
        {
            ready = true;
        }
    }
}

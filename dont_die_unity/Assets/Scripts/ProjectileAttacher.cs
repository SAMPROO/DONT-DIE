using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttacher : MonoBehaviour
{
    private bool isAttached;
    private bool ready;
    public float primeTime=0.3f;
    public AudioClip sound;
    public GameObject go;

    private void OnCollisionEnter(Collision collision)
    {
        if(isAttached == false && ready && !collision.gameObject.CompareTag("weapon") && !collision.gameObject.CompareTag("projectile"))
            DoAttach(collision.gameObject, collision.GetContact(0).point);
    }

    private void DoAttach(GameObject _go,Vector3 hitPos)
    {
        go = _go;
        GetComponent<AudioSource>().PlayOneShot(sound);
        if(go.GetComponentInParent<StatusHelper>())
        {
            go.GetComponentInParent<StatusHelper>().rr.OnStatusHeal(5, 20, true);
        }

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

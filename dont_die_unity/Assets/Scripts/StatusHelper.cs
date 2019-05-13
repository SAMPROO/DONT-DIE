using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHelper : MonoBehaviour
{
    public StatusController rr;

    //BETA STUFF
    public CharacterJoint[] joints;
    public Rigidbody[] rbs;
    int i = 0;
    public float forceLimit = 20000;

    private void Start()
    {
        joints = GetComponentsInChildren<CharacterJoint>();
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(i > 60f)
        {
            foreach (CharacterJoint joint in joints)
            {

                if (joint.currentForce.sqrMagnitude/10000 > forceLimit)
                {
                    RagdollEase();
                    Debug.Log("Ragdoll Eased");
                }
                    
            }
            i=0;
        }
        else
        {
            i++;
        }
        
    }

    private void RagdollEase()
    {
        foreach(Rigidbody rb in rbs)
        {
            //rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }
        //GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //StartCoroutine(ResetForces());

        //Debug.Log("Waiting for reset");
        Debug.Log("RB velocity reset");
    }

    private IEnumerator ResetForces()
    {
        
        yield return new WaitForSeconds(0.1f);

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
        GetComponent<Rigidbody>().isKinematic = false;

        Debug.Log("DONE");
        yield return null;
    }
    //BETA STUFF
}

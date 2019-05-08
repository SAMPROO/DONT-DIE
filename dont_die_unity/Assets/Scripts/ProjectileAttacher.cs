using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttacher : MonoBehaviour
{
    public int effectTicks = 5;
    public float effectPower = 20;
    [SerializeField]
    private Effects.Type myType;
    private bool isAttached;
    private bool ready;
    public float primeTime=0.3f;
    public AudioClip sound;
    private GameObject go;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
    	// No need to check tags: Projectiles are on layer that doesn't hit weapons or projectiles anyway
        if(isAttached == false && ready)// && !collision.gameObject.CompareTag("weapon") && !collision.gameObject.CompareTag("projectile"))
            DoAttach(collision.gameObject, collision.GetContact(0).point);
    }

    private void DoAttach(GameObject _go,Vector3 hitPos)
    {
        go = _go;
        GetComponent<AudioSource>().PlayOneShot(sound);
        if(go.GetComponentInParent<StatusHelper>())
        {
            switch (myType)
            {
                case Effects.Type.Heal:
                    go.GetComponentInParent<StatusHelper>().rr.OnStatusHeal(effectTicks, effectPower, true);
                    break;

                case Effects.Type.Damage:
                    go.GetComponentInParent<StatusHelper>().rr.OnStatusDamage(effectTicks, effectPower, true);
                    break;

                case Effects.Type.Slow:
                    go.GetComponentInParent<StatusHelper>().rr.OnStatusSlow(effectTicks, effectPower, true);
                    break;

                case Effects.Type.Stun:
                    go.GetComponentInParent<StatusHelper>().rr.OnStatusStun(effectTicks, true);
                    break;
            }
            
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

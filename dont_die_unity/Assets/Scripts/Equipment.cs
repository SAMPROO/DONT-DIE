using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Equipment : MonoBehaviour
{
    public bool testFire;
    public Vector3 holdPosition;

    public float fiveSecondRule = 5;
    public int ammo = 1;

    protected bool isCarried;
    protected Rigidbody rb;

    private FixedJoint joint;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        // Destroy this gameobject if not carried and has no ammo after 5? seconds

        if (!isCarried && ammo <= 0)
        {
            Invoke("Destroy", fiveSecondRule);
        }
        else if (IsInvoking("Destroy"))
        {
            CancelInvoke("Destroy");
        }

#if UNITY_EDITOR
        if (testFire)
        {
            Use();
            testFire = false;
        }
#endif
    }


    public abstract void Use();

    public virtual void StartCarrying(Rigidbody connectedBody, float angle)
    {
        if (joint != null) return;

        transform.rotation = connectedBody.rotation * Quaternion.AngleAxis(angle, Vector3.right);
        
        transform.position = connectedBody.transform.TransformPoint(Quaternion.AngleAxis(angle + 180, Vector3.right) * holdPosition);

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = connectedBody;

        isCarried = true;
    }

    public virtual void StopCarrying()
    {
        Destroy(joint);
        joint = null;

        isCarried = false;

        Debug.Log("Gun thrown away");
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Draw a sphere at the hold position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(holdPosition), .05f);
    }
}
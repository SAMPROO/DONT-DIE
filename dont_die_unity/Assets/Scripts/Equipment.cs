using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Equipment : MonoBehaviour
{
    public bool testFire;
    public Vector3 holdPosition;

    protected bool isCarried;
    protected Rigidbody rb;

    private FixedJoint joint;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (testFire)
        {
            Use();
            testFire = false;
        }
    }
#endif

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

    protected virtual void OnDrawGizmosSelected()
    {
        // Draw a sphere at the hold position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(holdPosition), .05f);
    }
}
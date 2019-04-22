using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Equipment : MonoBehaviour
{
    public Vector3 holdPosition;

    protected bool isCarried;
    protected Rigidbody rb;

    private FixedJoint joint;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public abstract void Use();

    public virtual void StartCarrying(Transform carrier)
    {
        if (joint != null) return;

        // Turn off physics etc.
        //transform.SetParent(carrier);
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;

        //rb.isKinematic = true;
        //rb.detectCollisions = false;

        transform.position = carrier.position - Quaternion.LookRotation(carrier.forward, carrier.up) * holdPosition;
        transform.rotation = carrier.rotation;

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = carrier.GetComponent<Rigidbody>();

        isCarried = true;

        Debug.Log("Gun hops on");
    }


    // Use offsetRotation to set other rotation relative to connectedBody's rotation
    public virtual void StartCarrying(Rigidbody connectedBody, Quaternion offsetRotation)
    {
        if (joint != null) return;

        // Lets not use this, but instead model our guns at origin
        transform.position = 
            connectedBody.position 
            - Quaternion.LookRotation(connectedBody.transform.forward, connectedBody.transform.up) * holdPosition;
        
        transform.rotation = connectedBody.rotation * offsetRotation;

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = connectedBody.GetComponent<Rigidbody>();

        isCarried = true;

        Debug.Log("Gun hops on");
    }

    public virtual void StopCarrying()
    {
        // Turn on physics etc.
        //transform.SetParent(null);
        //rb.isKinematic = false;
        //rb.detectCollisions = true;

        Destroy(joint);
        joint = null;

        isCarried = false;

        Debug.Log("Gun thrown away");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Draw a sphere at the projectile spawn position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Quaternion.LookRotation(transform.forward, transform.up) * holdPosition, .05f);
    }
}
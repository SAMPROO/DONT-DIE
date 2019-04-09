using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Equipment : MonoBehaviour
{
    protected bool isCarried;
    protected Rigidbody rb;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public abstract void Use();

    public virtual void StartCarrying(Transform carrier)
    {
        // Turn off physics etc.
        transform.SetParent(carrier);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rb.isKinematic = true;

        isCarried = true;

        Debug.Log("Gun hops on");
    }

    public virtual void StopCarrying()
    {
        // Turn on physics etc.
        transform.SetParent(null);
        rb.isKinematic = false;

        isCarried = false;

        Debug.Log("Gun thrown away");
    }
}
using UnityEngine;
public class JumpPad : MonoBehaviour
{
    public float force = 0f;
    
    private void OnTriggerEnter(Collider other)
    {
        var ragdollRoot = other.transform.root.GetComponent<RagdollArmatureRoot>();
        var rigidbody = other.transform.GetComponent<Rigidbody>();

        // Check if collision occurs opposite to mattress transform.up
        float collisionDotProduct = Vector3.Dot(other.transform.up.normalized, transform.up.normalized);
        int dir = collisionDotProduct < 0 ? 1 : -1;

        ragdollRoot?.AddUniformForce(force * transform.up * dir, ForceMode.VelocityChange);

        if (ragdollRoot == null && rigidbody != null )
            rigidbody.AddForce(force * transform.up * dir, ForceMode.VelocityChange);

        // Also add counter force to this only if hit something hittable
        if (ragdollRoot != null || rigidbody != null)
            gameObject.GetComponent<Rigidbody>().AddForce(-force * transform.up * dir, ForceMode.VelocityChange);
    }
}

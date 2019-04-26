using UnityEngine;
public class JumpPad : MonoBehaviour
{
    public float force = 0f;
    
    private void OnTriggerEnter(Collider other)
    {
        var ragdollRoot = other.transform.root.GetComponent<RagdollArmatureRoot>();
        var rigidbody = other.transform.GetComponent<Rigidbody>();

        ragdollRoot?.AddUniformForce(force * transform.up, ForceMode.VelocityChange);
        
        if (ragdollRoot == null && rigidbody != null )
            rigidbody.AddForce(force * transform.up, ForceMode.VelocityChange);
        
        // Also add counter force to this only if hit something hittable
        if (ragdollRoot != null || rigidbody != null)
            gameObject.GetComponent<Rigidbody>().AddForce(-force * transform.up, ForceMode.VelocityChange);
    }
}

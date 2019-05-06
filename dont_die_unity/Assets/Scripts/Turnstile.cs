using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class Turnstile : MonoBehaviour
{
    public float force;

    private HingeJoint hingeJoint;

    private void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            var ragdollRoot = contact.otherCollider.transform.root.GetComponent<RagdollArmatureRoot>();
            var rigidbody = contact.otherCollider.transform.GetComponent<Rigidbody>();

            int dir = hingeJoint.velocity < 0 ? 1 : -1;

            ragdollRoot?.AddUniformForce(force * contact.thisCollider.transform.right * dir, ForceMode.VelocityChange);

            if (ragdollRoot == null && rigidbody != null)
                rigidbody.AddForce(force * contact.thisCollider.transform.right * dir, ForceMode.VelocityChange);
        }
    }
}

using UnityEngine;
public class JumpPad : MonoBehaviour
{
    public float force = 0f;

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody rb = other.transform.root.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }
}

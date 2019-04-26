using UnityEngine;
public class JumpPad : MonoBehaviour
{
    public float force = 0f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody = other.transform.root.GetComponent<Rigidbody>();

        if (rigidbody != null)
            rigidbody.velocity = Vector3.up * force;
    }
}

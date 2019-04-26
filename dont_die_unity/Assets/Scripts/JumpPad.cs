using UnityEngine;
public class JumpPad : MonoBehaviour
{
    public float force = 0f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody = other.transform.root.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.AddForce(force * transform.up, ForceMode.VelocityChange);
            gameObject.GetComponent<Rigidbody>().AddForce(-force * transform.up, ForceMode.VelocityChange);
        }
            
    }
}

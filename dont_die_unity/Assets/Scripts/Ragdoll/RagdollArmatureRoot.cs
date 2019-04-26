using UnityEngine;

public class RagdollArmatureRoot : MonoBehaviour
{
    private Rigidbody[] allRigidbodies;

    private void Awake()
    {
        allRigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    
    // Add same force to all rigidbodies in this ragdoll
    public void AddUniformForce(Vector3 force, ForceMode forceMode)
    {
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            allRigidbodies[i].AddForce(force, forceMode);
        }
    }
}

using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(HingeJoint))]
public class Turnstile : MonoBehaviour
{
    public float force;
    public float turnstileTargetVelocity;
    public float turnstileForce;
    public int timeUntilJointForceDisable = 3;


    private HingeJoint hingeJoint;
    private Rigidbody turnstileRigidbody;
    private float tolerance = 10f;

    private void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        turnstileRigidbody = GetComponent<Rigidbody>();
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
            
            // Make the hinge motor rotate with 90 degrees per second and a strong force.
            var motor = hingeJoint.motor;
            motor.force = turnstileForce;
            motor.targetVelocity = turnstileTargetVelocity;
            motor.freeSpin = false;
            hingeJoint.motor = motor;
            hingeJoint.useMotor = true;
        }
    }

    private void FixedUpdate()
    {

        var difference = Mathf.Abs(turnstileTargetVelocity - hingeJoint.velocity);
        if (difference < tolerance)
            StartCoroutine(DisableHingejointMotor());
    }

    private IEnumerator DisableHingejointMotor()
    {
        yield return new WaitForSeconds(timeUntilJointForceDisable);
        hingeJoint.useMotor = false;
    }
}

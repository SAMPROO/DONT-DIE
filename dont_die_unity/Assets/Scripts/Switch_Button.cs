using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Switch_Button : MonoBehaviour, ISwitch
{
    public bool status;
    public bool Status
    {
        get => status;
        private set { status = value; }
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    public bool isToggle;

    public Rigidbody movingPart;
    public float triggerForce;
    [Range(0f, 1f)] public float triggerDistance;

    private void Start()
    {
        Status = status;
    }

    private void OnValidate()
    {
        var joint = GetComponent<ConfigurableJoint>();

        // set connectedAnchor on the middle of movingPart and button (trigger)
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = -movingPart.transform.localPosition.y * .5f * transform.up;

        // set the linear limit
        joint.linearLimit = new SoftJointLimit { limit = transform.localScale.y * .5f };

        // modify trigger collider to given triggerDistance
        (GetComponent<Collider>() as BoxCollider).center = new Vector3(0, -triggerDistance, 0);

    }

    private void FixedUpdate()
    {
        // continuesly add upwards force to movingPart
        movingPart.AddForce(transform.up * triggerForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == movingPart.gameObject)
        {
            if (isToggle)
            {
                Status = !Status;

                if (Status) OnTurnOn();
                else OnTurnOff();
            }
            else
            {
                Status = true;
                OnTurnOn();
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isToggle && other.gameObject == movingPart.gameObject)
        {
            Status = false;
            OnTurnOff();
        }
    }
}

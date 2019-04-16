using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Switch_Button : MonoBehaviour, ISwitch
{
    private bool state;
    public bool State
    {
        get => state;
        set { state = value; }
    }

    public float Range
    {
        get => Convert.ToInt32(State);
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    public bool isToggle;

    public float triggerForce, triggerDamper;
    [Range(.1f, .9f)] public float triggerDistance, releaseDistance;
    public Rigidbody movingPart;

    private float startPos;
    private bool isTriggered;

    private ConfigurableJoint joint;

    private void OnValidate()
    {
        joint = GetComponent<ConfigurableJoint>();

        startPos = .5f + movingPart.transform.localPosition.y;

        // set connectedAnchor on the middle of movingPart and button (trigger)
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = -startPos * .5f * Vector3.up;

        // set the linear limit
        joint.linearLimit = new SoftJointLimit { limit = transform.localScale.y * .5f - .001f };
        joint.yDrive = new JointDrive { positionSpring = 1000, maximumForce = triggerForce, positionDamper = triggerDamper };

        OnTurnOff?.Invoke();
    }

    private void FixedUpdate()
    {
        float distance = .5f + movingPart.transform.localPosition.y;

        if(distance <= startPos - triggerDistance)
        {
            //Debug.Log("Triggered");

            if (isToggle)
            {
                if (!isTriggered)
                {
                    State = !State;

                    if (State) OnTurnOn?.Invoke();
                    else OnTurnOff?.Invoke();

                    isTriggered = true;
                }
            }
            else
            {
                State = true;

                OnTurnOn?.Invoke();

                isTriggered = true;
            }
        }
        else if (isTriggered && distance >= startPos - releaseDistance)
        {
            //Debug.Log("Released");

            if (isToggle)
            {
                isTriggered = false;
            }
            else
            {
                State = false;
                OnTurnOff?.Invoke();
            }
        }
    }
}

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Switch_Button : MonoBehaviour, ISwitch
{
    public bool state;
    public bool State
    {
        get => state;
        private set { state = value; }
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    public bool isToggle;

    public float triggerForce, triggerDamper;
    [Range(.1f, .9f)] public float triggerDistance, releaseDistance;
    public Rigidbody movingPart;

    private float startPos;
    private bool isTriggered;

    private void OnValidate()
    {
        var joint = GetComponent<ConfigurableJoint>();

        startPos = .5f + movingPart.transform.localPosition.y;

        // set connectedAnchor on the middle of movingPart and button (trigger)
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = -startPos * .5f * Vector3.up;

        // set the linear limit
        joint.linearLimit = new SoftJointLimit { limit = transform.localScale.y * .5f - .001f };
        joint.yDrive = new JointDrive { positionSpring = 1000, maximumForce = triggerForce, positionDamper = triggerDamper };
    }

    private void FixedUpdate()
    {
        float distance = .5f + movingPart.transform.localPosition.y;

        if(distance <= startPos - triggerDistance)
        {
            Debug.Log("Triggered");

            if (isToggle)
            {
                if (!isTriggered)
                {
                    State = !State;

                    if (State) OnTurnOn();
                    else OnTurnOff();

                    isTriggered = true;
                }
            }
            else
            {
                State = true;
                OnTurnOn();
            }
        }
        else if (distance >= startPos - releaseDistance)
        {
            Debug.Log("Released");

            if (isToggle)
            {
                isTriggered = false;
            }
            else
            {
                State = false;
                OnTurnOff();
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject == movingPart.gameObject)
    //    {
    //        if (isToggle)
    //        {
    //            State = !State;

    //            if (State) OnTurnOn();
    //            else OnTurnOff();
    //        }
    //        else
    //        {
    //            State = true;
    //            OnTurnOn();
    //        }            
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
//        if (!isToggle && other.gameObject == movingPart.gameObject)
//        {
//            State = false;
//            OnTurnOff();
//}
    //}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Lever : MonoBehaviour, ISwitch
{
    private bool state;
    public bool State
    {
        get => state;
        private set { state = value; }
    }

    private float range;
    public float Range
    {
        get => range;
        private set { range = value; }
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    [Range(0, 180)] public float turnAngle = 120;
    [Range(-1, 1)] public float startPoint;

    public float drag = 10;
    public float driveForce;

    public Rigidbody movingPart;
    private HingeJoint joint;

    private float currentAngle, maxAngle;

    private void OnValidate()
    {
        joint = GetComponent<HingeJoint>();

        // set angle limits
        maxAngle = turnAngle / 2;
        joint.limits = new JointLimits
        {
            min = -maxAngle + startPoint * maxAngle,
            max = maxAngle + startPoint * maxAngle
        };

        // rotate to start angle
        movingPart.transform.localRotation = Quaternion.AngleAxis(startPoint * .5f * turnAngle, transform.up);

        // set damper value
        joint.spring = new JointSpring { spring = driveForce,  damper = drag, targetPosition = startPoint * turnAngle };
    }

    private void Update()
    {
        currentAngle += Mathf.DeltaAngle(currentAngle, movingPart.transform.localRotation.eulerAngles.y);

        float angle = currentAngle - joint.limits.min;

        Range = angle / turnAngle;

        Debug.Log("Log: " + range);

        if (!State && angle >= turnAngle)
        {
            State = true;
            OnTurnOn?.Invoke();
        }
        else if (State && angle <= 0)
        {
            State = false;
            OnTurnOff?.Invoke();
        }
    }
}

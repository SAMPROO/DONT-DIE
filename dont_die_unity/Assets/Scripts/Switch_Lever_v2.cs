using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Lever_v2 : MonoBehaviour, ISwitch
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

    public float fromAngle = 0, toAngle = 90, triggerAngle, endSpring = 10;

    public Rigidbody movingPart;

    private float minAngle, maxAngle, angularRange, currentAngle;

    private void OnValidate()
    {
        minAngle = Mathf.Min(fromAngle, toAngle);
        maxAngle = Mathf.Max(fromAngle, toAngle);
        angularRange = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);

        triggerAngle = Mathf.Clamp(triggerAngle, 0, angularRange / 2);
    }

    private void FixedUpdate()
    {
        // keep track of current angle
        currentAngle += Mathf.DeltaAngle(currentAngle, -movingPart.transform.localRotation.eulerAngles.y);

        // set range output
        Range = (currentAngle + (minAngle < 0 ? -minAngle : 0)) / angularRange;
        Range = Mathf.Clamp(Range, 0, 1);

        if (currentAngle >= maxAngle - triggerAngle)
        {
            // limit rotation to maximum angle 
            if (currentAngle > maxAngle)
                movingPart.transform.localRotation = Quaternion.Lerp(movingPart.transform.localRotation, 
                    Quaternion.AngleAxis(maxAngle, -transform.up), Time.deltaTime * endSpring);

            if (!State)
            {
                State = true;
                OnTurnOn?.Invoke();
            }            
        }
        else if (currentAngle <= minAngle + triggerAngle)
        {
            // limit rotation to minimum angle 
            if (currentAngle < minAngle)
                movingPart.transform.localRotation = Quaternion.Lerp(movingPart.transform.localRotation, 
                    Quaternion.AngleAxis(minAngle, -transform.up), Time.deltaTime * endSpring);


            if (State)
            {
                State = false;
                OnTurnOff?.Invoke();
            }
        }
    }
}

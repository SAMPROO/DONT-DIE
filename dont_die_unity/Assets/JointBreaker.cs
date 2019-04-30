using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointBreaker : MonoBehaviour
{
    public Joint joint;

    public GameObject switchGameObject;
    private ISwitch iSwitch;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        iSwitch.OnTurnOn += BreakJoint;
    }

    private void BreakJoint()
    {
        if (joint)
        {
            Destroy(joint);
            joint = null;
        }

        Destroy(this);
    }
}

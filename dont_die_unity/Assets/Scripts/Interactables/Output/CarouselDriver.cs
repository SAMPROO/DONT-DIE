using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselDriver : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    public HingeJoint joint;

    public bool state;

    private void OnValidate()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        joint.useMotor = state;
    }

    private void Start()
    {
        iSwitch.OnTurnOn += Toggle;
        iSwitch.OnTurnOff += Toggle;
    }

    private void Toggle()
    {
        state = !state;
        joint.useMotor = state;
    }
}
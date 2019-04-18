using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselDriver : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    public Rigidbody movingPart;
    public float maxSpeed = 500, duration = 1;
    private float startSpeed, targetSpeed, startTime, currentAngle;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        iSwitch.OnTurnOn += Toggle;
        iSwitch.OnTurnOff += Toggle;

        startSpeed = maxSpeed;
        targetSpeed = 0;
    }

    private void Update()
    {
        float t = (Time.time - startTime) / duration;

        currentAngle += Mathf.SmoothStep(startSpeed, targetSpeed, t);

        movingPart.MoveRotation(Quaternion.AngleAxis(currentAngle, transform.up));
    }

    private void Toggle()
    {
        startTime = Time.time;

        float temp = targetSpeed;
        targetSpeed = startSpeed;
        startSpeed = temp;
    }
}

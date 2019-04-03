using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraTP : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -80;
    private const float Y_ANGLE_MAX = 80;

    [HideInInspector]
    public Transform anchor;

    public Vector2 sensitivity = new Vector2(1,1);
    public float cameraDistanceMax;
    public float cameraDistanceMin;

    public float cameraDistanceCurrent;

    private float inputX = 0;
    private float inputY = 0;
    private float angle;

    private bool aim;

    private InputController input;

    //for player
    public Quaternion baseRotation => Quaternion.Euler(0, inputX, 0);

    private void Start()
    {
        if (sensitivity.x == 0) sensitivity.x = 2f;
        if (sensitivity.y == 0) sensitivity.y = 0.5f;
    }


    private void Update()
    {
        inputX += input.LookHorizontal * sensitivity.x *90* Time.deltaTime;
        inputY += input.LookVertical * sensitivity.y *90* Time.deltaTime;
        //Debug.Log(input.LookHorizontal);
        inputY = Mathf.Clamp(inputY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        if (input.Focus)
        {
            aim = true;
        }
        else
        {
            aim = false;
        }


    }

    private void LateUpdate()
    {
        MoveCamera(inputY, inputX);

        if (aim == false)
        {
            angle = ((inputY + 10) * 3f) / 300;
            angle -= 0.2f;
            angle = Mathf.Clamp(angle, 0, 1);

            cameraDistanceCurrent = Mathf.Lerp(cameraDistanceCurrent, Mathf.Lerp(cameraDistanceMin, cameraDistanceMax, angle), 0.5f);
        }
        else
        {
            cameraDistanceCurrent = Mathf.Lerp(cameraDistanceCurrent, cameraDistanceMin / 2, 0.5f);
        }

    }

    public void MoveCamera(float _yRot, float _xRot)
    {
        Vector3 dir = new Vector3(0, 0, -cameraDistanceCurrent);
        Quaternion rotation = Quaternion.Euler(_yRot, _xRot, 0);
        transform.position = anchor.position + rotation * dir;
        transform.LookAt(anchor.position);
    }

    public void SetInputController(InputController _input)
    {
        input = _input;
        Debug.Log(sensitivity);
    }
}

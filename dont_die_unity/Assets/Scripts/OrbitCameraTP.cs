using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraTP : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -80;
    private const float Y_ANGLE_MAX = 80;

    [HideInInspector]
    public Transform anchor;

    public float sensitivity;
    public float cameraDistanceMax;
    public float cameraDistanceMin;

    public float cameraDistanceCurrent;

    private float inputX = 0;
    private float inputY = 0;
    private float angle;

    private bool aim;

    //for player
    public Quaternion baseRotation => Quaternion.Euler(0, inputX, 0);

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inputX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        inputY += Input.GetAxis("Mouse Y") * -sensitivity * Time.deltaTime;

        inputY = Mathf.Clamp(inputY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        if (Input.GetMouseButton(1))
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
}

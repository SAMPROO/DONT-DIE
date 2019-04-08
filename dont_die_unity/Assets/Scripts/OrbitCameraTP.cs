using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraTP : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -80;
    private const float Y_ANGLE_MAX = 80;

    public Transform anchor;
    public Vector2 normalLookOffset;
    public Vector2 focusLookOffset;

    public Vector2 sensitivity = new Vector2(1,1);
    public float cameraDistanceMax;
    public float cameraDistanceMin;

    public float cameraDistanceCurrent;

    private float inputX = 0;
    private float inputY = 0;
    private float angle;

    private bool aim;

    private IInputController input;

    //for player
    public Quaternion baseRotation => Quaternion.Euler(0, inputX, 0);

    // Focus lerp things
    private int smoothIndex = 0;
    private const int smoothArraySize = 20;
    private float [] smoothArray = new float[smoothArraySize];
    private float focusLerp;

    private void Start()
    {
        //Hack sensitivityn Vector2 prefabissa instanssioituna on jostain syystä 0 ja 0
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
        smoothArray [smoothIndex] = input.Focus == true ? 1f : 0f; // == 0 / 1
        smoothIndex = (smoothIndex + 1) % smoothArray.Length;
        focusLerp = smoothArray.Average();

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
        Vector3 zPosition = new Vector3(0, 0, -cameraDistanceCurrent);

        Vector2 offset = Vector2.Lerp(normalLookOffset, focusLookOffset, focusLerp);
        Vector3 xyPosition = new Vector3(offset.x, offset.y, 0);

        // TODO: make this happen to make camera not go underground      
        // Vector3 anchor2 = anchor.position + rotation * xyposition;
        // Ray ray = new Ray (anchor2, -1 * rotation * Vector3.forward);
        // Vector3 finalPosition = sphereCast (ray, cameraDistanceCurrent);

        Quaternion rotation = Quaternion.Euler(_yRot, _xRot, 0);
        transform.position = anchor.position + rotation * (xyPosition + zPosition);

        Vector3 lookTarget = anchor.position + rotation * xyPosition;
        transform.LookAt(lookTarget);
    }

    public void SetInputController(IInputController _input)
    {
        Debug.Log("Setted inputs");
        input = _input;
    }
}

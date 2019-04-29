using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCameraTP : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -80;
    private const float Y_ANGLE_MAX = 80;

    public Transform anchor;
    public Vector2 normalLookOffset;
    public Vector2 focusLookOffset;

    public Vector3 offset;

    public Vector2 sensitivity = new Vector2(1,1);
    public float cameraDistanceMax;
    public float cameraDistanceMin;

    public float cameraDistanceCurrent;

    private float inputX = 0;
    private float inputY = 0;

    private bool aim;

    private IInputController input;
    public void SetInputController(IInputController _input)
        => input = _input;

    // For PlayerCharacter
    public Quaternion BaseRotation => Quaternion.Euler(0, inputX, 0);
    public float AimAngle { get; private set; }

    // Focus lerp things
    private int smoothIndex = 0;
    private const int smoothArraySize = 20;
    private float [] smoothArray = new float[smoothArraySize];
    private float focusLerp;

    public Camera GetCamera() => GetComponent<Camera>();

    private Vector3 DEBUGCameraPosition;
    private Vector3 DEBUGRayOrigin;
    private Vector3 DEBUGRayDirection;


    private Vector3 [] DEBUGRayPoints = new Vector3[3];

    public LayerMask cameraRayMask;
    public float cameraColliderRadius = 0.2f;

    private void LateUpdate()
    {
        // Input is fine in LateUpdate
        inputX += input.LookHorizontal * sensitivity.x *90* Time.deltaTime;
        inputY += input.LookVertical * sensitivity.y *90* Time.deltaTime;
        inputY = Mathf.Clamp(inputY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        aim = input.Focus;

        smoothArray [smoothIndex] = input.Focus == true ? 1f : 0f; // == 0 / 1
        smoothIndex = (smoothIndex + 1) % smoothArray.Length;
        focusLerp = smoothArray.Average();

        /// Aim stuff -------------------------------------------------------
        if (aim == false)
        {
            float angle;
            angle = ((inputY + 10) * 3f) / 300;
            angle -= 0.2f;
            angle = Mathf.Clamp(angle, 0, 1);

            cameraDistanceCurrent = Mathf.Lerp(cameraDistanceCurrent, Mathf.Lerp(cameraDistanceMin, cameraDistanceMax, angle), 0.5f);
        }
        else
        {
            cameraDistanceCurrent = Mathf.Lerp(cameraDistanceCurrent, cameraDistanceMin / 2, 0.5f);
        }

        var flatForwardVector = new Vector3(transform.forward.x, 0, transform.forward.z);
        AimAngle = Vector3.SignedAngle(transform.forward, flatForwardVector, transform.right);


        // Move stuff ------------------------------------------------------
        Vector2 offset = Vector2.Lerp(normalLookOffset, focusLookOffset, focusLerp);
        Vector3 xyPosition = new Vector3(offset.x, offset.y, 0);


        Vector3 zPosition = new Vector3(0, 0, -cameraDistanceCurrent);

        Quaternion rotation = Quaternion.Euler(inputY, inputX, 0);

        // Go up. No avoid clipping on this direction, so no cast
        // TODO: rotating updirection may be the cause of "nod"
        // var upDir = rotation * Vector3.up;
        var upDir = Vector3.up;
        var point0 = anchor.position + upDir * offset.y;
        
        // Go right/left
        var rightDir = rotation * Vector3.right;
        var point1 = point0 + rightDir * offset.x;
        RaycastHit hit1;
        if (Physics.SphereCast(point0, cameraColliderRadius, rightDir, out hit1, offset.x, cameraRayMask))
        {
            point1 = point0 + rightDir * hit1.distance;
        }

        // Go back
        var backDir = rotation * Vector3.back;
        var point2 = point1 + backDir * cameraDistanceCurrent;
        RaycastHit hit2;
        if (Physics.SphereCast(point1, cameraColliderRadius, backDir, out hit2, cameraDistanceCurrent, cameraRayMask))
        {
            point2 = point1 + backDir * hit2.distance;
        }


        DEBUGRayPoints[0] = point0;
        DEBUGRayPoints[1] = point1;
        DEBUGRayPoints[2] = point2;

        transform.position = point2;

        Vector3 lookTarget = point1;
        transform.LookAt(lookTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(DEBUGRayPoints[0], 0.15f);
        Gizmos.DrawLine(DEBUGRayPoints[0], DEBUGRayPoints[1]);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(DEBUGRayPoints[1], 0.15f);
        Gizmos.DrawLine(DEBUGRayPoints[1], DEBUGRayPoints[2]);

        Gizmos.color = new Color(1, 0, 1);
        Gizmos.DrawWireSphere(DEBUGRayPoints[2], 0.15f);    


    }
}

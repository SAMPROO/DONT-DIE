﻿using System.Collections;
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
    public float cameraDistanceMax;
    public float cameraDistanceMin;

    public Vector2 sensitivity = new Vector2(1,1);

    [Header("Avoid clipping")]
    public LayerMask clippingRayMask;   // not clipping really. hiding or something....
    public float cameraColliderRadius = 0.2f;

    private float xAngle = 0;
    private float yAngle = 0;
    private SmoothFloat smoothFocus = new SmoothFloat (10);

    private IInputController input;
    public void SetInputController(IInputController input)
        => this.input = input;

    // For PlayerCharacter
    public Quaternion BaseRotation => Quaternion.Euler(0, xAngle, 0);
    public float AimAngle { get; private set; }

    public Camera GetCamera() => GetComponent<Camera>();

    // Set camera rig to look given.
    // Currently only sets horizontal rotation
    public void SetLookDirection(Vector3 direction)
    {
        xAngle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
    
        // This would be implemented something like this, but due to lack of time it is left out
        // Vector3 yAxis = Vector3.Cross(Vector3.up, direction);
        // yAngle = Vector3.SignedAngle(Vector3.forward, direction, yAxis);
        yAngle = 0;
    }

    private void LateUpdate()
    {
        // Input stuff --------------------------------------------------------
        xAngle += input.LookHorizontal * sensitivity.x * 90 * Time.deltaTime;
        yAngle += input.LookVertical * sensitivity.y * 90 * Time.deltaTime;
        yAngle = Mathf.Clamp(yAngle, Y_ANGLE_MIN, Y_ANGLE_MAX);

        bool aim = input.Focus;
        smoothFocus.Put(aim ? 1f : 0f);

        /// Aim stuff ---------------------------------------------------------
        // Tested values, do not change if you are not Iiro Pelttari ¤#"¤!(¤"!
        float angle = Mathf.Clamp01((yAngle + 10) / 100f - 0.2f);

        float noAimTargetDistance = Mathf.Lerp(cameraDistanceMin, cameraDistanceMax, angle);
        float yesAimTargetDistance = cameraDistanceMin / 2f;

        // Compute aim value for player controller hands.
        var flatForwardVector = new Vector3(transform.forward.x, 0, transform.forward.z);
        AimAngle = Vector3.SignedAngle(transform.forward, flatForwardVector, transform.right);

        // Move stuff ---------------------------------------------------------
        Quaternion rotation = Quaternion.Euler(yAngle, xAngle, 0);

        // Take smoothed focus input into account
        float focusLerp = smoothFocus.Value;
        float xLocalPos = Mathf.Lerp(normalLookOffset.x, focusLookOffset.x, focusLerp);
        float yLocalPos = Mathf.Lerp(normalLookOffset.y, focusLookOffset.y, focusLerp);
        float zLocalPos = Mathf.Lerp(noAimTargetDistance, yesAimTargetDistance, focusLerp);

        /*
        Use Physics.SphereCast to avoid going inside/behind walls. We do not currently care
        about upward clipping, but it could be done easily.

        First, move position up. Then cast right to get x offset / local position.
        Then from that position cast back to get z local position.
        */

        // Go up. No avoid clipping on this direction, so no cast
        var upDir = Vector3.up;
        var upPoint = anchor.position + upDir * yLocalPos;

        // Use these for both casts 
        RaycastHit hitInfo;
        bool hit;

        // Go right/left
        var rightDir = rotation * Vector3.right;
        hit = Physics.SphereCast(upPoint, cameraColliderRadius, rightDir, out hitInfo, xLocalPos, clippingRayMask);
        var rightPoint = upPoint + rightDir * (hit ? hitInfo.distance : xLocalPos);

        // Go back
        var backDir = rotation * Vector3.back;
        hit = Physics.SphereCast(rightPoint, cameraColliderRadius, backDir, out hitInfo, zLocalPos, clippingRayMask);
        var backPoint = rightPoint + backDir * (hit ? hitInfo.distance : zLocalPos);

        #if UNITY_EDITOR
        DEBUGRayPoints[0] = upPoint;
        DEBUGRayPoints[1] = rightPoint;
        DEBUGRayPoints[2] = backPoint;
        #endif

        // Translate and rotate
        // TODO: smooth movement here
        transform.position = backPoint;
        transform.rotation = rotation;
    }

    #if UNITY_EDITOR

    private Vector3 [] DEBUGRayPoints = new Vector3[3];
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

    #endif
}

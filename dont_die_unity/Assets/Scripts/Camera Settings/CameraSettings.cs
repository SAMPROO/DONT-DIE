﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Camera Settings", menuName = "Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Vertical angle limits -80 | +80")]
    [Range(5f, 80f)]
    public float verticalAngleMax = 50f;
    [Range(-5f, -80f)]
    public float verticalAngleMin = -50f;
    [Header("Camera distance limits 1 | 50")]
    [Range(5f, 50f)]
    public float distanceMax = 30f;
    [Range(1f, 5f)]
    public float distanceMin = 4f;

    [Header("Focus smooth value 1 | 40")]
    [Range(1f,40f)]
    public int focusSmooth = 20;
    [Header("Camera anchor position smooth 0.1 | 1.0")]
    [Range(0.1f, 1f)]
    public float anchorSmooth = 0.5f;

    public void ApplySettings(OrbitCameraTP camera)
    {
        camera.Y_ANGLE_MIN = verticalAngleMin;
        camera.Y_ANGLE_MAX = verticalAngleMax;
        camera.smoothArray = new float[focusSmooth];
        camera.cameraDistanceMin = distanceMin;
        camera.cameraDistanceMax = distanceMax;
        camera.anchorSmooth = anchorSmooth;
    }
}

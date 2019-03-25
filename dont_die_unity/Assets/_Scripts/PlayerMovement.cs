using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Transform CameraRotator;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xAxis, 0, zAxis).normalized;
        Quaternion moveRotation = Quaternion.FromToRotation(Vector3.forward, CameraRotator.forward);
        moveDirection = moveRotation * moveDirection;

        rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);


        // Rotate Camera Rotator towards mouse
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Vector3 point = new Vector3(hit.point.x, CameraRotator.position.y, hit.point.z);
            Vector3 dir = hit.point - CameraRotator.position;
            CameraRotator.LookAt(CameraRotator.position + dir);
        }
    }
}

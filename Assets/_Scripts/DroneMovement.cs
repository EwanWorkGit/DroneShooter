using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DroneMovement : MonoBehaviour
{
    [NonSerialized] public float Altitude = 0;

    [SerializeField] float RotationSpeed = 1, MoveSpeed = 7, AltitudeSpeed = 5;
    float Angle = 0;
    float StartAngle = -90;

    private void Start()
    {
        Angle = StartAngle;
        transform.rotation = Quaternion.Euler(0,0,Angle);
    }

    private void Update()
    {
        //rotation
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + StartAngle;
        float curAngle = transform.eulerAngles.z;
        Angle = Mathf.MoveTowardsAngle(curAngle, targetAngle, RotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, Angle);

        //xy movement
        Vector3 inputs = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.position += inputs * Time.deltaTime * MoveSpeed;

        //altitude (z movement)
        float altitudeInput = Input.GetAxisRaw("Altitude");
        Altitude += altitudeInput * AltitudeSpeed * Time.deltaTime;
    }
}

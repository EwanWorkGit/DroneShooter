using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] Transform ToFollow, MouseTransform;
    [SerializeField] float DistanceWeight = 0.2f, SmoothTime = 0.2f;

    Vector3 Velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector2 dir = MouseTransform.position - ToFollow.position;
        float dist = dir.magnitude;
        dir.Normalize();
        Vector3 targetPos = (Vector2)ToFollow.position + (dir * dist * DistanceWeight);
        targetPos.z = -10;

        //smoothdamp estimates a spring moving towards the target and helps with smoothness and aiming ease
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref Velocity, SmoothTime);
    }
}

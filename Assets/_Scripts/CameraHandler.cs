using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;

    [SerializeField] Transform ToFollow, MouseTransform;
    [SerializeField] float DistanceWeight = 0.2f, SmoothTime = 0.2f, RecoilSmoothTime = 0.1f;

    Vector3 Velocity = Vector3.zero, RecoilVelocity = Vector3.zero, RecoilOffset = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        Vector2 dir = MouseTransform.position - ToFollow.position;
        float dist = dir.magnitude;
        dir.Normalize();
        Vector3 targetPos = (Vector2)ToFollow.position + (dir * dist * DistanceWeight);
        targetPos.z = -10;

        //smoothdamp estimates a spring moving towards the target and helps with smoothness and aiming ease
        Vector3 followPosition = Vector3.SmoothDamp(transform.position, targetPos, ref Velocity, SmoothTime);
        RecoilOffset = Vector3.SmoothDamp(RecoilOffset, Vector3.zero, ref RecoilVelocity, RecoilSmoothTime);

        transform.position = followPosition + RecoilOffset;
    }

    public void InjectVelocity(Vector3 dir, float magnitude)
    {
        RecoilVelocity += dir * magnitude;
    }
}

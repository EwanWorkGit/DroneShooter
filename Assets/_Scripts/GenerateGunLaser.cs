using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGunLaser : MonoBehaviour
{
    //get crosshair position and line renderer
    //if collider.transform == null: end position = crosshair position; else: end position = hit.point

    [SerializeField] Transform Crosshair;
    [SerializeField] LineRenderer LineRenderer;
    [SerializeField] LayerMask RayMask;

    private void Awake()
    {
        if(LineRenderer != null)
        {
            LineRenderer.positionCount = 2;
        }
        else
        {
            Debug.Log("Line renderer has not been assigned in laser script.");
        }
    }

    private void Update()
    {
        Vector3 dir = Crosshair.position - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, RayMask);
        Vector3 endPosition = Vector3.zero;
        if(hit.transform != null)
        {
            endPosition = hit.point;
        }
        else
        {
            endPosition = Crosshair.position;
        }

        if(LineRenderer != null)
        {
            LineRenderer.SetPosition(0, transform.position);
            LineRenderer.SetPosition(1, endPosition);
        }
        else
        {
            Debug.Log("Cannot set positions in linerenderer. Linerenderer null.");
        }
    }
}

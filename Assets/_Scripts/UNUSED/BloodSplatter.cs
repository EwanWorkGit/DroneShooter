using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BloodSplatter : MonoBehaviour
{
    [NonSerialized] public Vector2 Direction;
    [SerializeField] float VelocityMagnitude = 5, Friction = 0.2f;

    Vector2 Velocity = new();

    private void Start()
    {
        Velocity = UnityEngine.Random.Range(0.7f, 1.1f) * VelocityMagnitude * Direction;
    }

    private void Update()
    {
        Velocity = Vector2.MoveTowards(Velocity, Vector2.zero, Velocity.magnitude * Friction * Time.deltaTime);
        transform.position += (Vector3)Velocity * Time.deltaTime;
    }
}

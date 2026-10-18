using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float Speed = 15f;

    private void Update()
    {
        transform.position += -transform.right * Speed * Time.deltaTime;
    }
}

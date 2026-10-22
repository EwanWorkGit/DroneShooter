using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthHandler : MonoBehaviour
{
    public static event Action<Transform> WasDamaged;

    [SerializeField] float Health = 10f;

    public void TakeDamageGeneral(float damage)
    {
        Health -= damage;
        //Debug.Log(Health);
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        WasDamaged?.Invoke(transform);
    }

    public void TakeDamagePlayer(float damage)
    {
        Health -= damage;
        //Debug.Log(Health);
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

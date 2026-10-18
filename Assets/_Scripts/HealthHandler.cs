using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] float Health = 10f;

    public void TakeDamage(float damage)
    {
        Health -= damage;
        //Debug.Log(Health);
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

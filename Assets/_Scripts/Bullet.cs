using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float Speed = 15f, Damage = 1;

    private void Update()
    {
        transform.position += -transform.right * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if: collision isnt player; destroy object; else: damage player, then destroy object
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.TryGetComponent(out HealthHandler healthHandler))
            {
                healthHandler.TakeDamage(Damage);
            }
            else
            {
                Debug.Log("Player does not have a health handler");
            }
        }
        Destroy(gameObject);
    }
}

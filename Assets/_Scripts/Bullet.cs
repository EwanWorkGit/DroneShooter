using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //add action event HasImpacted
    //if action is triggered every subrscriber should check distance to the bullet and if theyre inside alert radius the enemy should detect the player

    [SerializeField] float Speed = 15f, Damage = 1;

    Vector2 InitialPosition;

    private void Awake()
    {
        InitialPosition = transform.position;
    }

    private void Update()
    {
        transform.position += -transform.right * Speed * Time.deltaTime;
        if(Vector2.Distance(InitialPosition, transform.position) >= 100f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if: collision isnt player; destroy object; else: damage player, then destroy object
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.TryGetComponent(out HealthHandler healthHandler))
            {
                healthHandler.TakeDamagePlayer(Damage);
            }
            else
            {
                Debug.Log("Player does not have a health handler");
            }
        }

        if(LayerMask.LayerToName(collision.gameObject.layer) != "Bullets")
        {
            Destroy(gameObject);
        }
    }
}

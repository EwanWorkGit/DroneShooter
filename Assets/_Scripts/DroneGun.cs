using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGun : MonoBehaviour
{
    [SerializeField] Transform Drone, Crosshair, MouseHair;

    //rotation and opacity
    [SerializeField] float ConeAngle = 30f, MinFractionValue = 0.2f, MaxCrosshairDist = 1;
    float StartAngle = -90;

    //gun
    [SerializeField] GameObject ImpactPrefab;
    [SerializeField] float Damage = 4f, ShotCooldown = 0.05f, ImpactRadius = 0.1f, MaxSpread = 25 /*degrees*/, SpreadIncreaseSpeed = 2f, SpreadDecreaseSpeed = 8f; 
    float Timer = 0, Spread = 0;

    //animation
    [SerializeField] Animator Animator;

    //blood
    [SerializeField] GameObject BloodPrefab1;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        //rotating the gun
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - (Vector2)transform.position;
        //getting radius before information is lost
        float radius = dir.magnitude;
        dir.Normalize();
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + StartAngle; 
        float droneRot = Drone.eulerAngles.z;
        //returns fastest way to target angle 
        float difference = Mathf.DeltaAngle(droneRot, targetAngle);
        difference = Mathf.Clamp(difference, -ConeAngle/2f, ConeAngle/2f);
        targetAngle = droneRot + difference;
        transform.rotation = Quaternion.Euler(0,0,targetAngle);
        //polar coordinates
        Vector2 crosshairPos = (Vector2)Drone.position + new Vector2(Mathf.Cos(Mathf.Deg2Rad*(targetAngle-StartAngle)), Mathf.Sin(Mathf.Deg2Rad*(targetAngle-StartAngle))) * radius;
        Crosshair.position = crosshairPos;
        MouseHair.position = mousePos;
        //distance-based opacity for mousehair
        float distToCrosshair = Vector2.Distance(crosshairPos, mousePos);
        float frac = distToCrosshair / MaxCrosshairDist;
        frac = Mathf.Clamp(frac, MinFractionValue, 1);
        SpriteRenderer rend = MouseHair.GetComponent<SpriteRenderer>();
        Color color = rend.color;
        color.a = frac;
        rend.color = color;

        //shooting
        if(Input.GetMouseButton(0))
        {
            Timer -= Time.deltaTime;
            Spread += SpreadIncreaseSpeed * Time.deltaTime;
            Spread = Mathf.Min(Spread, MaxSpread);
            float angleWithSpread = targetAngle + Random.Range(-Spread/2f, Spread/2f); //it can impact within target angle -+ spead
            Vector2 impactPos = (Vector2)Drone.position + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angleWithSpread - StartAngle)), Mathf.Sin(Mathf.Deg2Rad * (angleWithSpread - StartAngle))) * radius;

            if (Timer <= 0)
            {
                Timer = ShotCooldown;
                if(Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    Animator.SetTrigger("HasFired");
                }
                //shoot
                Collider2D[] collisions = Physics2D.OverlapCircleAll(impactPos, ImpactRadius);
                foreach(Collider2D collider in collisions)
                {
                    //Debug.Log(collider.gameObject.name);
                    if(collider.transform.TryGetComponent(out HealthHandler damageHandler))
                    {
                        damageHandler.TakeDamage(Damage);
                        //blood
                        Vector2 dirToEnemy = (collider.transform.position - transform.position).normalized;
                        float angle = Mathf.Atan2(dirToEnemy.y, dirToEnemy.x)*Mathf.Rad2Deg;
                        GameObject blood = Instantiate(BloodPrefab1, (Vector2)collider.transform.position + dirToEnemy, Quaternion.Euler(0,0,angle-90));
                        blood.GetComponent<BloodSplatter>().Direction = dirToEnemy;
                    }
                }
                Instantiate(ImpactPrefab, impactPos, Quaternion.Euler(0,0,targetAngle - 180f));

                

            }
            
            //hit ground (mousepos) and overlapcircle to get enemies hit

            //might need to lag compensate bullet count
            //perhaps elapsedtime // firecooldown for bullet count and fire them of with small timer
        }
        else
        {
            Animator.Play("Idle", 0, 0f);
            Spread -= SpreadDecreaseSpeed * Time.deltaTime;
            Spread = Mathf.Max(0, Spread);
        }
    }
}

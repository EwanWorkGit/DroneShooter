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
    [SerializeField] LayerMask RayMask;
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

            //change spread to be inside a uniform circle with a radius proportional to 1/time elapsed
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
                //draw ray from transform to impactPos
                Vector2 dirToImpact = impactPos - (Vector2)transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToImpact, radius, RayMask);
                //Debug.DrawLine(transform.position, radius * dirToImpact, Color.red, 20f);
                if(hit.collider == null || hit.transform.CompareTag("Enemy"))
                {

                    foreach (Collider2D collider in collisions)
                    {
                        //Debug.Log(collider.gameObject.name);
                        if (collider.transform.TryGetComponent(out HealthHandler damageHandler))
                        {
                            damageHandler.TakeDamageGeneral(Damage);
                            //blood
                            Vector2 dirToEnemy = (collider.transform.position - transform.position).normalized;
                            float angle = Mathf.Atan2(dirToEnemy.y, dirToEnemy.x) * Mathf.Rad2Deg;
                        }
                    }
                    Instantiate(ImpactPrefab, impactPos, Quaternion.Euler(0, 0, targetAngle - 180f));
                }
                else
                {
                    Debug.Log("Hit found collider");
                    //since decals zero point is at 90 degrees
                    float normalAngle = Vector2.Angle(Vector2.up, hit.normal); //change to right / left if the decal changes to sideways direction
                    Instantiate(ImpactPrefab, hit.point, Quaternion.Euler(0, 0, normalAngle));
                }

                
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

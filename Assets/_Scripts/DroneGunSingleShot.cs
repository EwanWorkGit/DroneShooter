using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGunSingleShot : MonoBehaviour
{
    public static DroneGunSingleShot Instance;

    [SerializeField] Transform Drone, Crosshair, MouseHair;

    //rotation and opacity
    [SerializeField] float GunConeAngle = 30f, MinFractionValue = 0.2f, MaxCrosshairDist = 1;
    float StartAngle = -90;

    //gun
    [SerializeField] GameObject ImpactPrefab;
    [SerializeField] LayerMask RayMask;
    [SerializeField] float MaxDamage = 4f, MinDamage = 2, ImpactRadius = 0.1f, MaxTimeElapsed = 0.785f, MinTimeElapsedToFire = 0.2f, MaxCrosshairScale = 1.0f, MinCrosshairScale = 0.5f;
    public bool IsAiming = false;
    float TimeElapsed = 0;
    float CurrentDamage = 0;
    
    //visuals
    [SerializeField] Animator Animator;
    [SerializeField] GameObject BloodPrefab1;
    [SerializeField] float ShakeMagnitude = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Crosshair.localScale = new Vector3(MaxCrosshairScale, MaxCrosshairScale, 1);
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
        difference = Mathf.Clamp(difference, -GunConeAngle / 2f, GunConeAngle / 2f);
        targetAngle = droneRot + difference;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        //polar coordinates
        Vector2 crosshairPos = (Vector2)Drone.position + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (targetAngle - StartAngle)), Mathf.Sin(Mathf.Deg2Rad * (targetAngle - StartAngle))) * radius;
        Crosshair.position = crosshairPos;
        MouseHair.position = mousePos;
        //distance-based opacity for mousehair
        float distToCrosshair = Vector2.Distance(crosshairPos, mousePos);
        float distFrac = distToCrosshair / MaxCrosshairDist;
        distFrac = Mathf.Clamp(distFrac, MinFractionValue, 1);
        SpriteRenderer rend = MouseHair.GetComponent<SpriteRenderer>();
        Color color = rend.color;
        color.a = distFrac;
        rend.color = color;

        //Aiming
        float timeFrac = TimeElapsed / MaxTimeElapsed;
        if (Input.GetMouseButton(0))
        {
            IsAiming = true;
            TimeElapsed += Time.deltaTime;
            float inverseTimeFrac = Mathf.Max(1 - timeFrac, 0f);
            float crosshairScale = Mathf.Max(inverseTimeFrac * MaxCrosshairScale, MinCrosshairScale);
            if(TimeElapsed >= MinTimeElapsedToFire)
            {
                CurrentDamage = timeFrac * MaxDamage;
                CurrentDamage = Mathf.Min(CurrentDamage, MaxDamage);
            }
            
            Crosshair.localScale = new Vector3(crosshairScale, crosshairScale, 1);
        }
        if(Input.GetMouseButtonUp(0))
        {
            //Shooting
            if (TimeElapsed >= MinTimeElapsedToFire)
            {
                if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    Animator.SetTrigger("HasFired");
                }

                float distToTarget = Vector2.Distance(crosshairPos, transform.position);
                Vector2 dirToImpact = (crosshairPos - (Vector2)transform.position).normalized;
                Shoot(crosshairPos, dirToImpact, distToTarget, targetAngle, CurrentDamage);
                CameraHandler.Instance.InjectVelocity(-dirToImpact, ShakeMagnitude);
            }

            Crosshair.localScale = new Vector3(MaxCrosshairScale, MaxCrosshairScale, 1);
            TimeElapsed = 0;
            CurrentDamage = 0;
            IsAiming = false;
        }
    }


    void Shoot(Vector2 impactPos, Vector2 dirToImpact, float distToMouse, float targetAngle, float damage)
    {
        //shoot
        Collider2D[] collisions = Physics2D.OverlapCircleAll(impactPos, ImpactRadius);
        //draw ray from transform to impactPos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToImpact, distToMouse, RayMask);
        //Debug.DrawLine(transform.position, radius * dirToImpact, Color.red, 20f);
        if (hit.collider == null || hit.transform.CompareTag("Enemy"))
        {
            foreach (Collider2D collider in collisions)
            {
                //Debug.Log(collider.gameObject.name);
                if (collider.transform.TryGetComponent(out HealthHandler damageHandler))
                {
                    damageHandler.TakeDamageGeneral(damage);
                }
            }
            Instantiate(ImpactPrefab, impactPos, Quaternion.Euler(0, 0, targetAngle - 180f));
        }
        else
        {
            Debug.Log($"Hit {hit.transform}'s collider");
            //since decals zero point is at 90 degrees
            float normalAngle = Vector2.SignedAngle(Vector2.up, hit.normal); //change to right / left if the decal changes to sideways direction
            Instantiate(ImpactPrefab, hit.point, Quaternion.Euler(0, 0, normalAngle));
        }
    }
}

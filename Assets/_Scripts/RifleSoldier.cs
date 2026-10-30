using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RifleSoldier : MonoBehaviour
{
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] Transform ShootingOrigin, Wedge;
    [SerializeField] LayerMask RayMask;
    [SerializeField] int BurstCount = 3;
    [SerializeField] float AngleError = 2f, BurstCooldown = 2f, BetweenShotDelay = 0.2f,
        DelayRandomMax = 0.5f, SightCone = 40, SightRange = 25f, 
        ReactionTime = 0.2f, RotationSpeed = 200, AlertRadius = 15f;
    [SerializeField] Material WedgeMaterial;
    [SerializeField] SpriteRenderer WedgeRend;
    [SerializeField] bool WedgeEnabled = true, AlertRadiusEnabled = false;
    MaterialPropertyBlock PropertyBlock;
    
    //List<Transform> Enemies = new List<Transform>();
    Transform Player;
    float ShootingTimer = 0, ReactionTimer = 0;
    float CurrentCooldown;
    

    public bool DetectedPlayer = false;
    bool NotFiring = true;
    

    private void OnDrawGizmos()
    {
        if(AlertRadiusEnabled)
        {
            Gizmos.DrawWireSphere(transform.position, AlertRadius);
        }
    }

    private void OnValidate()
    {
        UpdateWedge();
    }

    private void Awake()
    {
        //Transform[] enemiesFound = FindObjectsOfType<Transform>();
        //Enemies = enemiesFound.ToList();
        HealthHandler.WasDamaged += TryAlert;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        CurrentCooldown = BurstCooldown + UnityEngine.Random.Range(-DelayRandomMax,DelayRandomMax);
        //starts a little above / under what to reach to randomize firing time at start
        ShootingTimer = BurstCooldown;
    }
    private void Update()
    {
        Vector2 dir = Player.position - ShootingOrigin.position;
        float dist = dir.magnitude;
        dir.Normalize();
        float relativeAngle = Vector2.SignedAngle(transform.up, dir);

        RaycastHit2D hit = Physics2D.Raycast(ShootingOrigin.position, dir, dist, RayMask);
        if(hit.point != Vector2.zero)
        {
            Debug.DrawLine(ShootingOrigin.position, hit.point);
        }

        UpdateWedge();

        bool rayHitPlayer = hit.collider != null && hit.collider.CompareTag("Player");
        bool inSightCone = Mathf.Abs(relativeAngle) < SightCone / 2f;
        bool inRange = Vector2.Distance(ShootingOrigin.position, Player.position) < SightRange;
        
        //inside cone, can raycast, in range, inside shooting cone
        if (inSightCone && inRange && rayHitPlayer)
        {
            //Debug.Log($"INSIDE LINE OF SIGHT OF {transform.name}");
            ReactionTimer += Time.deltaTime;
            if(ReactionTimer > ReactionTime)
            {
                DetectedPlayer = true;
            }
        }
        else
        {
            ReactionTimer = 0;
        }

        if(DetectedPlayer && rayHitPlayer)
        {
            float targetRotationAngle = AbsoluteAngleToPlayer();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotationAngle), RotationSpeed * Time.deltaTime);

            ShootingTimer += Time.deltaTime;
            if (NotFiring && ShootingTimer >= CurrentCooldown)
            {
                StartCoroutine(FireBurst());
            }
        }
        else
        {
            ShootingTimer += Time.deltaTime;
        }
        
    }

    private void OnDisable()
    {
        HealthHandler.WasDamaged -= TryAlert;
    }

    IEnumerator FireBurst()
    {
        NotFiring = false;
        for(int i = 0; i < BurstCount; i++)
        {
            float angleError = UnityEngine.Random.Range(-AngleError, AngleError);
            Instantiate(BulletPrefab, ShootingOrigin.position, Quaternion.Euler(0, 0, transform.eulerAngles.z + angleError - 90));
            if(i < BurstCount-1)
            {
                yield return new WaitForSeconds(BetweenShotDelay);
            }
        }
        NotFiring = true;
        ShootingTimer = 0;
        CurrentCooldown = BurstCooldown + UnityEngine.Random.Range(-DelayRandomMax, DelayRandomMax);
    }

    float AbsoluteAngleToPlayer()
    {
        Vector2 dir = (Player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        return angle;
    }
    void UpdateWedge()
    {
        Wedge.gameObject.SetActive(WedgeEnabled);

        if (PropertyBlock == null)
        {
            PropertyBlock = new();
        }

        if (WedgeMaterial != null)
        {
            WedgeRend.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetFloat("_Angle", SightCone * Mathf.Deg2Rad);
            WedgeRend.SetPropertyBlock(PropertyBlock);
            if (Wedge != null)
            {
                Wedge.localScale = new Vector3(SightRange * 2, SightRange * 2, 1);
            }
        }
    }
    void TryAlert(Transform alertedTransform)
    {
        float dist = Vector2.Distance(alertedTransform.position, transform.position);
        if(dist <= AlertRadius)
        {
            DetectedPlayer = true;
        }
    }
}

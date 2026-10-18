using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleSoldier : MonoBehaviour
{
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] Transform ShootingOrigin, Wedge;
    [SerializeField] LayerMask RayMask;
    [SerializeField] int BurstCount = 3;
    [SerializeField] float AngleError = 2f, BurstCooldown = 2f, BetweenShotDelay = 0.2f,
        DelayRandomMax = 0.5f, SightCone = 40, SightRange = 25f, 
        ReactionTime = 0.2f, RotationSpeed = 200;
    [SerializeField] Material WedgeMaterial;
    

    Transform Player;
    float ShootingTimer = 0, ReactionTimer = 0;
    float CurrentCooldown;
    
    bool NotFiring = true;
    bool Detected = false;

    private void OnValidate()
    {
        if(WedgeMaterial != null)
        {
            WedgeMaterial.SetFloat("_Angle", SightCone * Mathf.Deg2Rad);
            if(Wedge != null)
            {
                Wedge.localScale = new Vector3(SightRange * 2, SightRange * 2, 1);
            }
        }
        
    }

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        CurrentCooldown = BurstCooldown + Random.Range(-DelayRandomMax,DelayRandomMax);
        //starts a little above / under what to reach to randomize firing time at start
        ShootingTimer = BurstCooldown;
        Wedge.gameObject.SetActive(true);
        Wedge.transform.localScale = new Vector3(SightRange*2, SightRange*2, 1);

        WedgeMaterial.SetFloat("_Angle", SightCone*Mathf.Deg2Rad);
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
        

        bool hitPlayer = hit.collider != null && hit.collider.CompareTag("Player");
        bool inSightCone = Mathf.Abs(relativeAngle) < SightCone / 2f;
        bool inRange = Vector2.Distance(ShootingOrigin.position, Player.position) < SightRange;
        
        //inside cone, can raycast, in range, inside shooting cone
        if (inSightCone && inRange && hitPlayer)
        {
            Debug.Log($"INSIDE LINE OF SIGHT OF {transform.name}");
            ReactionTimer += Time.deltaTime;
            if(ReactionTimer > ReactionTime)
            {
                Detected = true;
            }
        }
        else
        {
            ReactionTimer = 0;
        }

        if(Detected)
        {
            float targetRotationAngle = AbsoluteAngleToPlayer();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotationAngle), RotationSpeed * Time.deltaTime);

            ShootingTimer += Time.deltaTime;
            if (NotFiring && ShootingTimer >= CurrentCooldown)
            {
                StartCoroutine(FireBurst());
            }
        }
        
    }
    IEnumerator FireBurst()
    {
        NotFiring = false;
        for(int i = 0; i < BurstCount; i++)
        {
            float angleError = Random.Range(-AngleError, AngleError);
            Instantiate(BulletPrefab, ShootingOrigin.position, Quaternion.Euler(0, 0, transform.eulerAngles.z + angleError - 90));
            if(i < BurstCount-1)
            {
                yield return new WaitForSeconds(BetweenShotDelay);
            }
        }
        NotFiring = true;
        ShootingTimer = 0;
        CurrentCooldown = BurstCooldown + Random.Range(-DelayRandomMax, DelayRandomMax);
    }

    float AbsoluteAngleToPlayer()
    {
        Vector2 dir = (Player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        return angle;
    }
}

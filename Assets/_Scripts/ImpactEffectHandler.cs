using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectHandler : MonoBehaviour
{
    [SerializeField] float ShrunkSize, ShrinkTime;
    float BaseSize, Timer;

    private void Awake()
    {
        BaseSize = transform.localScale.x;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        float frac = Timer / ShrinkTime;
        float scale = Mathf.Lerp(BaseSize, ShrunkSize, Mathf.Min(frac, 1));
        transform.localScale = Vector3.one * scale;
        if(frac >= 1)
        {
            Destroy(gameObject);
        }
    }
}

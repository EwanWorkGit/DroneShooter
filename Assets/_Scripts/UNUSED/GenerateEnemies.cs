using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateEnemies : MonoBehaviour
{
    //Grid Geometry
    [Header("Grid Geometry")]
    [SerializeField] float GridSpacing = 2f;

    //Enemies
    [Header("Enemy Generation")]
    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] int DensitySpots = 3;
    [SerializeField] float BaseSpawnChance = 0.02f, MaxSpawnBonus = 0.2f; //0.01f = 1%

    Vector2[,] Positions;
    Vector2[] DensitySpotPositions;

    /*private void OnDrawGizmos()
    {
        if(Positions != null && Positions.Length > 0)
        {
            //density changes color
            for (int x = 0; x < Positions.GetLength(0); ++x)
            {
                for (int y = 0; y < Positions.GetLength(1); ++y)
                {
                    Gizmos.color = Color.red;
                    Vector2 pos = Positions[x, y];
                    if(DensitySpotPositions.Contains(pos))
                    {
                        Gizmos.color = Color.cyan;
                    }
                    
                    Gizmos.DrawSphere(pos, 0.2f);
                }
            }
            
        }
    }*/

    private void Awake()
    {
        //Positions
        Vector2 scale = (Vector2)transform.lossyScale;
        float xPer = scale.x / GridSpacing;
        float yPer = scale.y / GridSpacing;
        int countX = Mathf.CeilToInt(xPer) + 1;
        int countY = Mathf.CeilToInt(yPer) + 1;
        Positions = new Vector2[countX, countY];
        Debug.Log($"x: {countX} | y: {countY}");
        Vector2 offset = new Vector2(scale.x / 2f, scale.y / 2f);
        for (int x = 0; x<countX; ++x)
        {
            for (int y = 0; y < countY; ++y)
            {
                
                Positions[x,y] = (Vector2)transform.position + ((new Vector2(x, y)) * GridSpacing) - offset;
            }
        }

        //Density Spots
        DensitySpotPositions = new Vector2[DensitySpots];
        List<Vector2> generatedPositions = new();
        for(int i = 0; i < DensitySpots; i++)
        {
            bool foundPos = false;
            int attemps = 0;
            while (!foundPos && attemps < 1000)
            {
                int randX = Random.Range(0, Positions.GetLength(0));
                int randY = Random.Range(0, Positions.GetLength(1));
                Vector2 pos = Positions[randX, randY];
                if (!generatedPositions.Contains(pos))
                {
                    DensitySpotPositions[i] = pos;
                    generatedPositions.Add(pos);
                    foundPos = true;
                }
                else
                {
                    attemps++;
                }
            }
            
            if(foundPos)
            {
                Debug.Log("Generated density spot.");
            }
            else
            {
                Debug.Log("Found no position for density spot.");
            }
            
        }

        //Placing Enemies

    }
}

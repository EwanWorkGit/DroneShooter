using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OutOfRangeMarkers : MonoBehaviour
{
    [SerializeField] GameObject MarkerPrefab;

    List<RifleSoldier> Enemies = new();
    List<GameObject> Markers = new();

    private void Awake()
    {
        Enemies = FindObjectsOfType<RifleSoldier>().ToList();
        while(Markers.Count < Enemies.Count)
        {
            GameObject marker = Instantiate(MarkerPrefab);
            Markers.Add(marker);
        }
        Debug.Log(Enemies.Count);
    }

    private void Update()
    {
        List<RifleSoldier> toRemove = new();
        for(int i = 0; i < Enemies.Count; i++)
        {
            RifleSoldier enemy = Enemies[i];
            if(enemy != null)
            {
                if (enemy.DetectedPlayer)
                {
                    Vector3 pos = Camera.main.WorldToViewportPoint(enemy.transform.position);
                    bool isVisible = pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1 && pos.z >= 0;
                    if (!isVisible)
                    {
                        
                        Markers[i].SetActive(true);

                        //if pos.x > 1, then marker x = 1, else marker x = pos.x
                        //if pos.y > 1, then marker y = 1, else marker y = pos.y

                        Vector3 markerPos = new Vector3(0,0,0);

                        if(pos.x >= 1)
                        {
                            markerPos.x = 0.985f;
                        }
                        else if(pos.x <= 0)
                        {
                            markerPos.x = 0;
                        }
                        else
                        {
                            markerPos.x = pos.x;
                        }

                        if(pos.y >= 1)
                        {
                            markerPos.y = 0.985f;
                        }
                        else if(pos.y <= 0)
                        {
                            markerPos.y = 0;
                        }
                        else
                        {
                            markerPos.y = pos.y;
                        }

                        markerPos = Camera.main.ViewportToWorldPoint(markerPos);
                        markerPos.z = 0;
                        Markers[i].transform.position = markerPos;
                        
                    }
                    else
                    {
                        Markers[i].SetActive(false);
                    }
                }
                else
                {
                    Markers[i].SetActive(false);
                }
            }
            else
            {
                toRemove.Add(enemy);
                Markers[i].SetActive(false);
            }
        }

        for(int i = 0; i < toRemove.Count; i++)
        {
            int index = Enemies.IndexOf(toRemove[i]);
            Enemies.Remove(Enemies[index]);
            Markers.Remove(Markers[index]);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class RegionData
{
    public int population;
    public float energyStored;
    public float energyDemand;
    public float energyProduction;
    public float emmisionCO2;
    public float area;
    public List<ResourceStorage> resources;

    
    public RegionData(int p, float eS, float eD, float eP, float eCO2)
    {
        population       = p;
        energyStored     = eS;
        energyDemand     = eD;
        energyProduction = eP;
        emmisionCO2      = eCO2;

        resources = new List<ResourceStorage>()
        {
            new ResourceStorage(ResourceList.coal,    0),
            new ResourceStorage(ResourceList.lignite, 0),
            new ResourceStorage(ResourceList.gas,     0),
        };
    }

    public void FixResourceStorage()
    {
        if(resources == null)
        {
            resources = new List<ResourceStorage>()
            {
                new ResourceStorage(ResourceList.coal,    0),
                new ResourceStorage(ResourceList.lignite, 0),
                new ResourceStorage(ResourceList.gas,     0),
            };
        }
    }
}

public class RegionHover : MonoBehaviour
{
    public string regionName = "default";
    public RegionManager regionManager;
    public Regions region = Regions.None;

    SpriteRenderer spriteRenderer;
    Collider2D regionCollider;
    Collider2D edgeCollider;

    public RegionData regionData;


    private void OnMouseEnter()
    {
        //Debug.Log(string.Format("RegionEnter %s", regionName));
        spriteRenderer.color = new Color(0.3764559f, 0.6698113f, 0.2495995f, 1.0f);
        regionManager.selectedRegion = region;
        regionManager.regionCollider = regionCollider;
        regionManager.edgeCollider = edgeCollider;
    }

    private void OnMouseExit()
    {
        //Debug.Log(string.Format("RegionExit %s", regionName));
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        regionManager.selectedRegion = Regions.None;
        regionManager.regionCollider = null;
        regionManager.edgeCollider = null;
    }

    void Start()
    {
        regionData.FixResourceStorage();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        regionCollider = gameObject.GetComponent<PolygonCollider2D>();
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

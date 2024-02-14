using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public RegionManager regionManager;
    public bool isCursorBusy = false;


    public double cash;
    public RegionHover[] regions;

    public double GetCash()
    {
        return cash;
    }

    public RegionData GetAllRegionData()
    {
        Regions[] regionList = new Regions[7]
        {
            Regions.Central,
            Regions.East,
            Regions.Mazowsze,
            Regions.North,
            Regions.NorthWest,
            Regions.South,
            Regions.SouthWest
        };

        RegionData countryData = new RegionData(0, 0, 0, 0, 0);

        foreach(Regions r in regionList)
        {
            RegionData data = GetRegionData(r);
            countryData.population += data.population;
            countryData.energyStored += data.energyStored;
            countryData.energyDemand += data.energyDemand;
            countryData.energyProduction += data.energyProduction;
            countryData.emmisionCO2  += data.emmisionCO2;

            for (int i = 0; i < data.resources.Count; i++)
            {
                countryData.resources[i].quantity += data.resources[i].quantity;
            }
        }

        return countryData;
    }

    public RegionData GetRegionData(Regions region)
    {
        if(region == Regions.None)
        {
            return GetAllRegionData();
        }

        foreach(RegionHover rH in regions)
        {
            if(rH.region == region)
            {
                return rH.regionData;
            }
        }
        return null;
    } 


    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    void Update()
    {
        
    }
}

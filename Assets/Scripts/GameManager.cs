using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public RegionManager regionManager;
    public bool isCursorBusy = false;

    public bool isGamePaused = false;


    public double cash;
    public RegionHover[] regions;

    public float hoursPerSecond = 12;
    float updateTimer = 0f;

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

    public void TickRegions()
    {
        foreach(RegionHover region in regions)
        {
            region.regionData.energyProduction = 0f;
            region.regionData.energyDemand = 0f;

            Placeable[] objects = region.gameObject.GetComponentsInChildren<Placeable>();
            foreach (Placeable prop in objects)
            {
                ObjectData data = prop.objectData;

                bool meetsResourceRequirements = true;
                foreach (ResourceProduction rP in data.efficiencyLevels[data.efficiencyLevel - 1].resourceConsumption)
                {
                    if (region.regionData.resources[(int)rP.resource].quantity < rP.quantity) {
                        meetsResourceRequirements = false;
                        break;
                    }
                }
                if (meetsResourceRequirements && data.buildState == BuildState.Working && cash >= data.efficiencyLevels[data.efficiencyLevel-1].operativeCost)
                {
                    region.regionData.energyProduction += data.efficiencyLevels[data.efficiencyLevel-1].energyProduction;
                    region.regionData.energyProduction += data.efficiencyLevels[data.efficiencyLevel-1].energyConsumption;
                    region.regionData.emmisionCO2 += data.efficiencyLevels[data.efficiencyLevel - 1].emmision;
                    if (region.regionData.emmisionCO2 < 0) region.regionData.emmisionCO2 = 0;
                    cash += data.efficiencyLevels[data.efficiencyLevel - 1].profit;
                    cash -= data.efficiencyLevels[data.efficiencyLevel - 1].operativeCost;
                    foreach(ResourceProduction rP in data.efficiencyLevels[data.efficiencyLevel-1].resourceProduction)
                    {
                        region.regionData.resources[(int)rP.resource].quantity += rP.quantity;
                    }
                    foreach (ResourceProduction rP in data.efficiencyLevels[data.efficiencyLevel - 1].resourceConsumption)
                    {
                        region.regionData.resources[(int)rP.resource].quantity -= rP.quantity;
                    }
                }
                if(data.buildState != BuildState.Working)
                {
                    if(data.finishTime <= Game.UnixTimeStamp())
                    {
                        switch (data.buildState)
                        {
                            case (BuildState.Contstruction):
                                data.buildState = BuildState.Working;
                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Upgrade):
                                data.buildState = BuildState.Working;
                                data.efficiencyLevel = Mathf.Clamp(data.efficiencyLevel + 1, 1, data.maxEfficiencyLevel);
                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Demolition):
                                Destroy(prop.gameObject);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            region.regionData.energyStored += region.regionData.energyProduction;
            region.regionData.energyStored -= region.regionData.energyDemand;
        }
    }

    public void Update()
    {
        updateTimer += Time.deltaTime * hoursPerSecond;
        if (updateTimer < 24f) return;
        updateTimer = 0f;

        TickRegions();
    }
}

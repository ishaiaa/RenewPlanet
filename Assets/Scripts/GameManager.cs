using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{   
    public RegionManager regionManager;
    public GameObject researchFacility;
    public GameObject ministryFacility;

    public DateTime startDay = new DateTime(2024, 1, 1);
    public double daysPassed = 0;
    public Text dateDisplay;

    public ToastManager toastManager;

    public double minDeathsFactor = 1.000000068; //800/365/38mln
    public double maxDeathsFactor = 1.000000084; //1100/365/38mln

    public double minBirthsFactor = 1.000000076; //900/365/38mln
    public double maxBirthsFactor = 1.000000091; //1200/365/38mln

    public double[] emmisionTresholds = new double[] {1100,1300,1500};

    public bool isGamePaused = false;

    public double cash;
    public RegionHover[] regions;

    public float hoursPerSecond = 12;
    float updateTimer = 0f;

    public static bool IsCursorBusy()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == 5)
            {
                Debug.Log(result.gameObject.name);
                Debug.Log(result.gameObject.transform.parent.name);
                Debug.Log(result.gameObject.layer);
                return true;
            }

        }
        return false;
    }



    public double GetCash()
    {
        return cash;
    }

    public int GetResearchCap()
    {
        return researchFacility.GetComponent<Placeable>().objectData.efficiencyLevel;
    }

    public int GetBuildCap()
    {
        return ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel;
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
            countryData.area += data.area;
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

    public List<ResourceProduction[]> GetResourceStatsInRegion(Regions region)
    {
        List<ResourceProduction[]> rP = new List<ResourceProduction[]>();

        ResourceProduction coalProduction = new ResourceProduction();
        ResourceProduction uraniumProduction = new ResourceProduction();
        ResourceProduction gasProduction = new ResourceProduction();

        ResourceProduction coalDemand = new ResourceProduction();
        ResourceProduction uraniumDemand = new ResourceProduction();
        ResourceProduction gasDemand = new ResourceProduction();

        Placeable[] copies = GameObject.FindObjectsOfType<Placeable>();
        foreach (Placeable p in copies)
        {
            GameObject obj = p.gameObject;
            if (region != Regions.None && obj.transform.parent.gameObject.GetComponent<RegionHover>().region != region) continue;
            if (p.objectData.buildState != BuildState.Working) continue;

            foreach (ResourceProduction rDemand in p.objectData.efficiencyLevels[p.objectData.efficiencyLevel-1].resourceConsumption)
            {
                switch(rDemand.resource)
                {
                    case (ResourceType.Coal):
                        coalDemand.quantity += rDemand.quantity;
                        break;
                    case (ResourceType.Uranium):
                        uraniumDemand.quantity += rDemand.quantity;
                        break;
                    case (ResourceType.Gas):
                        gasDemand.quantity += rDemand.quantity;
                        break;
                    default:
                        break;
                }
            }

            foreach (ResourceProduction rProd in p.objectData.efficiencyLevels[p.objectData.efficiencyLevel-1].resourceProduction)
            {
                switch (rProd.resource)
                {
                    case (ResourceType.Coal):
                        coalProduction.quantity += rProd.quantity;
                        break;
                    case (ResourceType.Uranium):
                        uraniumProduction.quantity += rProd.quantity;
                        break;
                    case (ResourceType.Gas):
                        gasProduction.quantity += rProd.quantity;
                        break;
                    default:
                        break;
                }
            }
        }
        ResourceProduction[] resourceProductions = new ResourceProduction[] { coalProduction, uraniumProduction, gasProduction };
        ResourceProduction[] resourceDemands = new ResourceProduction[] { coalDemand, uraniumDemand, gasDemand };

        rP.Add(resourceProductions);
        rP.Add(resourceDemands);

        return rP;
    }

    public double[] GetEnergyBalanceInRegion(Regions region)
    {
        double energyProduction = 0;
        double energyConsumption = 0;
        
        Placeable[] copies = GameObject.FindObjectsOfType<Placeable>();
        foreach (Placeable p in copies)
        {
            GameObject obj = p.gameObject;
            if (region != Regions.None && obj.transform.parent.gameObject.GetComponent<RegionHover>().region != region) continue;
            if (p.objectData.buildState != BuildState.Working) continue;

            energyProduction += p.objectData.efficiencyLevels[p.objectData.efficiencyLevel - 1].energyProduction;
            energyConsumption += p.objectData.efficiencyLevels[p.objectData.efficiencyLevel - 1].energyConsumption;
        }

        return new double[] { energyProduction, energyConsumption };
    }

    public bool DoesExistInRegion(Placeable placeable, Regions region)
    {
        GameObject[] copies = GameObject.FindGameObjectsWithTag(placeable.objectData.name);
        foreach (GameObject obj in copies)
        {
            if (obj.transform.parent.gameObject.GetComponent<RegionHover>().region == region) return true;
        }
        return false;
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    public void TickRegions()
    {
        foreach(RegionHover region in regions)
        {
            double[] ePC = GetEnergyBalanceInRegion(region.region);
            List<ResourceProduction[]> rPC = GetResourceStatsInRegion(region.region);

            region.regionData.energyProduction = ePC[0];
            region.regionData.energyDemand = ePC[1];

            Placeable[] objects = region.gameObject.GetComponentsInChildren<Placeable>();
            foreach (Placeable prop in objects)
            {
                ObjectData data = prop.objectData;

                if(data.buildState == BuildState.Working)
                {
                    if(data.efficiencyLevels[data.efficiencyLevel-1].resourceConsumption.Length == 0)
                    {
                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                        region.regionData.emmisionCO2 += data.efficiencyLevels[data.efficiencyLevel - 1].emmision;
                    }
                    else
                    {
                        foreach(ResourceProduction rC in data.efficiencyLevels[data.efficiencyLevel - 1].resourceConsumption)
                        {
                            switch (rC.resource)
                            {
                                case (ResourceType.Coal):
                                    if(rPC[1][0].quantity >= rC.quantity)
                                    {
                                        rPC[1][0].quantity -= rC.quantity;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                    }
                                    else
                                    {
                                        region.regionData.energyProduction -= region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        toastManager.Toast("Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
                                    }
                                    break;
                                case (ResourceType.Uranium):
                                    if (rPC[1][1].quantity >= rC.quantity)
                                    {
                                        rPC[1][1].quantity -= rC.quantity;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                    }
                                    else
                                    {
                                        region.regionData.energyProduction -= region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        toastManager.Toast("Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
                                    }
                                    break;
                                case (ResourceType.Gas):
                                    if (rPC[1][2].quantity >= rC.quantity)
                                    {
                                        rPC[1][2].quantity -= rC.quantity;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                    }
                                    else
                                    {
                                        region.regionData.energyProduction -= region.regionData.energyStored += data.efficiencyLevels[data.efficiencyLevel - 1].energyProduction;
                                        toastManager.Toast("Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                //

                if(data.buildState != BuildState.Working)
                {
                    if(data.finishTime <= Game.UnixTimeStamp())
                    {
                        switch (data.buildState)
                        {
                            case (BuildState.Contstruction):
                                data.buildState = BuildState.Working;
                                toastManager.Toast("Ukoñczono budowê - " + data.efficiencyLevels[data.efficiencyLevel-1].name, ToastMode.Info, 5f);
                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Upgrade):
                                data.buildState = BuildState.Working;
                                data.efficiencyLevel = Mathf.Clamp(data.efficiencyLevel + 1, 1, data.maxEfficiencyLevel);
                                toastManager.Toast("Ulepszono - " + data.efficiencyLevels[data.efficiencyLevel - 1].name, ToastMode.Info, 5f);

                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Demolition):
                                toastManager.Toast("Wyburzono - " + data.efficiencyLevels[data.efficiencyLevel - 1].name, ToastMode.Info, 5f);
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

    public void UpdateUICalendar()
    {
        DateTime currentDate = startDay.AddDays(daysPassed);
        dateDisplay.text = currentDate.ToString("dd.MM.yyyy");
    }

    public void Update()
    {
        updateTimer += Time.deltaTime * hoursPerSecond;
        if (updateTimer < 24f) return;
        updateTimer = 0f;
        daysPassed++;
        UpdateUICalendar();
        TickRegions();
    }
}

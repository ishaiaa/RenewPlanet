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
    public ResearchManager researchManager;
    public ObjectPlacer objectPlacer;

    public DateTime startDay = new DateTime(2024, 1, 1);
    public double daysPassed = 0;
    public Text dateDisplay;

    public ToastManager toastManager;

    public double[] pricePerKiloWatt = new double[] { 0.9, 1.0, 1.1 };
    public double baseDailyEnergyConsumption = 900;
    public double energyDemandIncreasePerYear = 0.5;

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
    float autoSaveTimer = 0f;

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
                return true;
            }

        }
        return false;
    }

    public double GetCurrentEnergyDemand()
    {
        return Math.Round(baseDailyEnergyConsumption + (baseDailyEnergyConsumption * (daysPassed / 365) * (energyDemandIncreasePerYear)), 2);
    }

    public double GetPricePerKilowatt()
    {
        if (ministryFacility == null)
        {
            ministryFacility = GameObject.FindGameObjectWithTag("Budynek Administracyjny");
        }
        return pricePerKiloWatt[ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel - 1];
    }

    public double GetCash()
    {
        return cash;
    }

    public int GetResearchCap()
    {
        if(researchFacility == null)
        {
            researchFacility = GameObject.FindGameObjectWithTag("Oœrodek Badawczy");
        }
        return researchFacility.GetComponent<Placeable>().objectData.efficiencyLevel;
    }

    public int GetBuildCap()
    {
        if (ministryFacility == null)
        {
            ministryFacility = GameObject.FindGameObjectWithTag("Budynek Administracyjny");
        }
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

    public GameObject[] GetAllPlacedObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        Placeable[] copies = GameObject.FindObjectsOfType<Placeable>();
        foreach(Placeable p in copies)
        {
            if (!p.isPlaced) continue;
            objects.Add(p.gameObject);
        }

        return objects.ToArray();
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

    public RegionHover GetRegion(Regions region)
    {
        if (region == Regions.None)
        {
            return null;
        }

        foreach (RegionHover rH in regions)
        {
            if (rH.region == region)
            {
                return rH;
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
            if (!p.isPlaced) continue;
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
            if (!p.isPlaced) continue;
            if (region != Regions.None && obj.transform.parent.gameObject.GetComponent<RegionHover>().region != region) continue;
            if (p.objectData.buildState != BuildState.Working) continue;

            energyProduction += p.objectData.efficiencyLevels[p.objectData.efficiencyLevel - 1].energyProduction;
            energyConsumption += p.objectData.efficiencyLevels[p.objectData.efficiencyLevel - 1].energyConsumption;
        }

        return new double[] { energyProduction, energyConsumption };
    }

    public int GetConstructionWorkingsCount(Regions region)
    {
        Placeable[] copies = GameObject.FindObjectsOfType<Placeable>();
        int count = 0;
        
        foreach (Placeable obj in copies)
        {
            if (!obj.isPlaced) continue;
            if (obj.gameObject.transform.parent.gameObject.GetComponent<RegionHover>().region != region) continue;
            if (obj.objectData.buildState != BuildState.Working) count++;
        }
        return count;
    }

    public bool DoesExistInRegion(Placeable placeable, Regions region)
    {
        GameObject[] copies = GameObject.FindGameObjectsWithTag(placeable.objectData.name);
        foreach (GameObject obj in copies)
        {
            if (!obj.GetComponent<Placeable>().isPlaced) continue;
            if (obj.transform.parent.gameObject.GetComponent<RegionHover>().region == region) return true;
        }
        return false;
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        LoadGameFromSave();
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

            double r1 = (double)Random.Range(0.0f, 1.0f);
            double r2 = (double)Random.Range(0.0f, 1.0f);

            double births = Random.Range(0, 5);
            double deaths = Random.Range(0, 4);


            region.regionData.population += births - deaths;

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
                                        toastManager.Toast(SoundEffect.Warning, "Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
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
                                        toastManager.Toast(SoundEffect.Warning, "Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
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
                                        toastManager.Toast(SoundEffect.Warning, "Jedna z elektrowni przerwa³a pracê z powodu niewystarczaj¹cej iloœci zasobów", ToastMode.Warning, 5f);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (data.buildState != BuildState.Working)
                {
                    if(data.finishTime <= Game.UnixTimeStamp())
                    {
                        switch (data.buildState)
                        {
                            case (BuildState.Contstruction):
                                data.buildState = BuildState.Working;
                                toastManager.Toast(SoundEffect.Success, "Ukoñczono budowê - " + data.efficiencyLevels[data.efficiencyLevel-1].name, ToastMode.Info, 5f);
                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Upgrade):
                                data.buildState = BuildState.Working;
                                data.efficiencyLevel = Mathf.Clamp(data.efficiencyLevel + 1, 1, data.maxEfficiencyLevel);
                                toastManager.Toast(SoundEffect.Success, "Ulepszono - " + data.efficiencyLevels[data.efficiencyLevel - 1].name, ToastMode.Info, 5f);

                                prop.UpdateVisuals();
                                break;
                            case (BuildState.Demolition):
                                toastManager.Toast(SoundEffect.Success, "Wyburzono - " + data.efficiencyLevels[data.efficiencyLevel - 1].name, ToastMode.Info, 5f);
                                Destroy(prop.gameObject);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            double demand = GetCurrentEnergyDemand();
            double totalDemand = Math.Round(region.regionData.population * demand, 2);
            region.regionData.energyDemand += totalDemand;

            if(region.regionData.energyStored >= totalDemand)
            {
                region.regionData.energyStored -= totalDemand;
                cash = Math.Floor(cash + ((totalDemand/1000) * GetPricePerKilowatt()));
            }
            else
            {
                double missingEnergy = totalDemand - region.regionData.energyStored;
                double energyToSell = totalDemand - missingEnergy;

                cash = Math.Floor(cash + ((energyToSell / 1000) * GetPricePerKilowatt()));
                toastManager.Toast(SoundEffect.Warning, "Deficyt energi! Makroregion " + regionManager.regionsNames[region.region], ToastMode.Warning, 5f);
                region.regionData.energyStored = 0;
            }

        }
    }

    public void UpdateUICalendar()
    {
        DateTime currentDate = startDay.AddDays(daysPassed);
        dateDisplay.text = currentDate.ToString("dd.MM.yyyy");
    }

    public string GetUICalendar()
    {
        return dateDisplay.text;
    }

    public void Update()
    {
        if (ministryFacility == null)
        {
            ministryFacility = GameObject.FindGameObjectWithTag("Budynek Administracyjny");
        }
        if (researchFacility == null)
        {
            researchFacility = GameObject.FindGameObjectWithTag("Oœrodek Badawczy");
        }

        updateTimer += Time.deltaTime * hoursPerSecond;
        if (updateTimer < 24f) return;
        updateTimer = 0f;
        daysPassed++;
        UpdateUICalendar();
        TickRegions();
    }

    public void ManualSave()
    {
        SaveSystem.SaveGameState(new SaveData(this));
        toastManager.Toast("Rêczny Zapis", ToastMode.Success, 5f);
    }


    public void LoadGameFromSave()
    {
        SaveData saveData = SaveSystem.LoadGameState();
        if (saveData == null) return;

        cash = saveData.cash;
        daysPassed = saveData.daysPassed;

        int index = 0;
        foreach(RegionHover rHover in regions)
        {
            RegionSaveData rsd = saveData.regions[index];

            rHover.regionData.population = rsd.population;
            rHover.regionData.emmisionCO2 = rsd.emmisionCO2;
            rHover.regionData.energyStored = rsd.energyStored;

            index++;
        }

        Placeable[] placeableObjects = GameManager.FindObjectsOfType<Placeable>();
        foreach(Placeable p in placeableObjects)
        {
            if (p.gameObject.layer == 6) Destroy(p.gameObject);
        }

        foreach(ObjectSaveData osd in saveData.objects)
        {
            objectPlacer.PlaceFromSave(osd);
        }

        foreach (ResearchSaveData rsd in saveData.researchLevels)
        {
            researchManager.UpdateFromSave(rsd);
        }

        GameObject rFacility = GameObject.FindGameObjectWithTag("Oœrodek Badawczy");
        GameObject mFacility = GameObject.FindGameObjectWithTag("Budynek Administracyjny");

        researchFacility = rFacility;
        ministryFacility = mFacility;
    }


    public void LateUpdate()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer < 600f) return;
        SaveSystem.SaveGameState(new SaveData(this));
        toastManager.Toast("Automatyczny Zapis", ToastMode.Success, 5f);
    }
}

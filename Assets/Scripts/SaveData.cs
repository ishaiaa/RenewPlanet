using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResearchSaveData
{
    public string researchTarget;
    public int level;
    public int researchState;
    public double researchFinishTime;

    public ResearchSaveData() { }
    public ResearchSaveData(string researchTarget, int level, int researchState, double researchFinishTime)
    {
        this.researchTarget = researchTarget;
        this.level = level;
        this.researchState = researchState;
        this.researchFinishTime = researchFinishTime;
    }
} 


public class ObjectSaveData
{
    public string name;
    public float posX;
    public float posY;
    public float posZ;

    public int regionOrigin;

    public int level;
    public int buildState;
    public double buildFinishTime;

    

    public ObjectSaveData() { }
    public ObjectSaveData(string name, float posX, float posY, float posZ, int regionOrigin, int level, int buildState, double buildFinishTime, ResearchSaveData[] researchLevels)
    {
        this.name = name;
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;

        this.regionOrigin = regionOrigin;

        this.level = level;
        this.buildState = buildState;
        this.buildFinishTime = buildFinishTime;
    }
}

public class RegionSaveData
{
    public double population;
    public double emmisionCO2;
    public double energyStored;

    public RegionSaveData() { }

    public RegionSaveData(double population, double emmisionCO2, double energyStored)
    {
        this.population = population;
        this.emmisionCO2 = emmisionCO2;
        this.energyStored = energyStored;
    }
}

public class SaveData
{
    public double cash;
    public double daysPassed;

    public RegionSaveData[] regions;
    public ObjectSaveData[] objects;
    public ResearchSaveData[] researchLevels;

    public SaveData() { }

    public SaveData(GameManager gameManager)
    {
        this.cash = gameManager.cash;
        this.daysPassed = gameManager.daysPassed;

        GameObject[] objects = gameManager.GetAllPlacedObjects();
        List<ObjectSaveData> osd = new List<ObjectSaveData>();

        foreach(GameObject gObject in objects)
        {
            ObjectSaveData objData = new ObjectSaveData();

            Transform t = gObject.GetComponent<Transform>();
            Placeable p = gObject.GetComponent<Placeable>();
            RegionHover r = gObject.transform.parent.gameObject.GetComponent<RegionHover>();

            objData.name = gObject.tag;
            objData.posX = t.position.x;
            objData.posY = t.position.y;
            objData.posZ = t.position.z;
            objData.regionOrigin = (int)r.region;
            objData.level = p.objectData.efficiencyLevel;
            objData.buildState = (int)p.objectData.buildState;
            objData.buildFinishTime = p.objectData.finishTime;
            osd.Add(objData);
        }

        this.objects = osd.ToArray();

        List<RegionSaveData> regionsData = new List<RegionSaveData>();
        foreach(RegionHover rHover in gameManager.regions)
        {
            RegionSaveData regionSD = new RegionSaveData();

            regionSD.emmisionCO2 = rHover.regionData.emmisionCO2;
            regionSD.energyStored = rHover.regionData.energyStored;
            regionSD.population = rHover.regionData.population;

            regionsData.Add(regionSD);
        }
        this.regions = regionsData.ToArray();

        List<ResearchSaveData> rSD = new List<ResearchSaveData>();

        foreach (ShopCard sC in gameManager.researchManager.shopItems)
        {
            foreach(EfficiencyLevel eL in sC.objectData.efficiencyLevels)
            {
                ResearchSaveData rData = new ResearchSaveData();

                rData.researchTarget = sC.objectData.name;
                rData.level = eL.level;
                rData.researchState = (int)eL.researchState;
                rData.researchFinishTime = eL.researchFinishTime;

                rSD.Add(rData);
            }
        }
        this.researchLevels = rSD.ToArray();
    }
}
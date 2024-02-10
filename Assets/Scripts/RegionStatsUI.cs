using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionStatsUI : MonoBehaviour
{
    [Serializable]
    public class StatsDisplay
    {
        public string name;
        public Text cash;
        public RawImage icon;
    }
    
    public Text regionTitle;

    public RegionManager regionManager;
    public CameraManager cameraManager;
    public GameManager gameManager;

    public List<StatsDisplay> statistics;

    public struct RegionStatsData
    {
        public float cash;
        public int population;
        public float energyStored;
        public float energyDemand;
        public float energyProduction;
        public float emmisionCO2;

        public RegionStatsData(float c, int p, float eS, float eD, float eP, float eCO2)
        {
            cash = c;
            population = p;
            energyStored = eS;
            energyDemand = eD;
            energyProduction = eP;
            emmisionCO2 = eCO2;
        }
    }

    RegionStatsData GetRegionStats(Regions region)
    {
        return new RegionStatsData(0.0f, 0, 0.0f, 0.0f, 0.0f, 0.0f);
    }

    void UpdateStatsDisplay(Regions region, RegionStatsData data)
    {
        if(region == Regions.None)
        {
            regionTitle.text = "Ca³y Kraj";
            return;
        }
        regionTitle.text = "Region " + regionManager.regionsNames[region];
    }

    // Update is called once per frame
    void Update()
    {
        Regions currentRegion = regionManager.selectedRegion;
        if (cameraManager.cameraScale >= 8f) currentRegion = Regions.None;
        UpdateStatsDisplay(currentRegion, GetRegionStats(currentRegion));
    }
}

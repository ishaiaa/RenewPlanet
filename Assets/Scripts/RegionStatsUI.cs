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

    RegionData GetRegionStats(Regions region)
    {
        return gameManager.GetRegionData(region);
    }

    public string FormatCash(double cash)
    {
        return string.Format("{0:N0}", cash);
    }

    void UpdateStatsDisplay(Regions region, RegionData data)
    {
        //Stan Konta
        //Populacja
        //Przechowana Energia
        //Zapotrzebowanie Na Energiê
        //Produkcja Energii
        //Poziom CO2

        regionTitle.text = region == Regions.None? "Ca³y Kraj" : "Region " + regionManager.regionsNames[region];
        statistics[0].cash.text = Game.FormatCash(gameManager.GetCash());
        statistics[1].cash.text = Game.FormatCash(data.population);
        statistics[2].cash.text = Game.FormatUnits(data.energyStored) + "W";
        statistics[3].cash.text = Game.FormatUnits(data.energyDemand) + "W/h";
        statistics[4].cash.text = Game.FormatUnits(data.energyDemand) + "W/h";
        statistics[5].cash.text = Game.FormatCash(data.emmisionCO2)+"t";
    }

    // Update is called once per frame
    void Update()
    {
        
        Regions currentRegion = regionManager.selectedRegion;
        if (cameraManager.cameraScale >= 8f) currentRegion = Regions.None;
        UpdateStatsDisplay(currentRegion, GetRegionStats(currentRegion));
    }
}

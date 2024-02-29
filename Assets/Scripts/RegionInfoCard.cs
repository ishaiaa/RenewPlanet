using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoCard : MonoBehaviour
{
    public GameManager gameManager;
    public UIManager uiManager;
    public RegionManager regionManager;

    public bool isDisplayed = true;
    public Regions selectedRegion;
    public Text regionName;
    public Text cashTotal;
    float updateTick = 0f;

    //REGION STATS
    public RegionInfoCellStructure energyProduction;
    public RegionInfoCellStructure energyConsumption;
    public RegionInfoCellStructure energyStorage;

    public RegionInfoCellStructure population;
    public RegionInfoCellStructure area;
    public RegionInfoCellStructure emmisionCO2;

    //RESOURCES
    public RegionInfoCellStructure coalProduction;
    public RegionInfoCellStructure gasProduction;
    public RegionInfoCellStructure uraniumProduction;

    public RegionInfoCellStructure coalDemand;
    public RegionInfoCellStructure gasDemand;
    public RegionInfoCellStructure uraniumDemand;
    
    public void CloseCard()
    {
        uiManager.SelectShop((int)ShopCategory.None);
        isDisplayed = false;
    }

    public void SetDisplayed(bool state)
    {
        isDisplayed = state;
        updateTick = 0f;
        if(state)
        {
            UpdateRegionStatsDisplay();
        }
    }

    public void ToggleSelectedRegion(int offset)
    {
        int current = (int)selectedRegion;
        current += offset;
        if (current < 0) current = regionManager.regionsNames.Count-1;
        if (current > regionManager.regionsNames.Count - 1) current = 0;
        selectedRegion = (Regions)current;

        UpdateRegionStatsDisplay();

        if (selectedRegion != Regions.None)
        {
            regionName.text = "Makroregion "+regionManager.regionsNames[selectedRegion];
            return;
        }
        regionName.text = "Cały Kraj";
    }

    void UpdateRegionStatsDisplay()
    {
        RegionData data = gameManager.GetRegionData(selectedRegion);
        List<ResourceProduction[]> rPD = gameManager.GetResourceStatsInRegion(selectedRegion);
        double[] energyProdConsumpt = gameManager.GetEnergyBalanceInRegion(selectedRegion);
        cashTotal.text = "Pieniądze: "+Game.FormatCash(gameManager.cash);

        energyProduction.infoCell.UpdateText("Produkcja Energii", Game.FormatUnits(energyProdConsumpt[0]) + "W/dzień");
        energyConsumption.infoCell.UpdateText("Zużycie Energii", Game.FormatUnits(energyProdConsumpt[1]) + "W/dzień");
        energyStorage.infoCell.UpdateText("Zgromadzona Energia", Game.FormatUnits(data.energyStored));

        population.infoCell.UpdateText("Populacja Regionu", Game.FormatCash(data.population));
        area.infoCell.UpdateText("Powierzchnia Regionu", Game.FormatCash(data.area)+ "km²");
        emmisionCO2.infoCell.UpdateText("Emisja CO", Game.FormatCash(data.emmisionCO2));

        coalProduction.infoCell.UpdateText("Węgiel - Produkcja", Game.FormatCash(rPD[0][0].quantity) + "t/dzień");
        uraniumProduction.infoCell.UpdateText("Uran - Produkcja", Game.FormatCash(rPD[0][1].quantity) + "t/dzień");
        gasProduction.infoCell.UpdateText("Gaz Ziemny - Produkcja", Game.FormatCash(rPD[0][2].quantity) + "t/dzień");

        coalDemand.infoCell.UpdateText("Węgiel - Zużycie", Game.FormatCash(rPD[1][0].quantity) + "t/dzień");
        uraniumDemand.infoCell.UpdateText("Uran - Zużycie", Game.FormatCash(rPD[1][0].quantity) + "t/dzień");
        gasDemand.infoCell.UpdateText("Gaz Ziemny - Zużycie", Game.FormatCash(rPD[1][0].quantity) + "t/dzień");

    }

    void InitInfoCells()
    {
        if (selectedRegion != Regions.None)
        {
            regionName.text = "Makroregion " + regionManager.regionsNames[selectedRegion];
        }
        else
        {
            regionName.text = "Cały Kraj";
        }

        energyProduction.infoCell.UpdateAll(energyProduction.icon, "-", "-");
        energyConsumption.infoCell.UpdateAll(energyConsumption.icon, "-", "-");
        energyStorage.infoCell.UpdateAll(energyStorage.icon, "-", "-");

        population.infoCell.UpdateAll(population.icon, "-", "-");
        area.infoCell.UpdateAll(area.icon, "-", "-");
        emmisionCO2.infoCell.UpdateAll(emmisionCO2.icon, "-", "-");

        coalProduction.infoCell.UpdateAll(coalProduction.icon,"-","-");
        uraniumProduction.infoCell.UpdateAll(uraniumProduction.icon, "-", "-");
        gasProduction.infoCell.UpdateAll(gasProduction.icon, "-", "-");

        coalDemand.infoCell.UpdateAll(coalDemand.icon, "-", "-");
        uraniumDemand.infoCell.UpdateAll(uraniumDemand.icon, "-", "-");
        gasDemand.infoCell.UpdateAll(gasDemand.icon, "-", "-");
    }

    void Start()
    {
        InitInfoCells();
    }


    void Update()
    {
        if (!isDisplayed) return;
        updateTick += Time.deltaTime;
        if (updateTick < 1f) return;
        UpdateRegionStatsDisplay();
        updateTick = 0f;
    }
}

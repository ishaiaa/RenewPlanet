using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoCard : MonoBehaviour
{
    public GameManager gameManager;
    public SFXManager sfxManager;
    public UIManager uiManager;
    public RegionManager regionManager;

    public bool isDisplayed = true;
    public Regions selectedRegion;
    public Text regionName;
    float updateTick = 0f;


    //GLOBAL STATS
    public RegionInfoCellStructure money;
    public RegionInfoCellStructure daysPassed;

    public RegionInfoCellStructure kilowattPrice;
    public RegionInfoCellStructure energyDemandPerPpl;

    public RegionInfoCellStructure maxResearch;
    public RegionInfoCellStructure maxBuilds;

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

        sfxManager.PlaySound(SoundEffect.Info);

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


        money.infoCell              .UpdateText("Pieniądze", Game.FormatCash(gameManager.cash));
        daysPassed.infoCell         .UpdateText("Obecna Data", gameManager.GetUICalendar());

        kilowattPrice.infoCell      .UpdateText("Cena energii", gameManager.GetPricePerKilowatt()+"zł/kW");
        energyDemandPerPpl.infoCell .UpdateText("Zużycie energii na mieszkańca", Game.FormatUnits(gameManager.GetCurrentEnergyDemand())+"W/dzień");

        maxResearch.infoCell        .UpdateText("Limit badań", gameManager.researchFacility.GetComponent<Placeable>().objectData.efficiencyLevel + " równocześnie");
        maxBuilds.infoCell          .UpdateText("Limit prac budowlanych", gameManager.ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel + " równocześnie");

        energyProduction.infoCell.UpdateText("Produkcja Energii", Game.FormatUnits(data.energyProduction) + "W/dzień");
        energyConsumption.infoCell.UpdateText("Zużycie Energii", Game.FormatUnits(data.energyDemand) + "W/dzień");
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


        money.infoCell              .UpdateAll(money.icon, "-", "-");
        daysPassed.infoCell         .UpdateAll(daysPassed.icon, "-", "-");
                                                
        kilowattPrice.infoCell      .UpdateAll(kilowattPrice.icon, "-", "-");
        energyDemandPerPpl.infoCell .UpdateAll(energyDemandPerPpl.icon, "-", "-");
                                                
        maxResearch.infoCell        .UpdateAll(maxResearch.icon, "-", "-");
        maxBuilds.infoCell          .UpdateAll(maxBuilds.icon, "-", "-");


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
        if (updateTick < 0.25f) return;
        UpdateRegionStatsDisplay();
        updateTick = 0f;
    }
}

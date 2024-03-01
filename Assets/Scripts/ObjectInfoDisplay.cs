﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInfoDisplay : MonoBehaviour
{
    public Placeable placeable;
    public GameManager gameManager;
    public UIManager uiManager;
    public SFXManager sfxManager;

    public Sprite transparentSprite;
    
    ObjectData data;
    float updateTime = 0f;

    public Image iconMain;
    public Image iconOverlay;

    public Text title;
    public Text description;

    public Text stats;

    public Button upgradeButton;
    public Text upgradeButtonPrice;
    public Text upgradeButtonTime;

    public Button deconstructButton;
    public Text deconstructButtonPrice;
    public Text deconstructButtonTime;

    public Button moveButton;
    public Button closeButton;

    public GameObject funFactButtonObject;
    public bool showFunFact = false;
    public Sprite[] funFactButtonSprites;

    public void ToggleFunFact()
    {
        sfxManager.PlaySound(SoundEffect.Click);
        ToggleFunFact(!showFunFact);
        UpdateDisplay();
    }

    public void ToggleFunFact(bool state)
    {
        Image funFactButtonImage = funFactButtonObject.GetComponent<Image>();

        int spriteId = state ? 1 : 0;
        float opacity = state ? 1.0f : 0.4f;

        showFunFact = state;
        funFactButtonImage.sprite = funFactButtonSprites[spriteId];
        funFactButtonImage.color = new Color(1f, 1f, 1f, opacity);
    }

    public void SetDisplay()
    {
        gameManager.gameObject.GetComponent<HoverManager>().SetCursor(CursorMode.Idle);
        gameManager.gameObject.GetComponent<HoverManager>().DisplayTooltip();
        placeable = null;
        data = null;
        uiManager.SelectShop((int)ShopCategory.None);
        ToggleFunFact(false);
    }

    public void ClearDisplay()
    {
        placeable = null;
        data = null;
        ToggleFunFact(false);
    }
    public void SetDisplay(Placeable p)
    {
        if(p == null)
        {
            SetDisplay();
            return;
        }
        uiManager.SelectShop(0);
        placeable = p;
        data = p.objectData;
        uiManager.SelectShop((int)ShopCategory.ObjectInfo);
        updateTime = 0f;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (placeable == null || data == null || placeable.gameObject == null)
        {
            SetDisplay();
            return;
        }
        
        double timeLeft = data.finishTime - Game.UnixTimeStamp();
        timeLeft = timeLeft < 0 ? 0 : timeLeft;

        if(timeLeft == 0 && data.buildState == BuildState.Demolition)
        {
            SetDisplay();
            return;
        }

        description.text = data.efficiencyLevels[data.efficiencyLevel - 1].description;

        bool isUpgradeable = data.efficiencyLevel < data.maxEfficiencyLevel;
        bool notBusy = data.buildState == BuildState.Working;
        bool isAffordable = data.efficiencyLevels[data.efficiencyLevel - 1].cost <= gameManager.cash;
        bool isUnlocked = isUpgradeable && data.efficiencyLevels[data.efficiencyLevel].researchState == ResearchState.Researched;

        if (isUnlocked && isAffordable && isUpgradeable && notBusy)
        {
            upgradeButton.interactable = true;
            Hoverable hoverable = upgradeButton.gameObject.GetComponent<Hoverable>();
            hoverable.objectDescription = "";
            hoverable.showDescription = false;
            hoverable.cursorMode = CursorMode.Action;

            upgradeButtonPrice.text = Game.FormatCash(data.efficiencyLevels[data.efficiencyLevel - 1].cost);
            upgradeButtonTime.text = Game.FormatTime(data.efficiencyLevels[data.efficiencyLevel - 1].timeCost);
        }
        else
        {
            upgradeButton.interactable = false;
            Hoverable hoverable = upgradeButton.gameObject.GetComponent<Hoverable>();
            
            if(!isAffordable) hoverable.objectDescription = "Niewystarczająco pieniędzy";
            if(!isUnlocked) hoverable.objectDescription = "Ulepszenie należy odblokować poprzez badania";
            if(!notBusy) hoverable.objectDescription = "W tej chwili nad obiektem trwają prace";
            if (!isUpgradeable) hoverable.objectDescription = "Osiągnięto maksymalny poziom";

            hoverable.showDescription = true;
            hoverable.cursorMode = CursorMode.Restricted;

            upgradeButtonPrice.text = "-";
            upgradeButtonTime.text = "-";
        }

        isAffordable = data.demolitionCost <= gameManager.cash;

        if (data.deconstructable && isAffordable && notBusy)
        {
            deconstructButton.interactable = true;
            Hoverable hoverable = deconstructButton.gameObject.GetComponent<Hoverable>();
            hoverable.objectDescription = "";
            hoverable.showDescription = false;
            hoverable.cursorMode = CursorMode.Action;

            deconstructButtonPrice.text = Game.FormatCash(data.demolitionCost);
            deconstructButtonTime.text = Game.FormatTime(data.demolitionTime);
        }
        else
        {
            deconstructButton.interactable = false;
            Hoverable hoverable = deconstructButton.gameObject.GetComponent<Hoverable>();

            if (!isAffordable) hoverable.objectDescription = "Niewystarczająco pieniędzy";
            if(!notBusy) hoverable.objectDescription = "W tej chwili nad obiektem trwają prace";
            if (!data.deconstructable) hoverable.objectDescription = "Tego budynku nie można zburzyć";

            hoverable.showDescription = true;
            hoverable.cursorMode = CursorMode.Restricted;

            deconstructButtonPrice.text = "-";
            deconstructButtonTime.text = "-";
        }

        if (data.buildState == BuildState.Contstruction)
        {
            iconMain.sprite = data.stateSprites[0];
            iconOverlay.sprite = transparentSprite;

            title.text = data.efficiencyLevels[data.efficiencyLevel - 1].name + "\n" + "W BUDOWIE, " + Game.FormatTime(timeLeft);
        }
        else
        {
            iconMain.sprite = data.levelSprites[data.efficiencyLevel - 1];
            if (data.buildState == BuildState.Working)
            {
                iconOverlay.sprite = transparentSprite;
                title.text = data.efficiencyLevels[data.efficiencyLevel - 1].name + "\n" + "AKTYWNY";
            }
            else if (data.buildState == BuildState.Upgrade) {
                iconOverlay.sprite = data.stateSprites[1]; 
                title.text = data.efficiencyLevels[data.efficiencyLevel - 1].name + "\n" + "ULEPSZANIE, " + Game.FormatTime(timeLeft);
            }
            else if (data.buildState == BuildState.Demolition)
            {
                iconOverlay.sprite = data.stateSprites[1];
                title.text = data.efficiencyLevels[data.efficiencyLevel - 1].name + "\n" + "ROZBIÓRKA, " + Game.FormatTime(timeLeft);
            }
        }

        string statsText = "";

        if (showFunFact) 
        {
            statsText += "CIEKAWOSTKI:\n";
            statsText += "\n";
            statsText += data.efficiencyLevels[0].funFact + "\n";
            statsText += "\n";
            statsText += data.efficiencyLevels[1].funFact + "\n";
            statsText += "\n";
            statsText += data.efficiencyLevels[2].funFact + "\n";
        }
        else
        {
            statsText += "STATYSTYKI:\n";
        
            statsText += "\n";
            statsText += "Aktualny poziom: " + data.efficiencyLevel + "\n";
        
            if(data.isRegionLimited)
            {
                statsText += "Można zbudowac tylko jeden element tego typu w regionie.\n";
            }
            if (!data.deconstructable)
            {
                statsText += "Tego budynku nie można zburzyć.\n";
            }

            if(data.name == "Budynek Administracyjny")
            {
                statsText += "\n";
                statsText += "Cena Energii:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + gameManager.pricePerKiloWatt[0] + "zł/kWh:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + gameManager.pricePerKiloWatt[1] + "zł/kWh:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + gameManager.pricePerKiloWatt[2] + "zł/kWh:\n";

                statsText += "\n";
                statsText += "Maksymalna ilość prac budowlanych na jeden region:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: 1\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: 2\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: 3\n";
            }

            if (data.name == "Ośrodek Badawczy")
            {
                statsText += "\n";
                statsText += "Maksymalna ilość badań w tym samym czasie:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: 1\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: 2\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: 3\n";
            }

            if (data.efficiencyLevels[data.efficiencyLevel-1].energyProduction > 0)
            {
                statsText += "\n";
                statsText += "Produkcja energii:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + Game.FormatUnits(data.efficiencyLevels[0].energyProduction) + "W/h:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + Game.FormatUnits(data.efficiencyLevels[1].energyProduction) + "W/h:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + Game.FormatUnits(data.efficiencyLevels[2].energyProduction) + "W/h:\n";
            }

            if (data.efficiencyLevels[data.efficiencyLevel - 1].emmision > 0f)
            {
                statsText += "\n";
                statsText += "Emisja CO2:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + data.efficiencyLevels[0].emmision + "t/h:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + data.efficiencyLevels[1].emmision + "t/h:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + data.efficiencyLevels[2].emmision + "t/h:\n";
            }
            if (data.efficiencyLevels[data.efficiencyLevel - 1].emmision < 0f)
            {
                statsText += "\n";
                statsText += "Redukcja CO2:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + -data.efficiencyLevels[0].emmision + "t/h:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + -data.efficiencyLevels[1].emmision + "t/h:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + -data.efficiencyLevels[2].emmision + "t/h:\n";
            }

            if (data.efficiencyLevels[data.efficiencyLevel - 1].resourceConsumption.Length > 0)
            {
                ResourceProduction rP1 = data.efficiencyLevels[0].resourceConsumption[0];
                ResourceProduction rP2 = data.efficiencyLevels[1].resourceConsumption[0];
                ResourceProduction rP3 = data.efficiencyLevels[2].resourceConsumption[0];
            
            
                statsText += "\n";
                statsText += "Wykorzystywany surowiec: " + ResourceList.GetNameByType(rP1.resource);
                statsText += "\nZużycie surowca:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + rP1.quantity+"t/h:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + rP2.quantity+"t/h:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + rP3.quantity+"t/h:\n";
            }

            if (data.efficiencyLevels[data.efficiencyLevel - 1].resourceProduction.Length > 0)
            {
                ResourceProduction rP1 = data.efficiencyLevels[0].resourceProduction[0];
                ResourceProduction rP2 = data.efficiencyLevels[1].resourceProduction[0];
                ResourceProduction rP3 = data.efficiencyLevels[2].resourceProduction[0];


                statsText += "\n";
                statsText += "Produkowany surowiec: " + ResourceList.GetNameByType(rP1.resource);
                statsText += "\nProdukcja surowca:\n";
                statsText += (data.efficiencyLevel == 1 ? "● " : "○ ") + "Poziom 1: " + rP1.quantity + "t/h:\n";
                statsText += (data.efficiencyLevel == 2 ? "● " : "○ ") + "Poziom 2: " + rP2.quantity + "t/h:\n";
                statsText += (data.efficiencyLevel == 3 ? "● " : "○ ") + "Poziom 3: " + rP3.quantity + "t/h:\n";
            }
        }
        stats.text = statsText;
    }

    public void UpgradeObject()
    {
        Debug.Log("Upgrade");
        if (placeable == null || data == null) return;
        bool isUpgradeable = data.efficiencyLevel < data.maxEfficiencyLevel;
        bool isAffordable = data.efficiencyLevels[data.efficiencyLevel - 1].cost <= gameManager.cash;
        bool notBusy = data.buildState == BuildState.Working;
        bool isUnlocked = isUpgradeable && data.efficiencyLevels[data.efficiencyLevel].researchState == ResearchState.Researched;
        bool areSpacesAviable = gameManager.ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel > gameManager.GetConstructionWorkingsCount(placeable.gameObject.transform.parent.GetComponent<RegionHover>().region);

        if (!isUpgradeable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Nie można ulepszyć obiektu!", ToastMode.Error, 3f);
            return;
        }
        if (!isAffordable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Niewystarczająco pieniędzy!", ToastMode.Error, 3f);
            return;
        }
        if (!notBusy)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Nad obiektem trwają inne prace!", ToastMode.Error, 3f);
            return;
        }
        if (!isUnlocked)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Musisz odblokować to ulepszenie!", ToastMode.Error, 3f);
            return;
        }
        if (!areSpacesAviable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Osiągnięto limit aktywnych prac w regionie!", ToastMode.Error, 3f);
            return;
        }

        if (!areSpacesAviable || !isUpgradeable || !isAffordable || !isUnlocked || !notBusy) return;
        data.buildState = BuildState.Upgrade;
        data.finishTime = Game.UnixTimeStamp() + data.efficiencyLevels[data.efficiencyLevel].timeCost;
        gameManager.cash -= data.efficiencyLevels[data.efficiencyLevel].cost;
        placeable.UpdateVisuals();
        UpdateDisplay();

    }

    public void DeconstructObject()
    {
        Debug.Log("Deconstruct");
        if (placeable == null || data == null) return;
        bool isDestroyable = data.deconstructable;
        bool isAffordable = data.demolitionCost <= gameManager.cash;
        bool notBusy = data.buildState == BuildState.Working;
        bool areSpacesAviable = gameManager.ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel > gameManager.GetConstructionWorkingsCount(placeable.gameObject.transform.parent.GetComponent<RegionHover>().region);

        if (!isDestroyable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Obiektu nie mozna zburzyć!", ToastMode.Error, 3f);
            return;
        }
        if (!isAffordable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Niewystarczająco pieniędzy!", ToastMode.Error, 3f);
            return;
        }
        if (!notBusy)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Nad obiektem trwają inne prace!", ToastMode.Error, 3f);
            return;
        }
        
        if (!areSpacesAviable)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Osiągnięto limit aktywnych prac w regionie!", ToastMode.Error, 3f);
            return;
        }

        if (!isDestroyable || !isAffordable || !notBusy || !areSpacesAviable) return;
        
        data.buildState = BuildState.Demolition;
        data.finishTime = Game.UnixTimeStamp() + data.demolitionTime;
        gameManager.cash -= data.demolitionCost;
        placeable.UpdateVisuals();
        UpdateDisplay();
        sfxManager.PlaySound(SoundEffect.Click);
    }

    public void MoveObject()
    {
        if (placeable == null) return;
        if(placeable.objectData.buildState != BuildState.Working)
        {
            gameManager.toastManager.Toast(SoundEffect.Error, "Możesz przemieszczać jedynie aktywne obiekty", ToastMode.Error, 3f);
            return;
        }
        gameManager.gameObject.GetComponent<ObjectMover>().MoveObject(placeable.gameObject);
        SetDisplay();
        sfxManager.PlaySound(SoundEffect.Click);
    }

    public void CloseCard()
    {
        SetDisplay();
    }

    void Update()
    {
        if (placeable == null) return;
        updateTime += Time.deltaTime;
        if (updateTime < 1f) return;
        updateTime = 0f;
        UpdateDisplay();
    }
}

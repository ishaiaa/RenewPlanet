using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    public ObjectData objectData = new ObjectData();

    public RawImage rawImage;
    public Text titleText;
    public Text levelText;
    public Text costText;
    public Text buildTimeText;
    public GameObject lockedStateReference;
    public SFXManager sfxManager;
    
    public GameObject gameManager;

    public int displayedLevel = 1;

    public void ToggleLevel(int direction)
    {
        displayedLevel = Mathf.Clamp(displayedLevel + direction, 1, 3);
        sfxManager.PlaySound(SoundEffect.Info);
        UpdateLevelDisplay();
    } 

    public void UpdateLevelDisplay()
    {
        double cost = 0;
        double time = 0;

        for(int i = 0; i < displayedLevel; i++)
        {
            try
            {
                cost += objectData.efficiencyLevels[i].cost;
                time += objectData.efficiencyLevels[i].timeCost;
            } catch {};
        }

        rawImage.texture = objectData.levelSprites[displayedLevel-1].texture;
        levelText.text = "POZIOM " + displayedLevel;
        costText.text = Game.FormatCash(cost);
        buildTimeText.text = Game.FormatTime(time);
        lockedStateReference.SetActive(!IsUnlocked());
    }

    public bool IsUnlocked()
    {
        if (objectData.efficiencyLevels[displayedLevel-1].researchState == ResearchState.Researched) return true;
        return false;
    }

    public void TrySelectObject()
    {
        if (!IsUnlocked()) return;
        ObjectPlacer placer = gameManager.GetComponent<ObjectPlacer>();

        double cost = 0;
        double time = 0;

        for (int i = 0; i < displayedLevel; i++)
        {
            try
            {
                cost += objectData.efficiencyLevels[i].cost;
                time += objectData.efficiencyLevels[i].timeCost;
            }
            catch { };
        }

        placer.TryCreateInstance(objectData, displayedLevel, cost, time);
    }

    void Start()
    {
        titleText.text = objectData.name;
        sfxManager = gameManager.GetComponent<SFXManager>();
        UpdateLevelDisplay();
    }
}

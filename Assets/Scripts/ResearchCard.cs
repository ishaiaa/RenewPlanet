using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchCard : MonoBehaviour
{
    public Image icon;
    public Text title;
    public Text stateDescription;
    public Text price;
    public Text time;
    public Image background;
    public EfficiencyLevel efficiencyLevel;
    public Sprite[] effLevelSprites;

    public ResearchManager researchManager;
    void Start()
    {
        if(researchManager == null)
        {
            researchManager = GameObject.Find("ResearchScreen").GetComponent<ResearchManager>();
        }
    }

    public void UpdateVisuals()
    {
        string name = efficiencyLevel.name;
        ResearchState rState = efficiencyLevel.researchState;
        double rTime = efficiencyLevel.researchTime;
        double rCost = efficiencyLevel.researchCost;
        Sprite levelIcon = effLevelSprites[efficiencyLevel.level-1];

        string description = "";
        string timeText = "-";
        string costText = "-";
        Color bgColor = new Color();

        switch (rState)
        {
            case (ResearchState.Locked):
                description = "Wymagane ukoñczenie badañ nad poprzednim poziomem";
                bgColor = new Color(0.490566f, 0.490566f, 0.490566f);
                timeText = Game.FormatTime(rTime);
                costText = Game.FormatCash(rCost);
                break;
            case (ResearchState.Researchable):
                description = "Mo¿esz odblokowaæ tê technologiê";
                bgColor = new Color(0f, 0.5759377f, 1.0f);
                timeText = Game.FormatTime(rTime);
                costText = Game.FormatCash(rCost);
                break;
            case (ResearchState.Researched):
                description = "Technologia zosta³a odblokowana";
                bgColor = new Color(0.01909029f, 0.4056604f, 0f);
                break;
            case (ResearchState.Researching):
                description = "Badanie w toku...\nPozosta³o " + Game.FormatTime(efficiencyLevel.researchFinishTime - Game.UnixTimeStamp());
                bgColor = new Color(0.8773585f, 0.5228164f, 0f);
                break;
            default:
                break;
        }

        icon.sprite = levelIcon;
        title.text = name;
        stateDescription.text = description;
        time.text = timeText;
        price.text = costText;
        background.color = bgColor;
    }

    public void SelectResearch()
    {
        researchManager.SelectResearch(this);
    }
}

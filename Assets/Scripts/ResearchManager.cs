using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject scrollView;
    public GameObject researchCardTemplate;
    public ShopCard[] shopItems;
    List<ResearchCard> researchCardsReferences = new List<ResearchCard>();

    public Sprite transparentSprite;

    public Image infoIcon;
    public Text infoTitle;
    public Text infoDescription;

    public Button researchButton;
    public Text researchButtonPrice;
    public Text researchButtonTime;

    ResearchCard selectedCard;

    public void SelectResearch(ResearchCard rC)
    {
        if(rC == null)
        {
            UpdateInfo();
            return;
        }
        selectedCard = rC;
        UpdateInfo(selectedCard);
    }

    public void UpdateInfo(ResearchCard rC)
    {
        if (rC == null)
        {
            UpdateInfo();
            return;
        }
        infoIcon.sprite = rC.effLevelSprites[rC.efficiencyLevel.level - 1];
        infoTitle.text = rC.efficiencyLevel.name;
        infoDescription.text = "TO DO"; //TO DO

        Hoverable hoverable = researchButton.gameObject.GetComponent<Hoverable>();

        switch (rC.efficiencyLevel.researchState)
        {
            case (ResearchState.Locked):
                researchButton.interactable = false;
                researchButtonPrice.text = Game.FormatTime(rC.efficiencyLevel.researchTime);
                researchButtonTime.text = Game.FormatCash(rC.efficiencyLevel.researchCost);

                hoverable.cursorMode = CursorMode.Restricted;
                hoverable.showDescription = true;
                hoverable.objectDescription = "Wymagane ukoñczenie badañ nad poprzednim poziomem";
                hoverable.leftButtonInteractDisplay = false;
                break;
            case (ResearchState.Researchable):
                researchButton.interactable = true;
                researchButtonPrice.text = Game.FormatTime(rC.efficiencyLevel.researchTime);
                researchButtonTime.text = Game.FormatCash(rC.efficiencyLevel.researchCost);
                
                hoverable.cursorMode = CursorMode.Action;
                hoverable.showDescription = false;
                hoverable.objectDescription = "";
                hoverable.leftButtonInteractDisplay = true;
                break;
            case (ResearchState.Researched):
                researchButton.interactable = false;
                researchButtonPrice.text = "-";
                researchButtonTime.text = "-";

                hoverable.cursorMode = CursorMode.Restricted;
                hoverable.showDescription = true;
                hoverable.objectDescription = "Technologia zosta³a ju¿ odblokowana";
                hoverable.leftButtonInteractDisplay = false;
                break;
            case (ResearchState.Researching):
                researchButton.interactable = false;
                researchButtonPrice.text = "-";
                researchButtonTime.text = "-";

                hoverable.cursorMode = CursorMode.Restricted;
                hoverable.showDescription = true;
                hoverable.objectDescription = "Badanie w toku";
                hoverable.leftButtonInteractDisplay = false;
                break;
            default:
                break;
        }
    }

    public void UpdateInfo()
    {
        infoIcon.sprite = transparentSprite;
        infoTitle.text = "";
        infoDescription.text = "";

        researchButton.interactable = false;
        researchButtonPrice.text = "-";
        researchButtonTime.text = "-";
    }


    public void PopulateResearchList()
    {
        foreach(ShopCard shopItem in shopItems)
        {
            ObjectData data = shopItem.objectData;
            foreach(EfficiencyLevel efficiencyLevel in data.efficiencyLevels)
            {
                GameObject card = Instantiate(researchCardTemplate);
                card.gameObject.name = efficiencyLevel.name;
                card.transform.parent = scrollView.transform;

                ResearchCard rCard = card.GetComponent<ResearchCard>();
                rCard.efficiencyLevel = efficiencyLevel;
                rCard.effLevelSprites = data.levelSprites;
                rCard.researchManager = this;
                rCard.UpdateVisuals();
                researchCardsReferences.Add(rCard);
            }
        }
    }

    void Start()
    {
        PopulateResearchList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

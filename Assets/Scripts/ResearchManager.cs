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
    List<GameObject> cardsReferences = new List<GameObject>();
    List<ResearchCard> ongoingResearch = new List<ResearchCard>();

    public Sprite transparentSprite;

    public Text researchCount;

    public Image infoIcon;
    public Text infoTitle;
    public Text infoDescription;

    public Button researchButton;
    public Text researchButtonPrice;
    public Text researchButtonTime;

    public UIManager uiManager;
    ResearchCard selectedCard;

    float tickTime = 0f;

    public void CloseCard()
    {
        uiManager.SelectShop((int)ShopCategory.None);
    }

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

    public void StartResearch()
    {
        if (selectedCard == null) return;
        if (selectedCard.efficiencyLevel.researchState != ResearchState.Researchable) return;
        if (gameManager.GetResearchCap() <= ongoingResearch.Count) return;
        if (gameManager.cash < selectedCard.efficiencyLevel.researchCost) return;

        selectedCard.efficiencyLevel.researchState = ResearchState.Researching;
        selectedCard.efficiencyLevel.researchFinishTime = Game.UnixTimeStamp() + selectedCard.efficiencyLevel.researchTime;
        gameManager.cash -= selectedCard.efficiencyLevel.researchCost;
        ongoingResearch.Add(selectedCard);
        selectedCard.UpdateVisuals();
        UpdateInfo(selectedCard);
    }

    public void UpdateTopInfo()
    {
        researchCount.text = GetFinishedResearch() + " / " + researchCardsReferences.Count + " UKOÑCZONYCH\n"
            + ongoingResearch.Count + " / " + gameManager.GetResearchCap() + " W TOKU";
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

        UpdateTopInfo();

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

    public int GetFinishedResearch()
    {
        int count = 0;
        foreach(ResearchCard rCard in researchCardsReferences)
        {
            if (rCard.efficiencyLevel.researchState == ResearchState.Researched) count++;
        }
        return count;
    }

    public void UpdateInfo()
    {
        infoIcon.sprite = transparentSprite;
        infoTitle.text = "";
        infoDescription.text = "";

        researchButton.interactable = false;
        researchButtonPrice.text = "-";
        researchButtonTime.text = "-";
        researchCount.text = "";
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
                cardsReferences.Add(card);
                if (rCard.efficiencyLevel.researchState == ResearchState.Researching) ongoingResearch.Add(rCard);
            }
        }
    }

    void Start()
    {
        PopulateResearchList();
    }

    public bool TickOngoingResearch(ResearchCard rCard, int index)
    {
        bool hasFinished = false;
        if(rCard.efficiencyLevel.researchFinishTime <= Game.UnixTimeStamp())
        {
            hasFinished = true;
            rCard.efficiencyLevel.researchState = ResearchState.Researched;
            if(rCard.efficiencyLevel.level < 3)
            {
                researchCardsReferences[GetIndexInList(rCard) + 1].efficiencyLevel.researchState = ResearchState.Researchable;
                researchCardsReferences[GetIndexInList(rCard) + 1].UpdateVisuals();
            }
            UpdateCorrespondingShopCard(rCard);
            UpdateWorld(rCard);
        }
        rCard.UpdateVisuals();
        return hasFinished;
    }

    public void UpdateCorrespondingShopCard(ResearchCard rCard)
    {
        int levelIndex = GetLevelOneIndex(rCard);
        int index = (int)levelIndex / 3;
        shopItems[index].objectData.efficiencyLevels[0] = researchCardsReferences[levelIndex].efficiencyLevel;
        shopItems[index].objectData.efficiencyLevels[1] = researchCardsReferences[levelIndex+1].efficiencyLevel;
        shopItems[index].objectData.efficiencyLevels[2] = researchCardsReferences[levelIndex+2].efficiencyLevel;
    }

    public void UpdateWorld(ResearchCard rCard)
    {
        int levelIndex = GetLevelOneIndex(rCard);
        int index = (int)levelIndex / 3;

        GameObject[] objects = GameObject.FindGameObjectsWithTag(shopItems[index].objectData.name);

        foreach(GameObject gObject in objects)
        {
            Placeable placeable = gObject.GetComponent<Placeable>();
            if (placeable == null) continue;

            placeable.objectData.efficiencyLevels[0] = researchCardsReferences[levelIndex].efficiencyLevel;
            placeable.objectData.efficiencyLevels[1] = researchCardsReferences[levelIndex + 1].efficiencyLevel;
            placeable.objectData.efficiencyLevels[2] = researchCardsReferences[levelIndex + 2].efficiencyLevel;
        }
    }

    public int GetLevelOneIndex(ResearchCard rCard)
    {
        int offset = 1-rCard.efficiencyLevel.level;
        return (GetIndexInList(rCard) + offset);
    }

    public int GetIndexInList(ResearchCard rCard)
    {
        int index = 0;
        foreach(ResearchCard rC in researchCardsReferences)
        {
            if(rC.efficiencyLevel.name == rCard.efficiencyLevel.name)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    // Update is called once per frame
    void Update()
    {
        List<int> indexesToRemove = new List<int>();

        tickTime += Time.deltaTime;
        if (tickTime < 1f) return;

        int index = 0;
        foreach(ResearchCard rC in ongoingResearch)
        {
            if(TickOngoingResearch(rC, index)) indexesToRemove.Add(index);
            index++;
        }
        indexesToRemove.Reverse();
        foreach (int i in indexesToRemove)
        {
            ongoingResearch.RemoveAt(i);
        }
        UpdateTopInfo();
    }
}

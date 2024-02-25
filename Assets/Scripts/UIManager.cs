using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class ShopReference
    {
        public RectTransform bookmark;
        public RectTransform tray;

        public ShopReference(RectTransform b, RectTransform t)
        {
            bookmark = b;
            tray = t;
        }
    }

    public List<ShopReference> shopReferences;

    public class UIPositions
    {
        public Vector2 bookmarkStart;
        public Vector2 bookmarkEnd;

        public Vector2 shoptrayStart;
        public Vector2 shoptrayEnd;

        public UIPositions(Vector2 bStart, Vector2 bEnd, Vector2 sStart, Vector2 sEnd)
        {
            bookmarkStart = bStart;
            bookmarkEnd   = bEnd;
            shoptrayStart = sStart;
            shoptrayEnd = sEnd;
        }
    }

    public Dictionary<ShopCategory, UIPositions> uiElementPositions = new Dictionary<ShopCategory, UIPositions>()
    {
        {ShopCategory.Powerplants, new UIPositions(
            new Vector2(0f,  90f),    
            new Vector2(90f, 90f),    
            new Vector2(1000f, 0f),    
            new Vector2(-420f, 0f)
        )},
        {ShopCategory.Mines, new UIPositions(
            new Vector2(0f,  25f),
            new Vector2(90f, 25f),
            new Vector2(1000f, 0f),
            new Vector2(0f, 0f)
        )},
        {ShopCategory.Nature, new UIPositions(
            new Vector2(0f,  -40f),
            new Vector2(90f, -40f),
            new Vector2(1000f, 0f),
            new Vector2(0f, 0f)
        )},
        {ShopCategory.Infrastructure, new UIPositions(
            new Vector2(0f,  -105f),
            new Vector2(90f, -105f),
            new Vector2(1000f, 0f),
            new Vector2(0f, 0f)
        )}
    };

    float timeDelta = 0f;
    public float animationTime;
    public ObjectInfoDisplay objectInfoDisplay;

    public ShopCategory selectedShop = ShopCategory.None;

    void Start()
    {
        
    }

    void PositionTick(ShopCategory category, float dT)
    {
        RectTransform bookmark = shopReferences[(int)category - 1].bookmark;
        RectTransform tray = shopReferences[(int)category - 1].tray;

        if (category == selectedShop)
        {
            if((Vector2) bookmark.anchoredPosition != uiElementPositions[category].bookmarkEnd) 
                bookmark.anchoredPosition = Vector2.Lerp((Vector2)bookmark.anchoredPosition, uiElementPositions[category].bookmarkEnd, dT*2);
            if((Vector2) tray.anchoredPosition != uiElementPositions[category].shoptrayEnd)
                tray.anchoredPosition = Vector2.Lerp((Vector2)tray.anchoredPosition, uiElementPositions[category].shoptrayEnd, dT);
            return;
        }

        if ((Vector2)bookmark.anchoredPosition != uiElementPositions[category].bookmarkStart)
            bookmark.anchoredPosition = Vector2.Lerp((Vector2)bookmark.anchoredPosition, uiElementPositions[category].bookmarkStart, dT*2);

        if ((Vector2)tray.anchoredPosition != uiElementPositions[category].shoptrayStart)
            tray.anchoredPosition = Vector2.Lerp((Vector2)tray.anchoredPosition, uiElementPositions[category].shoptrayStart, dT);
    }
    
    public void SelectShop(int categoryID)
    {
        ShopCategory category = (ShopCategory)categoryID;
        if (category != ShopCategory.None) objectInfoDisplay.CloseCard();
        timeDelta = 0f;
        selectedShop = selectedShop == category ? ShopCategory.None : category;
    }

    void Update()
    {
        if(timeDelta < 1f)
        {
            timeDelta = Mathf.Clamp(timeDelta + (Time.deltaTime / animationTime), 0f, 1f);
        }

        PositionTick(ShopCategory.Powerplants, timeDelta);
        PositionTick(ShopCategory.Mines, timeDelta);
        PositionTick(ShopCategory.Nature, timeDelta);
        PositionTick(ShopCategory.Infrastructure, timeDelta);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverManager : MonoBehaviour
{
    public GameObject tooltipBox;
    public Texture2D cursorIdle;
    public Texture2D cursorAction;
    public Texture2D cursorInfo;
    public Texture2D cursorMove;
    public Texture2D cursorBuild;
    public Texture2D cursorRestricted;

    public GameObject leftButtonInteraction;
    public GameObject leftButtonPlaceObject;
    public GameObject rightButtonInformation;


    public void SetCursor(CursorMode mode)
    {
        switch(mode)
        {
            case CursorMode.Idle:
                Cursor.SetCursor(cursorIdle, new Vector2(0, 0), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.Action:
                Cursor.SetCursor(cursorAction, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.Info:
                Cursor.SetCursor(cursorInfo, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.Move:
                Cursor.SetCursor(cursorMove, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.Build:
                Cursor.SetCursor(cursorBuild, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.Restricted:
                Cursor.SetCursor(cursorRestricted, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(cursorIdle, new Vector2(0,0), UnityEngine.CursorMode.Auto);
                break;
        }

        leftButtonInteraction.SetActive(false);
        leftButtonPlaceObject.SetActive(false);
        rightButtonInformation.SetActive(false);
    }

    public void SetCursor(CursorMode mode, bool lButtonInteract, bool lButtonPlaceObj, bool rButtonInfo)
    {
        SetCursor(mode);

        leftButtonInteraction.SetActive(lButtonInteract);
        leftButtonPlaceObject.SetActive(lButtonPlaceObj);
        rightButtonInformation.SetActive(rButtonInfo);
    }

    public void DisplayTooltip()
    {
        tooltipBox.GetComponentInChildren<Text>().text = "";
        tooltipBox.SetActive(false);
    }

    public void DisplayTooltip(string text)
    {
        if(text == "" || text == null)
        {
            DisplayTooltip();
            return;
        }

        tooltipBox.SetActive(true);
        tooltipBox.GetComponentInChildren<Text>().text = text;
        Canvas.ForceUpdateCanvases();
        tooltipBox.GetComponent<HorizontalLayoutGroup>().enabled = false; // **
        tooltipBox.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    private void Start()
    {
        SetCursor(CursorMode.Idle);
        DisplayTooltip();
    }

    private void Update()
    {
        if (tooltipBox.activeSelf)
        {
            float xOffset = 80f; 
            float yOffset = 80f;
            RectTransform rT = tooltipBox.GetComponent<RectTransform>();

            Vector2 calculatedPosition = (Vector2)Input.mousePosition + new Vector2(xOffset, -rT.sizeDelta.y - yOffset);
            float fixedX = Mathf.Clamp(calculatedPosition.x, 0, 1920 - rT.sizeDelta.x);
            float fixedY = Mathf.Clamp(calculatedPosition.y, 0, 1080 - rT.sizeDelta.y);
            Vector2 fixedPosition = new Vector2(fixedX, fixedY);
            rT.anchoredPosition = fixedPosition;
        }
    }
}

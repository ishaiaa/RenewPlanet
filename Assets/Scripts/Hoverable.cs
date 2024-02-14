using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)]
    public string objectDescription;
    public bool showDescription;
    public CursorMode cursorMode;

    public bool leftButtonInteractDisplay = false;
    public bool leftButtonPlaceObjectDisplay = false;
    public bool rightButtonInfoDisplay = false;

    GameObject gameManagerObject;
    HoverManager hoverManager;

    private void Start()
    {
        gameManagerObject = GameObject.Find("GameManager") ?? null;
        if (gameManagerObject == null) return;
        hoverManager = gameManagerObject.GetComponent<HoverManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverManager.SetCursor(cursorMode, leftButtonInteractDisplay, leftButtonPlaceObjectDisplay, rightButtonInfoDisplay);
        hoverManager.DisplayTooltip(objectDescription);
        gameManagerObject.GetComponent<GameManager>().isCursorBusy = true;
    }

    public void OnMouseEnter()
    {
        hoverManager.SetCursor(cursorMode, leftButtonInteractDisplay, leftButtonPlaceObjectDisplay, rightButtonInfoDisplay);
        hoverManager.DisplayTooltip(objectDescription);
        gameManagerObject.GetComponent<GameManager>().isCursorBusy = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverManager.SetCursor(CursorMode.Idle);
        hoverManager.DisplayTooltip();
        gameManagerObject.GetComponent<GameManager>().isCursorBusy = false;
    }

    public void OnMouseExit()
    {
        hoverManager.SetCursor(CursorMode.Idle);
        hoverManager.DisplayTooltip();
        gameManagerObject.GetComponent<GameManager>().isCursorBusy = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerCollider : MonoBehaviour
{
    public Camera referenceCamera;
    public HoverManager hoverManager;
    public GameManager gameManager;
    public ObjectInfoDisplay objectInfoDisplay;
    Collider2D pointerCollider;
    bool wasHovered = false;


    void Start()
    {
        pointerCollider = gameObject.GetComponent<CircleCollider2D>();
        gameManager = hoverManager.gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGamePaused) return;

        Vector3 position = referenceCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        transform.position = new Vector3(position.x, position.y, 100f);

        if (GameManager.IsCursorBusy()) return;


        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        int count = pointerCollider.OverlapCollider(filter, results);


        if (count > 2)
        {

            foreach (Collider2D collider in results)
            {
                Placeable temp = collider.gameObject.GetComponentInParent<Placeable>();
                
                if (temp == null) continue;
                if (!temp.isPlaced) continue;
                
                //Debug.Log(collider.gameObject.transform.parent.gameObject.name);
                wasHovered = true;

                hoverManager.DisplayTooltip(temp.objectData.efficiencyLevels[temp.objectData.efficiencyLevel-1].name);
                hoverManager.SetCursor(CursorMode.Info, false, false, true);

                if(Input.GetMouseButtonDown(1) && !gameManager.isGamePaused)
                {
                    //Debug.Log("CLICKITY");
                    objectInfoDisplay.SetDisplay(temp);
                }
                return;
            }
        }
        if (wasHovered)
        {
            //Debug.Log("Not hovering anything, i guess");
            wasHovered = false;
            hoverManager.DisplayTooltip();
            hoverManager.SetCursor(CursorMode.Idle);
        }
    }
}

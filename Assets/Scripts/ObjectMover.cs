using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public GameObject objectDummyTemplate;
    public Camera referenceCamera;

    GameObject objectDummy;
    public GameObject objectToMove = null;

    public GameManager gameManager;
    public HoverManager hoverManager;
    public RegionManager regionManager;
    public SFXManager sfxManager;

    List<Placeable> intersectingObjects = new List<Placeable>();

    public void MoveObject(GameObject objToMove)
    {
        objectToMove = objToMove;
        SetOpacity(objectToMove, 0.25f);
        objectDummy = Instantiate(objectDummyTemplate);
        objectDummy.GetComponent<Placeable>().objectData = objectToMove.GetComponent<Placeable>().objectData;
        objectDummy.GetComponent<Placeable>().isPlaced = false;
        objectDummy.GetComponent<Placeable>().UpdateVisuals();
    }

    public void SetOpacity(GameObject obj, float opacity)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        opacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
        foreach(SpriteRenderer sR in renderers)
        {
            if (sR.gameObject.name == "Hitbox") continue;
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, opacity);
        }
    }

    void Update()
    {
        if (gameManager.isGamePaused)
        {
            if(objectToMove != null) SetOpacity(objectToMove, 1f);

            Destroy(objectDummy);
            objectDummy = null;
            objectToMove = null;
        }

        foreach (Placeable placeable in intersectingObjects)
        {
            placeable.ToggleColliderState(CollisionState.Static);
        }
        intersectingObjects.Clear();

        if (objectDummy == null)
        {
            return;
        }

        if (!GameManager.IsCursorBusy())
        {
            hoverManager.SetCursor(CursorMode.Move, false, true, false);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetOpacity(objectToMove, 1.0f);
                Destroy(objectDummy);
                objectDummy = null;
                objectToMove = null;
                return;
            }
        }

        Vector3 position = referenceCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        objectDummy.transform.position = new Vector3(position.x, position.y, -50f + position.y);

        Placeable objectControll = objectDummy.GetComponent<Placeable>();
        objectControll.ToggleColliderState(CollisionState.Blocked);

        if (regionManager.selectedRegion == Regions.None)
        {
            return;
        }

        Collider2D objectCollider = objectControll.collider;

        if (objectCollider == null)
        {
            return;
        }

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();
        int count = objectCollider.OverlapCollider(filter, results);

        int placeableObjectsCount = 0;

        if (count > 2)
        {

            foreach (Collider2D collider in results)
            {
                Placeable temp = collider.gameObject.GetComponentInParent<Placeable>();
                if (temp == null) continue;
                if (temp.gameObject == objectToMove) continue;
                placeableObjectsCount++;
                temp.ToggleColliderState(CollisionState.Touching);
                intersectingObjects.Add(temp);
            }
        }

        if (placeableObjectsCount > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameManager.toastManager.Toast("Nie mo¿esz umieœciæ tutaj tego obiektu!", ToastMode.Error, 5f);
            }
            return;
        }

        if (!(objectCollider.IsTouching(regionManager.regionCollider) && !objectCollider.IsTouching(regionManager.edgeCollider)))
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameManager.toastManager.Toast("Nie mo¿esz umieœciæ tutaj tego obiektu!", ToastMode.Error, 5f);
            }
            return;
        }

        objectControll.ToggleColliderState(CollisionState.Unrestricted);

        if (Input.GetMouseButtonDown(0))
        {
            if (objectToMove.transform.parent.gameObject.GetComponent<RegionHover>().region != regionManager.selectedRegion && gameManager.DoesExistInRegion(objectControll, regionManager.selectedRegion))
            {
                gameManager.toastManager.Toast("Mo¿esz umieœciæ tylko jeden taki budynek w regionie!", ToastMode.Error, 5f);
                return;
            }

            sfxManager.PlaySound(SoundEffect.Click);

            objectToMove.transform.position = objectDummy.transform.position;
            objectToMove.transform.parent = regionManager.regionCollider.gameObject.transform;
            gameManager.toastManager.Toast("Przemieszczono Obiekt", ToastMode.Success, 5f);
            SetOpacity(objectToMove, 1.0f);
            Destroy(objectDummy);
            objectDummy = null;
            objectToMove = null;
        }
    }
}

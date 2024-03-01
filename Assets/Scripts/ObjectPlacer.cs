using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject objectToPlace;
    public RegionManager regionManager;
    public SFXManager sfxManager;
    public GameManager gameManager;
    public Camera referenceCamera;
    public GameObject instantiatedObject = null;
    HoverManager hoverManager;

    List<Placeable> intersectingObjects = new List<Placeable>();

    double buildPrice = Mathf.Infinity;
    double buildTime = Mathf.Infinity;

    public void TryCreateInstance(ObjectData dataCopy, int level, double price, double time)
    {
        if (gameManager.isGamePaused) return;
        ObjectData data = new ObjectData(dataCopy);
        if (objectToPlace != null)
        {
            Destroy(instantiatedObject);
            buildPrice = Mathf.Infinity;
            buildTime = Mathf.Infinity;
        }
        data.efficiencyLevel = level;
        instantiatedObject = Instantiate(objectToPlace);
        Placeable objectConfig = instantiatedObject.GetComponent<Placeable>();
        objectConfig.objectData = data;
        objectConfig.isPlaced = false;
        objectConfig.UpdateVisuals();
        buildPrice = price;
        buildTime = time;
    }

    public void PlaceFromSave(ObjectSaveData osd)
    {
        ObjectData data = new ObjectData(gameManager.researchManager.GetObjectTemplate(osd.name));

        GameObject loadObject = Instantiate(objectToPlace);
        loadObject.transform.position = new Vector3(osd.posX, osd.posY, osd.posZ);
        loadObject.transform.parent = gameManager.GetRegion((Regions)osd.regionOrigin).gameObject.transform;

        loadObject.layer = 6;
        loadObject.name = osd.name;
        loadObject.tag = osd.name;

        Placeable objectConfig = loadObject.GetComponent<Placeable>();
        objectConfig.objectData = data;
        objectConfig.isPlaced = true;
        objectConfig.objectData.efficiencyLevel = osd.level;
        objectConfig.objectData.buildState = (BuildState)osd.buildState;
        objectConfig.objectData.finishTime = osd.buildFinishTime;
        objectConfig.ToggleColliderState(CollisionState.Static);
        objectConfig.UpdateVisuals();
    }

    void Start()
    {
        hoverManager = gameManager.gameObject.GetComponent<HoverManager>();
        //if (objectToPlace != null)
        //{
        //    instantiatedObject = Instantiate(objectToPlace);
        //}
    }

    void Update()
    {
        if (gameManager.isGamePaused)
        {
            if(instantiatedObject != null)
            {
                buildPrice = Mathf.Infinity;
                buildTime = Mathf.Infinity;
                Destroy(instantiatedObject);
                instantiatedObject = null;
            }
        }

        foreach (Placeable placeable in intersectingObjects)
        {
            placeable.ToggleColliderState(CollisionState.Static);
        }
        intersectingObjects.Clear();

        if (instantiatedObject == null)
        {
            return;
        }

        if (!GameManager.IsCursorBusy())
        {
            hoverManager.SetCursor(CursorMode.Build, false, true, false);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(instantiatedObject);
                instantiatedObject = null;
                return;
            }
        }

        Vector3 position = referenceCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        instantiatedObject.transform.position = new Vector3(position.x, position.y, -50f + position.y);

        Placeable objectControll = instantiatedObject.GetComponent<Placeable>();
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

        if(count > 2)
        {

            foreach (Collider2D collider in results)
            {
                Placeable temp = collider.gameObject.GetComponentInParent<Placeable>();
                if (temp == null) continue;
                
                placeableObjectsCount++;
                temp.ToggleColliderState(CollisionState.Touching);
                intersectingObjects.Add(temp);
            }
        }

        if(placeableObjectsCount > 0)
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
            if (gameManager.cash < buildPrice)
            {
                gameManager.toastManager.Toast("Niewystarczaj¹ca iloœæ pieniêdzy!", ToastMode.Error, 5f);
                return; //TOO LOW ON CASH
            }
            if(gameManager.DoesExistInRegion(objectControll, regionManager.selectedRegion))
            {
                gameManager.toastManager.Toast("Mo¿esz umieœciæ tylko jeden taki budynek w regionie!", ToastMode.Error, 5f);
                return;
            }
            if (gameManager.ministryFacility.GetComponent<Placeable>().objectData.efficiencyLevel <= gameManager.GetConstructionWorkingsCount(regionManager.selectedRegion))
            {
                gameManager.toastManager.Toast("Osi¹gniêto maksymaln¹ iloœæ równoczesnych prac budowlanych w tym regionie!", ToastMode.Error, 5f);
                return;
            }
            gameManager.cash -= buildPrice;

            sfxManager.PlaySound(SoundEffect.Click);

            instantiatedObject.transform.parent = regionManager.regionCollider.gameObject.transform;
            instantiatedObject.gameObject.layer = 6;
            instantiatedObject.gameObject.name = objectControll.objectData.name;
            instantiatedObject.gameObject.tag = objectControll.objectData.name;
            objectControll.isPlaced = true;
            objectControll.ToggleColliderState(CollisionState.Static);
            objectControll.objectData.finishTime = Game.UnixTimeStamp()+buildTime;
            objectControll.objectData.buildState = BuildState.Contstruction;
            objectControll.UpdateVisuals();

            gameManager.toastManager.Toast("Umieszczono Obiekt", ToastMode.Success, 5f);

            Destroy(instantiatedObject.gameObject.GetComponentInChildren<Rigidbody2D>());
            instantiatedObject = null;
            objectControll = null;
            hoverManager.SetCursor(CursorMode.Idle, false, false, false);

            buildPrice = Mathf.Infinity;
            buildTime = Mathf.Infinity;
        }
    }
}

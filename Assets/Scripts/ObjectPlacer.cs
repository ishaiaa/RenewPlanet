using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject objectToPlace;
    public RegionManager regionManager;
    public Camera referenceCamera;
    public GameObject instantiatedObject = null;

    List<Placeable> intersectingObjects = new List<Placeable>();

    void Start()
    {
        if(objectToPlace != null)
        {
            instantiatedObject = Instantiate(objectToPlace);
        }
    }

    void Update()
    {
        foreach (Placeable placeable in intersectingObjects)
        {
            placeable.ToggleColliderState(CollisionState.Static);
        }
        intersectingObjects.Clear();

        if (instantiatedObject == null)
        {
            instantiatedObject = Instantiate(objectToPlace);
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
        Debug.Log(count);

        int placeableObjectsCount = 0;

        if(count > 2)
        {
            Debug.Log("countPass");

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
            return;
        }

        if (objectCollider.IsTouching(regionManager.regionCollider) && !objectCollider.IsTouching(regionManager.edgeCollider))
        {
            objectControll.ToggleColliderState(CollisionState.Unrestricted);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            instantiatedObject.transform.parent = regionManager.regionCollider.gameObject.transform;
            instantiatedObject.gameObject.layer = 6;
            objectControll.ToggleColliderState(CollisionState.Static);


            Destroy(instantiatedObject.gameObject.GetComponentInChildren<Rigidbody2D>());
            instantiatedObject = null;
            objectControll = null;
        }
    }
}

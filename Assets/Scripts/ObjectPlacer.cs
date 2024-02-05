using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject objectToPlace;
    public RegionManager regionManager;
    public Camera referenceCamera;

    public GameObject instantiatedObject = null;

    void Start()
    {
        if(objectToPlace != null)
        {
            instantiatedObject = Instantiate(objectToPlace);
        }
    }

    void Update()
    {
        if (instantiatedObject == null)
        {
            instantiatedObject = Instantiate(objectToPlace);
        }
        
        Vector3 position = referenceCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        instantiatedObject.transform.position = new Vector3(position.x, position.y, -50f + position.y);

        if (regionManager.selectedRegion != Regions.None)
        {
            Collider2D objectCollider = instantiatedObject.GetComponentInChildren<PolygonCollider2D>();
            
            if(objectCollider != null)
            {
                instantiatedObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
                LayerMask layerMask = 6;
                ContactFilter2D filter = new ContactFilter2D().NoFilter();
                List<Collider2D> results = new List<Collider2D>();
                
                Debug.Log(objectCollider.OverlapCollider(filter, results));

                if (objectCollider.OverlapCollider(filter, results) <= 2 && objectCollider.IsTouching(regionManager.regionCollider) && !objectCollider.IsTouching(regionManager.edgeCollider))
                {
                    instantiatedObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
                    if (Input.GetMouseButtonDown(0))
                    {
                        instantiatedObject.transform.parent = regionManager.regionCollider.gameObject.transform;
                        instantiatedObject.gameObject.layer = 6;
                        Destroy(instantiatedObject.gameObject.GetComponentInChildren<Rigidbody2D>());
                        instantiatedObject = null;
                    }
                }
            }
        }
    }
}

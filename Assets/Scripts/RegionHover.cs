using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionHover : MonoBehaviour
{
    public string regionName = "default";
    public RegionManager regionManager;
    public Regions region = Regions.None;


    SpriteRenderer spriteRenderer;
    Collider2D regionCollider;
    Collider2D edgeCollider;


    private void OnMouseEnter()
    {
        Debug.Log(string.Format("RegionEnter %s", regionName));
        spriteRenderer.color = new Color(0.3764559f, 0.6698113f, 0.2495995f, 1.0f);
        regionManager.selectedRegion = region;
        regionManager.regionCollider = regionCollider;
        regionManager.edgeCollider = edgeCollider;
    }

    private void OnMouseExit()
    {
        Debug.Log(string.Format("RegionExit %s", regionName));
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        regionManager.selectedRegion = Regions.None;
        regionManager.regionCollider = null;
        regionManager.edgeCollider = null;
    }

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        regionCollider = gameObject.GetComponent<PolygonCollider2D>();
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

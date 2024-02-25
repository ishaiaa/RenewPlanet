using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraReference;
    public GameManager gameManager;
    public Camera zoomCameraReference;
    public Camera staticCameraReference;
    public GameObject clouds;
    Material cloudsMaterial;

    public static float minCameraScale = 1f;
    public static float maxCameraScale = 10f;

    public static Vector2 boundingBox = new Vector2(11f, 10f);
    public float cameraScale = 8f;

    bool isMouseDown = false;
    Vector2 mouseDragPos = Vector2.positiveInfinity;
    
    Vector2 cameraPosition = new Vector2(0f, 0f);
    Transform objectTransform;

    void Start()
    {
        cloudsMaterial = clouds.GetComponent<SpriteRenderer>().material;
        objectTransform = gameObject.transform;
        objectTransform.position = cameraPosition;

        objectTransform.localScale = new Vector3(cameraScale,cameraScale, 1f);
        cameraReference.GetComponent<Camera>().orthographicSize = cameraScale;
    }

    void Update()
    {
        if (gameManager.isCursorBusy) return;
        if (gameManager.isGamePaused) return;
        if (Input.GetMouseButtonDown(2))
        {
            isMouseDown = true;
            mouseDragPos = zoomCameraReference.ScreenToWorldPoint(Input.mousePosition);
            cameraPosition = objectTransform.position;
        }
        if(Input.GetMouseButtonUp(2))
        {
            isMouseDown = false;
            mouseDragPos = Vector2.positiveInfinity;
            cameraPosition = objectTransform.position;
        }

        if(isMouseDown)
        {
            Vector2 movementDelta = (mouseDragPos - (Vector2)zoomCameraReference.ScreenToWorldPoint(Input.mousePosition));

            Vector2 cameraVector = cameraPosition + movementDelta;
            cameraVector = new Vector2(Mathf.Clamp(cameraVector.x, -(boundingBox.x - cameraScale)-minCameraScale, (boundingBox.x - cameraScale)+minCameraScale), Mathf.Clamp(cameraVector.y, -(boundingBox.y - cameraScale)-minCameraScale, (boundingBox.y - cameraScale)+minCameraScale));
            objectTransform.position = cameraVector;
        }
        
        cloudsMaterial.SetFloat("_Transparency", Mathf.Clamp(cameraScale - (maxCameraScale - 1), 0, 1));

        if (Input.mouseScrollDelta.y != 0)
        {
            cameraScale = Mathf.Clamp(cameraScale - (Input.mouseScrollDelta.y / 3f), minCameraScale, maxCameraScale);

            Vector2 mousePosition1 = cameraReference.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            cameraReference.GetComponent<Camera>().orthographicSize = cameraScale;
            zoomCameraReference.orthographicSize = cameraScale;

            Vector2 mousePosition2 = cameraReference.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            if (!isMouseDown)
            {
                Vector2 cVector = (Vector2)objectTransform.position - (mousePosition2 - mousePosition1);
                cVector = new Vector2(Mathf.Clamp(cVector.x, -(boundingBox.x - cameraScale) - minCameraScale, (boundingBox.x - cameraScale) + minCameraScale), Mathf.Clamp(cVector.y, -(boundingBox.y - cameraScale) - minCameraScale, (boundingBox.y - cameraScale) + minCameraScale));
                objectTransform.position = cVector;
            }

        }
    }
}

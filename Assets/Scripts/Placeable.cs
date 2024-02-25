using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    
    
    public Collider2D collider;

    public SpriteRenderer objectSprite;
    public SpriteRenderer overlaySprite;
    public SpriteRenderer hitboxSprite;

    public ObjectData objectData;

    public CollisionState collisionState = CollisionState.Static;
    public bool isPlaced = false;

    float timeTick = 0f;
    public void UpdateVisuals()
    {
        switch (objectData.buildState)
        {
            case BuildState.Contstruction:
                objectSprite.sprite = objectData.stateSprites[0];
                overlaySprite.sprite = null;
                break;
            case BuildState.Working:
                objectSprite.sprite = objectData.levelSprites[objectData.efficiencyLevel-1];
                overlaySprite.sprite = null;
                break;
            case BuildState.Upgrade:
                objectSprite.sprite = objectData.levelSprites[objectData.efficiencyLevel - 1];
                overlaySprite.sprite = objectData.stateSprites[1];
                break;
            case BuildState.Demolition:
                objectSprite.sprite = objectData.levelSprites[objectData.efficiencyLevel - 1];
                overlaySprite.sprite = objectData.stateSprites[1];
                break;
            default:
                objectSprite.sprite = null;
                overlaySprite.sprite = null;
                break;
        }
    }

    public void ToggleColliderState(CollisionState state)
    {
        collisionState = state;
        switch (state)
        {
            case CollisionState.Static:
                objectSprite.color = ColorPicker.white;
                hitboxSprite.color = ColorPicker.alpha;
                break;
            case CollisionState.Unrestricted:
                objectSprite.color = ColorPicker.white;
                hitboxSprite.color = ColorPicker.Translucent(ColorPicker.green, 0.5f);
                break;
            case CollisionState.Touching:
                objectSprite.color = ColorPicker.orange;
                hitboxSprite.color = ColorPicker.Translucent(ColorPicker.orange, 0.5f);
                break;
            case CollisionState.Blocked:
                objectSprite.color = ColorPicker.red;
                hitboxSprite.color = ColorPicker.Translucent(ColorPicker.red, 0.5f);
                break;
            default:
                break;
        }
    }
}

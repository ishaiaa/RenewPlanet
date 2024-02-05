using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    
    
    public Collider2D collider;
    public SpriteRenderer objectSprite;
    public SpriteRenderer hitboxSprite;

    public CollisionState collisionState = CollisionState.Static;

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

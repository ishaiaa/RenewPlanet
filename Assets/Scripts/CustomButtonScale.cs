using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomButtonScale : CustomeButtonBase
{
    private const float OriginalScale = 1.0f;
    [SerializeField] private float toScale;
    [SerializeField] private float duration;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        transform.DOScale(toScale, duration)
            .SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        transform.DOScale(OriginalScale, duration)
            .SetEase(Ease.InOutSine);
    }
}

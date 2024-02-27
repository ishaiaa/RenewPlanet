using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    public Sprite defaultSprite;
    public Image toastIcon;
    public Text toastText;

    bool initialized = false;

    ToastManager toastManager;
    public float lifeTime = 1f;
    float fadeInOutTime = 0.25f;
    public ToastState toastState = ToastState.FadeIn;
    int toastIndex;

    public RectTransform rectTransform;

    public float internalTimer = 0f;

    public void InitializeToast(ToastManager tManager, int index, string text = "", Sprite icon = null, float duration = 1f, float fadeInOut = 0.25f)
    {
        if (icon == null) icon = defaultSprite;
        toastManager = tManager;
        toastState = ToastState.FadeIn;
        toastIcon.sprite = icon;
        toastIndex = index;
        toastText.text = text;
        lifeTime = duration;
        fadeInOutTime = fadeInOut;
        initialized = true;
    }

    void Start()
    {
        rectTransform.localScale = new Vector3(0f, 1f, 1f);
    }

    void Update()
    {
        if (!initialized) return;
        internalTimer += Time.deltaTime;
        switch(toastState)
        {
            case ToastState.FadeIn:
                rectTransform.localScale = new Vector3(Mathf.Clamp(internalTimer * (1 / fadeInOutTime),0f,1f), 1f, 1f);
                if(internalTimer >= fadeInOutTime)
                {
                    toastState = ToastState.Display;
                    internalTimer = 0f;
                }
                return;
            case ToastState.Display:
                rectTransform.localScale = new Vector3(1f, 1f, 1f);
                if (internalTimer >= lifeTime)
                {
                    toastState = ToastState.FadeOut;
                    internalTimer = 0f;
                }
                return;
            case ToastState.FadeOut:
                rectTransform.localScale = new Vector3(Mathf.Clamp(1f -(internalTimer * (1 / fadeInOutTime)),0f,1f), 1f, 1f);
                if (internalTimer >= fadeInOutTime)
                {
                    toastManager.DestroyToast(toastIndex);
                    toastState = ToastState.FadeOut;
                }
                return;
            default:
                return;
        }
    }
}

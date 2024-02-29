using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public GameObject toastTemplate;
    public GameObject toastRearranger;
    public SFXManager sfxManager;

    public List<GameObject> toasts;

    public Sprite infoIcon;
    public Sprite warningIcon;
    public Sprite errorIcon;
    public Sprite successIcon;

    int toastIndex = 0;

    Dictionary<ToastMode, Sprite> sprites;

    void Start()
    {
        sprites = new Dictionary<ToastMode, Sprite>()
        {
            {ToastMode.Info, infoIcon },
            {ToastMode.Warning, warningIcon },
            {ToastMode.Error, errorIcon },
            {ToastMode.Success, successIcon },
        };
    }

    
    public bool UpdateIfExists(string text, float duration)
    {
        foreach (GameObject gO in toasts)
        {
            Toast t = gO.GetComponent<Toast>();
            if (t.toastText.text == text && t.toastState == ToastState.Display)
            {
                t.lifeTime = duration+t.internalTimer;
                return true;
            }
        }
        return false;
    }


    public void Toast(string text, ToastMode mode = ToastMode.Info, float duration = 1f, float fadeInOutTime = 0.25f)
    {
        if (UpdateIfExists(text, duration)) return;
        toastIndex++;
        GameObject toastObject = Instantiate(toastTemplate);
        toastObject.name = "toast-" + toastIndex;
        toastObject.transform.SetParent(this.gameObject.transform, false); 

        Toast toast = toastObject.GetComponent<Toast>();

        toast.InitializeToast(this, toastIndex, text, sprites[mode], duration, fadeInOutTime);
        toasts.Add(toastObject);
    }

    public void Toast(SoundEffect soundEffect, string text, ToastMode mode = ToastMode.Info, float duration = 1f, float fadeInOutTime = 0.25f)
    {
        if (UpdateIfExists(text, duration)) return;
        toastIndex++;
        GameObject toastObject = Instantiate(toastTemplate);
        toastObject.name = "toast-" + toastIndex;
        toastObject.transform.SetParent(this.gameObject.transform, false);

        Toast toast = toastObject.GetComponent<Toast>();

        sfxManager.PlaySound(soundEffect);
        toast.InitializeToast(this, toastIndex, text, sprites[mode], duration, fadeInOutTime);

        toasts.Add(toastObject);
    }

    public void DestroyToast(int index)
    {
        int searchIndex = 0;
        foreach(GameObject gO in toasts)
        {
            if(gO.name == "toast-" + index) 
            {
                break;
            }
            searchIndex++;
        }
        if (searchIndex >= toasts.Count) return;
        Destroy(toasts[searchIndex]);
        toasts.RemoveAt(searchIndex);
    }
}

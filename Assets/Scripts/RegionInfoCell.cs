using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoCell : MonoBehaviour
{
    public Image icon;
    public Text title;
    public Text value;

    public void UpdateAll(Sprite i, string t, string v)
    {
        icon.sprite = i;
        title.text = t;
        value.text = v;
    }

    public void UpdateText(string t, string v)
    {
        title.text = t;
        value.text = v;
    }

    public void SetColor(Color c)
    {
        gameObject.GetComponent<Image>().color = c;
    }
}

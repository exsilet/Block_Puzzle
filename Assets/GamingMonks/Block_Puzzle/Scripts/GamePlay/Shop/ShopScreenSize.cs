using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScreenSize : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2 (GetComponentInParent<RectTransform>().sizeDelta.x, this.GetComponent<RectTransform>().sizeDelta.y);
    }
}

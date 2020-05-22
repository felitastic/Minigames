using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int PosInArray = 0;
    public RectTransform rectTrans;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    public void SetColor(Color newColor)
    {
        GetComponentInChildren<Image>().color = newColor;
    }    
}

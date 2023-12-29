using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellHightlight : MonoBehaviour
{
    private Material _material;
    private Color _defaultColor;

    void Awake()
    {
        _material = transform.GetComponent<Renderer>().material;
        _defaultColor = transform.GetComponent<Renderer>().material.color;
    }

    /*
    private void OnMouseEnter() {
        _material.SetColor("_Color", Color.blue);
    }

    private void OnMouseExit() {
        _material.SetColor("_Color", _defaultColor);
    }
    */

    public void PaintCell() {
        _material.SetColor("_Color", Color.green);
    }

    public void PaintCellToOriginalColor() {
        _material.SetColor("_Color", _defaultColor);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellHightlight : MonoBehaviour
{
    private Material _material;
    private Color _defaultColor;
    private Color _darkRed;

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _defaultColor = GetComponent<Renderer>().material.color;
        _darkRed = new Color(0.5f, 0, 0);
    }

    /*
    private void OnMouseEnter() {
        _material.SetColor("_Color", Color.blue);
    }

    private void OnMouseExit() {
        _material.SetColor("_Color", _defaultColor);
    }
    */

    public void PaintMoveCell() {
        _material.SetColor("_Color", Color.green);
    }

    public void PaintAttackCell() {
        _material.SetColor("_Color", Color.red);
    }

    public void PaintAttackOccupiedCell() {
        _material.SetColor("_Color", _darkRed);
    }

    public void PaintCellToOriginalColor() {
        _material.SetColor("_Color", _defaultColor);
    }

    public void DebugPaint() {
        _material.SetColor("_Color", Color.blue);
    }
}

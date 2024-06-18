using UnityEngine;

public class CellHightlight : MonoBehaviour
{
    private Material _material;
    private Color _defaultColor;
    private Color _darkRed;
    private Color _darkGreen;

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _defaultColor = GetComponent<Renderer>().material.color;
        _darkRed = new Color(0.5f, 0, 0);
        _darkGreen = new Color(0, 0.4f, 0);
    }

    public void PaintMoveCell() {
        _material.SetColor("_Color", Color.green);
    }

    public void PaintAttackCell() {
        _material.SetColor("_Color", Color.red);
    }

    public void PaintAttackOccupiedCell() {
        _material.SetColor("_Color", _darkRed);
    }

    public void PaintPositioningCell() {
        _material.SetColor("_Color", _darkGreen);
    }

    public void PaintEnchantOccupiedCell() {
        _material.SetColor("_Color", Color.blue);
    }

    public void PaintCellToOriginalColor() {
        _material.SetColor("_Color", _defaultColor);
    }
}

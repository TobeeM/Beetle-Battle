using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CellType _type;
    private bool _isOccupied;
    public CellType Type => _type;
    public bool IsOccupied => _isOccupied;

    public void Occupy() {
        _isOccupied = true;
    }

    public void Deoccupy() {
        _isOccupied = false;
    }
}

public enum CellType {
    Inner,
    Top,
    Right,
    Bottom,
    Left,
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft
}

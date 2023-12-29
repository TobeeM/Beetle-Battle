using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CellType _type;
    private int _number;

    public CellType Type => _type;
    public int Number => _number;

    void Start()
    {
        _number = int.Parse(transform.name);
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

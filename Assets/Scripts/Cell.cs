using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool _isOccupied;
    public bool IsOccupied => _isOccupied;

    public void Occupy() {
        _isOccupied = true;
    }

    public void Deoccupy() {
        _isOccupied = false;
    }
}

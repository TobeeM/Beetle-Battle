using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : MonoBehaviour
{
    private bool _hasMoved;
    private bool _hasAttacked;
    private bool _hasSkipped;

    public bool HasMoved {
        get => _hasMoved;
        set => _hasMoved = value;
    }

    public bool HasAttacked {
        get => _hasAttacked;
        set => _hasAttacked = value;
    }

    public bool HasSkipped {
        get => _hasSkipped;
        set => _hasSkipped = value;
    }

    public bool HasFinishedTurn() {
        return _hasMoved || _hasAttacked || _hasSkipped;
    }

    public void ResetUnitState() {
        _hasAttacked = false;
        _hasMoved = false;
        _hasSkipped = false;
    }
}

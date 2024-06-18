using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBase : MonoBehaviour
{
    protected Stats _stats;
    protected int _atk;
    protected List<Vector2Int> _availableSpecials;
    protected Vector2Int[] _specialPattern;
    protected SpecialType _specialType;
    protected UnitState _unitState;

    public SpecialType SpecialType => _specialType;
    public static event Action<int, TeamColor> OnUnitTakeDamage;

    public List<Vector2Int> CalculateAvailableSpecials(Cell currentCell) {
        _availableSpecials.Clear();

        Vector2Int currentPosition = new Vector2Int((int)currentCell.transform.position.x, (int)currentCell.transform.position.z);
        foreach (Vector2Int special in _specialPattern) {
            int finalX = currentPosition.x + special.x;
            int finalY = currentPosition.y + special.y;

            if (finalX >= -20 && finalX <= 20 && finalY >= -40 && finalY <= 20)
                _availableSpecials.Add(new Vector2Int(finalX, finalY));
        }

        return _availableSpecials;
    }

    public virtual bool SpecialAttackTargetedUnit(GameObject enemyUnit) {
        return default;
    }

    public virtual bool SpecialAttackNonTargetedUnit(List<GameObject> enemyUnits) { 
        return default;
    }

    protected void TriggerOnUnitTakeDamageEvent(int totalDamage, TeamColor team) {
        OnUnitTakeDamage?.Invoke(totalDamage, team);
    }
}

public enum SpecialType {
    Targeted,
    NonTargeted
}

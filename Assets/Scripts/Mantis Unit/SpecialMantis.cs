using UnityEngine;
using System.Collections.Generic;

public class SpecialMantis : SpecialBase
{
    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
        _availableSpecials = new List<Vector2Int>();
        _specialPattern = new Vector2Int[3];
        _specialType = SpecialType.NonTargeted;
        _unitState = GetComponent<UnitState>();

        _specialPattern[0] = new Vector2Int(-10, 10);
        _specialPattern[1] = new Vector2Int(0, 10);
        _specialPattern[2] = new Vector2Int(10, 10);
    }

    public override bool SpecialAttackNonTargetedUnit(List<GameObject> enemyUnits) {
        foreach (GameObject enemyUnit in enemyUnits) {
            Stats enemyStats = enemyUnit.GetComponent<Stats>();
            bool unitDodged = Random.Range(0, 101) <= enemyStats.Fortitude;
            Health enemyHealth = enemyUnit.GetComponent<Health>();
                
            if (unitDodged) {
                Debug.Log($"{enemyUnit.name} dodged the MANTIS special.");
                continue;
            }

            int totalDamage = _atk - enemyStats.Def > 0 ? _atk - enemyStats.Def : 0;
            enemyStats.Hp -= totalDamage;
            enemyHealth.CalculateHealth();
            Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} using MANTIS special for {totalDamage} damage.");
            TriggerOnUnitTakeDamageEvent(totalDamage, _stats.Team);
        }

        _unitState.HasAttacked = true;
        return true;
    }

    public void OverrideSpecialPatternBlue() {
        _specialPattern[0] = new Vector2Int(-10, -10);
        _specialPattern[1] = new Vector2Int(0, -10);
        _specialPattern[2] = new Vector2Int(10, -10);
    }
}


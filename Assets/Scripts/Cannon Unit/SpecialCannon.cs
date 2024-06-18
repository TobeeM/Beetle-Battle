using UnityEngine;
using System.Collections.Generic;

public class SpecialCannon : SpecialBase
{
    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
        _availableSpecials = new List<Vector2Int>();
        _specialPattern = new Vector2Int[12];
        _specialType = SpecialType.Targeted;
        _unitState = GetComponent<UnitState>();

        _specialPattern[0] = new Vector2Int(-40, 0);
        _specialPattern[1] = new Vector2Int(-30, 0);
        _specialPattern[2] = new Vector2Int(-20, 0);
        _specialPattern[3] = new Vector2Int(20, 0);
        _specialPattern[4] = new Vector2Int(30, 0);
        _specialPattern[5] = new Vector2Int(40, 0);
        _specialPattern[6] = new Vector2Int(0, -40);
        _specialPattern[7] = new Vector2Int(0, -30);
        _specialPattern[8] = new Vector2Int(0, -20);
        _specialPattern[9] = new Vector2Int(0, 20);
        _specialPattern[10] = new Vector2Int(0, 30);
        _specialPattern[11] = new Vector2Int(0, 40);
    }

    public override bool SpecialAttackTargetedUnit(GameObject enemyUnit) {
        Vector2Int enemyCoords = new Vector2Int((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);

        foreach (Vector2Int special in _availableSpecials) {
            if (special == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                Health enemyHealth = enemyUnit.GetComponent<Health>();
                bool unitDodged = Random.Range(0, 101) <= enemyStats.Durability;
                
                if (unitDodged) {
                    Debug.Log($"{enemyUnit.name} dodged the CANNON special.");
                    return true;
                }

                int totalDamage = _atk * 2 - enemyStats.Def > 0 ? _atk * 2 - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} using CANNON special for {totalDamage} damage.");
                _unitState.HasAttacked = true;
                TriggerOnUnitTakeDamageEvent(totalDamage, _stats.Team);
                enemyHealth.CalculateHealth();
                return true;
            }
        }

        return false;
    }
}


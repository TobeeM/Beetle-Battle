using UnityEngine;
using System.Collections.Generic;

public class SpecialKnight : SpecialBase
{
    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
        _availableSpecials = new List<Vector2Int>();
        _specialPattern = new Vector2Int[8];
        _specialType = SpecialType.Targeted;
        _unitState = GetComponent<UnitState>();
        int count = 0;

        for (int x = -10; x <= 10; x += 10) {
            for (int y = -10; y <= 10; y += 10) {
                if (x == 0 && y == 0) continue;
                _specialPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }
    }

    public override bool SpecialAttackTargetedUnit(GameObject enemyUnit) {
        Vector2Int enemyCoords = new Vector2Int((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);

        foreach (Vector2Int special in _availableSpecials) {
            if (special == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                Health enemyHealth = enemyUnit.GetComponent<Health>();
                bool unitDodged = Random.Range(0, 101) <= enemyStats.Fortitude;
                
                if (unitDodged) {
                    Debug.Log($"{enemyUnit.name} dodged the KNIGHT SPECIAL.");
                    return true;
                }

                int totalDamage = _atk * 2 - enemyStats.Def > 0 ? _atk * 2 - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                enemyHealth.CalculateHealth();
                Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} using KNIGHT special for {totalDamage} damage.");
                Debug.Log(_unitState);
                _unitState.HasAttacked = true;
                TriggerOnUnitTakeDamageEvent(totalDamage, _stats.Team);
                return true;
            }
        }

        return false;
    }
}

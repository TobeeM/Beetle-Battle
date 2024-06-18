using System.Collections.Generic;
using UnityEngine;

public class SpecialBee : SpecialBase
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
                bool unitDodged = Random.Range(0, 101) <= enemyStats.Immunity;
                
                if (unitDodged) {
                    Debug.Log($"{enemyUnit.name} dodged the BEE special.");
                    return true;
                }

                UnitStatus enemyStatus = enemyUnit.GetComponent<UnitStatus>();
                enemyStatus.ApplyStatus(StatusType.Poisoned, 3, 50);
                Debug.Log($"{enemyUnit.transform.name} has been poisoned by {transform.name}.");
                _unitState.HasAttacked = true;
                return true;
            }
        }

        return false;
    }
}

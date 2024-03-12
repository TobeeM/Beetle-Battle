using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    private Stats _stats;
    private int _atk;
    private List<Vector2Int> _availableAttacks;
    private Vector2Int[] _attackPattern;

    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
        _availableAttacks = new List<Vector2Int>();
        _attackPattern = new Vector2Int[8];
        int count = 0;

        for (int x = -10; x <= 10; x += 10) {
            for (int y = -10; y <= 10; y += 10) {
                if (x == 0 && y == 0) continue;
                _attackPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }
    }

    public List<Vector2Int> CalculateAvailableAttacks(Cell currentCell) {
        _availableAttacks.Clear();

        Vector2Int currentPosition = new Vector2Int((int)currentCell.transform.position.x, (int)currentCell.transform.position.z);
        foreach (Vector2Int attack in _attackPattern) {
            int finalX = currentPosition.x + attack.x;
            int finalY = currentPosition.y + attack.y;

            if (finalX >= -20 && finalX <= 20 && finalY >= -40 && finalY <= 20)
                _availableAttacks.Add(new Vector2Int(finalX, finalY));
        }

        return _availableAttacks;
    }

    public virtual bool AttackUnit(GameObject enemyUnit) {
        Vector2Int enemyCoords = new Vector2Int((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);

        foreach (Vector2Int attack in _availableAttacks) {
            if (attack == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                bool unitDodged = Random.Range(0, 101) > enemyStats.Dodge ? false : true;
                
                if (unitDodged) {
                    Debug.Log($"{enemyUnit.name} dodged the attack.");
                    return true;
                }

                int totalDamage = _atk - enemyStats.Def > 0 ? _atk - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} for {totalDamage} damage.");
                return true;
            }
        }

        return false;
    }
}

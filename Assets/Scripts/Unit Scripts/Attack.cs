using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Stats _stats;
    private int _atk;
    private List<int> _availableAttacks = new List<int>();

    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
    }

    public List<int> CalculateAvailableAttacks(Cell currentCell) {
        _availableAttacks.Clear();
        switch (currentCell.Type) {
            case CellType.Inner:
                _availableAttacks.AddRange(new List<int> {-6, -5, -4, -1, 1, 4, 5, 6});
                break;
            case CellType.Left:            
                _availableAttacks.AddRange(new List<int> {-5, -4, 1, 5, 6});
                break;
            case CellType.Right:
                _availableAttacks.AddRange(new List<int> {-6, -5, -1, 4, 5});
                break;
            case CellType.Top:
                _availableAttacks.AddRange(new List<int> {-1, 1, -6, -5, -4});
                break;
            case CellType.Bottom:
                _availableAttacks.AddRange(new List<int> {4, 5, 6, -1, 1});
                break;
            case CellType.TopLeft:
                _availableAttacks.AddRange(new List<int> {1, -5, -4});
                break;
            case CellType.TopRight:
                _availableAttacks.AddRange(new List<int> {-1, -6, -5});
                break;
            case CellType.BottomLeft:
                _availableAttacks.AddRange(new List<int> {1, 6, 5});
                break;
            case CellType.BottomRight:
                _availableAttacks.AddRange(new List<int> {-1, 4, 5});
                break;
        }
        return _availableAttacks;
    }
    
    public bool AttackUnit(GameObject enemyUnit) {
        int enemyCoords = CoordinateConverter.Convert((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);
        int unitCoords = CoordinateConverter.Convert((int)transform.position.x, (int)transform.position.z);

        foreach (int attack in _availableAttacks) {
            int finalAttack = unitCoords + attack;
            if (finalAttack == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                int totalDamage = _atk - enemyStats.Def > 0 ? _atk - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} for {totalDamage} damage.");
                return true;
            }
        }

        return false;
    }

    public void InstaKill(GameObject enemyUnit) {
        enemyUnit.GetComponent<Stats>().Hp = 0;
    }
}

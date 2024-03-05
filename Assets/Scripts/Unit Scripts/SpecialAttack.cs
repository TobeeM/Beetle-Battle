using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    private Stats _stats;
    private int _atk;
    private List<int> _availableSpecials = new List<int>();

    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
    }

    public List<int> CalculateAvailableSpecials(Cell currentCell) {
        _availableSpecials.Clear();
        switch (currentCell.Type) {
            case CellType.Inner:
                _availableSpecials.AddRange(new List<int> {-6, -5, -4, -1, 1, 4, 5, 6});
                break;
            case CellType.Left:            
                _availableSpecials.AddRange(new List<int> {-5, -4, 1, 5, 6});
                break;
            case CellType.Right:
                _availableSpecials.AddRange(new List<int> {-6, -5, -1, 4, 5});
                break;
            case CellType.Top:
                _availableSpecials.AddRange(new List<int> {-1, 1, -6, -5, -4});
                break;
            case CellType.Bottom:
                _availableSpecials.AddRange(new List<int> {4, 5, 6, -1, 1});
                break;
            case CellType.TopLeft:
                _availableSpecials.AddRange(new List<int> {1, -5, -4});
                break;
            case CellType.TopRight:
                _availableSpecials.AddRange(new List<int> {-1, -6, -5});
                break;
            case CellType.BottomLeft:
                _availableSpecials.AddRange(new List<int> {1, 6, 5});
                break;
            case CellType.BottomRight:
                _availableSpecials.AddRange(new List<int> {-1, 4, 5});
                break;
        }
        return _availableSpecials;
    }
    
    public bool SpecialAttackUnit(GameObject enemyUnit) {
        int enemyCoords = CoordinateConverter.Convert((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);
        int unitCoords = CoordinateConverter.Convert((int)transform.position.x, (int)transform.position.z);

        foreach (int attack in _availableSpecials) {
            int finalAttack = unitCoords + attack;
            if (finalAttack == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                int totalDamage = _atk * 2 - enemyStats.Def > 0 ? _atk * 2 - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                Debug.Log($"{transform.name} attacked using [DEBUG SPECIAL ABILITY NAME] {enemyUnit.transform.name} for {totalDamage} damage.");
                return true;
            }
        }

        return false;
    }
}

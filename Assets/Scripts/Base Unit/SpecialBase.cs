using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBase : MonoBehaviour
{
    protected Stats _stats;
    protected int _atk;
    protected List<Vector2Int> _availableSpecials;
    protected Vector2Int[] _specialPattern;

    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
        _availableSpecials = new List<Vector2Int>();
        _specialPattern = new Vector2Int[8];
        int count = 0;

        for (int x = -10; x <= 10; x += 10) {
            for (int y = -10; y <= 10; y += 10) {
                if (x == 0 && y == 0) continue;
                _specialPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }
    }

    public virtual List<Vector2Int> CalculateAvailableSpecials(Cell currentCell) {
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

    public virtual bool SpecialAttackUnit(GameObject enemyUnit) {
        Vector2Int enemyCoords = new Vector2Int((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);

        foreach (Vector2Int special in _availableSpecials) {
            if (special == enemyCoords) {
                Stats enemyStats = enemyUnit.GetComponent<Stats>();
                int totalDamage = _atk * 2 - enemyStats.Def > 0 ? _atk * 2 - enemyStats.Def : 0;
                enemyStats.Hp -= totalDamage;
                Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} using SPECIAL ABILITY for {totalDamage} damage.");
                return true;
            }
        }

        return false;
    }
}

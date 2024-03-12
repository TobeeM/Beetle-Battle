using UnityEngine;

public class SpecialBee : SpecialBase
{
    public override bool SpecialAttackUnit(GameObject enemyUnit) {
        Vector2Int enemyCoords = new Vector2Int((int)enemyUnit.transform.position.x, (int)enemyUnit.transform.position.z);

        foreach (Vector2Int special in _availableSpecials) {
            if (special == enemyCoords) {
                UnitStatus enemyStatus = enemyUnit.GetComponent<UnitStatus>();
                
                enemyStatus.ApplyStatus(StatusType.Poisoned, 3, 50);
                Debug.Log($"{enemyUnit.transform.name} has been poisoned by {transform.name}.");
                return true;
            }
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    private Stats _stats;
    private int _atk;

    void Awake() {
        _stats = GetComponent<Stats>();
        _atk = _stats.Atk;
    }

    public void AttackUnit(GameObject enemyUnit) {
        Stats enemyStats = enemyUnit.GetComponent<Stats>();
        int totalDamage = _atk - enemyStats.Def > 0 ? _atk - enemyStats.Def : 0;
        
        enemyStats.Hp -= totalDamage;

        Debug.Log($"{transform.name} attacked {enemyUnit.transform.name} for {totalDamage} damage.");
    }

    public void InstaKill(GameObject enemyUnit) {
        enemyUnit.GetComponent<Stats>().Hp = 0;
    }
}

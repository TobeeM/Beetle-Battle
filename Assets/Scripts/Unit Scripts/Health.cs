using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Stats _stats;
    public static event Action<GameObject, TeamColor> OnUnitDied;

    void Awake() {
        _stats = GetComponent<Stats>();
    }

    public void CalculateHealth() {
        if (_stats.Hp <= 0) {
            _stats.Hp = 0;
            Debug.Log($"{gameObject.name} has died!");
            OnUnitDied?.Invoke(gameObject, GetComponent<Stats>().Team);
        }
    }
}

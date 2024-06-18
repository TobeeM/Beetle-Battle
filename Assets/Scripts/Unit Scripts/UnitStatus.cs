using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    private List<StatusType> _currentStatuses;
    private List<int> _statusDurations;
    private List<int> _statusesDamage;

    void Awake() {
        _currentStatuses = new List<StatusType>();
        _statusDurations = new List<int>();
        _statusesDamage = new List<int>();
    }

    public void ApplyStatus(StatusType status, int statusDuration, int statusDamage) {
        foreach (StatusType statusType in _currentStatuses) {
            if (statusType == StatusType.Poisoned || statusType == StatusType.Dazed || statusType == StatusType.Immobilized) return;
        }

        _currentStatuses.Add(status);
        _statusDurations.Add(statusDuration);
        _statusesDamage.Add(statusDamage);
    }

    private void DecreaseStatusDuration() {
        if (_statusDurations.Count == 0) return;

        if (_currentStatuses.Count > 0) {
            for (int i = 0; i < _currentStatuses.Count; i++) Debug.Log($"{transform.name} is {_currentStatuses[i]} for {_statusDurations[i]} more turn(s).");
        }

        for (int i = 0; i < _statusDurations.Count; i++) {
            if (_currentStatuses[i] == StatusType.Poisoned) {
                GetComponent<Stats>().Hp -= _statusesDamage[i];
                GetComponent<Health>().CalculateHealth();
            }

            _statusDurations[i]--;

            if (_statusDurations[i] == 0) {
                _currentStatuses.RemoveAt(i);
                _statusDurations.RemoveAt(i);
                _statusesDamage.RemoveAt(i);
            }     
        }
    }

    void OnEnable() {
        BoardManager.OnTurnOver += DecreaseStatusDuration;
    }

    void OnDisable() {
        BoardManager.OnTurnOver -= DecreaseStatusDuration;
    }
}

public enum StatusType {
    Poisoned,
    Dazed,
    Immobilized
}

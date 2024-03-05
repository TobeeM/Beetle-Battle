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
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;
    private Stats _stats;

    void Awake() {
        _stats = GetComponent<Stats>();
        _slider.maxValue = _stats.Hp;
        _slider.value = _stats.Hp;
        _text.text = $"{_stats.Hp} / {_slider.maxValue}";
    }

    void Start() {
        CalculateHealth(new List<GameObject>());
    }

    public void CalculateHealth(List<GameObject> units) {
        _slider.value = _stats.Hp;
        _text.text = $"{_stats.Hp} / {_slider.maxValue}";

        if (_stats.Hp <= 0) {
            units.Remove(gameObject);
            Destroy(gameObject, 0.5f);
        }
    }
}

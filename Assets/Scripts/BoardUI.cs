using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _redTMP;
    [SerializeField] private TextMeshProUGUI _blueTMP;
    [SerializeField] private TextMeshProUGUI _turnCountTMP;

    private BoardManager _bm;
    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;

    void Start() {
        _bm = GetComponent<BoardManager>();
        _redUnits = _bm.RedUnits;
        _blueUnits = _bm.BlueUnits;

        _redTMP.text = $"Red units: {_bm.RedUnits.Count}\nTotal health: {500}";
        _blueTMP.text = $"Blue units: {_bm.BlueUnits.Count}\nTotal health: {500}";
        _turnCountTMP.text = $"Current Turn: {_bm.TurnCount}";
    }

    public void UpdateUnitInfo() {
        int redTotalHealth = 0;
        int blueTotalHealth = 0;

        foreach (var unit in _redUnits) redTotalHealth += unit.GetComponent<Stats>().Hp;
        foreach (var unit in _blueUnits) blueTotalHealth += unit.GetComponent<Stats>().Hp;

        _redTMP.text = $"Red units: {_bm.RedUnits.Count}\nTotal health: {redTotalHealth}";
        _blueTMP.text = $"Blue units: {_bm.BlueUnits.Count}\nTotal health: {blueTotalHealth}";
    }

    public void UpdateTurnCount() {
        _turnCountTMP.text = $"Current Turn: {_bm.TurnCount}";
        Debug.Log($"Current Turn is {_bm.TurnCount}");
    }
}

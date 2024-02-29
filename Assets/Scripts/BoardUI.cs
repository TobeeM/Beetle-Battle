using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _redTMP;
    [SerializeField] private TextMeshProUGUI _blueTMP;
    [SerializeField] private TextMeshProUGUI _turnCountTMP;
    [SerializeField] private TextMeshProUGUI _gameStateTMP;
    [SerializeField] private GameObject _redHealthbars;
    [SerializeField] private GameObject _blueHealthbars;

    private BoardManager _bm;
    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;
    private List<UnityEngine.UI.Image> _redHealthbarsImage = new List<UnityEngine.UI.Image>();
    private List<UnityEngine.UI.Image> _blueHealthbarsImage = new List<UnityEngine.UI.Image>();

    void Start() {
        _bm = GetComponent<BoardManager>();
        _redUnits = _bm.RedUnits;
        _blueUnits = _bm.BlueUnits;
        
        for (int i = 0; i < 5; i++) {
            _redHealthbarsImage.Add(_redHealthbars.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>());
            _blueHealthbarsImage.Add(_blueHealthbars.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>());
        }

        _turnCountTMP.text = $"Current Turn: {_bm.TurnCount}";
    }

    public void UpdateUnitInfo() {
        for (int i = 0; i < _redUnits.Count; i++) {
            float unitMaxHP = _redUnits[i].GetComponent<Stats>().MaxHP;
            float unitHealth = _redUnits[i].GetComponent<Stats>().Hp;
            float currentHP = unitHealth / unitMaxHP;

            _redHealthbarsImage[i].fillAmount = currentHP;
        }

        for (int i = 0; i < _blueUnits.Count; i++) {
            float unitMaxHP = _blueUnits[i].GetComponent<Stats>().MaxHP;
            float unitHealth = _blueUnits[i].GetComponent<Stats>().Hp;
            float currentHP = unitHealth / unitMaxHP;

            _blueHealthbarsImage[i].fillAmount = currentHP;
        }
    }

    public void UpdateTurnCount() {
        _turnCountTMP.text = $"Current Turn: {_bm.TurnCount}";
        if (_bm.IsYourTurn) {
            _gameStateTMP.color = Color.red;
            _gameStateTMP.text = "Your turn";
        }
        else {
            _gameStateTMP.color = Color.blue;
            _gameStateTMP.text = "Enemy turn";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _redTMP;
    [SerializeField] private TextMeshProUGUI _blueTMP;
    [SerializeField] private TextMeshProUGUI _turnCountTMP;
    [SerializeField] private TextMeshProUGUI _gameStateTMP;
    [SerializeField] private GameObject _redHealthbarsObject;
    [SerializeField] private GameObject _blueHealthbarsObject;
    [SerializeField] private Slider _currentUnitHPBar;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _positioningPanel;

    private BoardManager _bm;
    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;
    private List<GameObject> _redHealthbars = new List<GameObject>();
    private List<GameObject> _blueHealthbars = new List<GameObject>();
    private List<Image> _redHealthbarsImage = new List<Image>();
    private List<Image> _blueHealthbarsImage = new List<Image>();
    private Image _currentUnitBarForeground;
    private TextMeshProUGUI _currentUnitTextBar;

    void Start() {
        _bm = GetComponent<BoardManager>();
        _redUnits = _bm.RedUnits;
        _blueUnits = _bm.BlueUnits;
        _currentUnitBarForeground = _currentUnitHPBar.fillRect.GetComponent<Image>();
        _currentUnitTextBar = _currentUnitHPBar.GetComponentInChildren<TextMeshProUGUI>();
 
        for (int i = 0; i < 5; i++) {
            _redHealthbars.Add(_redHealthbarsObject.transform.GetChild(i).gameObject);
            _redHealthbarsImage.Add(_redHealthbars[i].GetComponent<Image>());

            _blueHealthbars.Add(_blueHealthbarsObject.transform.GetChild(i).gameObject);
            _blueHealthbarsImage.Add(_blueHealthbars[i].GetComponent<Image>());
        }

        _turnCountTMP.text = $"Current Turn: {_bm.TurnCount}";
        ShowPositioningPanel(TeamColor.Red);
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

        if (_bm.CurrentTeam == TeamColor.Red) {
            _gameStateTMP.color = Color.red;
            _gameStateTMP.text = "Your turn";
        }
        else {
            _gameStateTMP.color = Color.blue;
            _gameStateTMP.text = "Enemy turn";
        }
    }

    private void UpdateCurrentUnitBar(Stats unitStats) {
        _currentUnitBarForeground.color = unitStats.Team == TeamColor.Red ? Color.red : Color.blue;
        _currentUnitHPBar.maxValue = unitStats.MaxHP;
        _currentUnitHPBar.value = unitStats.Hp;
        _currentUnitTextBar.text = $"{unitStats.Hp} / {unitStats.MaxHP}";
    }

    public void ShowGameOverPanel(TeamColor winningTeam) {
        var statsPanel = _gameOverPanel.transform.GetChild(2);
        TextMeshProUGUI[] combatStats = new TextMeshProUGUI[5] {
            statsPanel.GetChild(1).GetComponent<TextMeshProUGUI>(), // Damage Done by Red
            statsPanel.GetChild(2).GetComponent<TextMeshProUGUI>(), // Red Units Killed
            statsPanel.GetChild(3).GetComponent<TextMeshProUGUI>(), // Damage Done by Blue
            statsPanel.GetChild(4).GetComponent<TextMeshProUGUI>(), // Blue Units Killed
            statsPanel.GetChild(5).GetComponent<TextMeshProUGUI>()  // Total Turns
        };

        combatStats[0].text = $"Damage Done by Red: {_bm.RedTotalDamage}";
        combatStats[1].text = $"Red Units Killed: {5 - _bm.RedUnits.Count}";
        combatStats[2].text = $"Damage Done by Blue: {_bm.BlueTotalDamage}";
        combatStats[3].text = $"Blue Units Killed: {5 - _bm.BlueUnits.Count}";
        combatStats[4].text = $"Total Turns: {_bm.TurnCount}";

        if (winningTeam == TeamColor.Red) _gameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
        else _gameOverPanel.transform.GetChild(1).gameObject.SetActive(true);
        _gameOverPanel.SetActive(true);
    }

    public void ShowPositioningPanel(TeamColor team) {
        Transform[] unitStrings = new Transform[5];
        Stats unitStats;
        TextMeshProUGUI choosingTeam = _positioningPanel.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        if (team == TeamColor.Blue) {
            choosingTeam.text = "BLUE TEAM IS CHOOSING . . .";
            choosingTeam.color = Color.blue;
        }

        for (int i = 0; i < 5; i++) unitStrings[i] = _positioningPanel.transform.GetChild(i);
        for (int i = 0; i < 5; i++) {
            if (team == TeamColor.Red) unitStats = _redUnits[i].GetComponent<Stats>();
            else unitStats = _blueUnits[i].GetComponent<Stats>();

            TextMeshProUGUI unitType = unitStrings[i].GetChild(0).GetComponent<TextMeshProUGUI>();
            unitType.text = unitStats.Type.ToString();
            if (unitStats.IsKing) unitType.color = Color.yellow;
            else unitType.color = Color.white;
            unitStrings[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = $"HP: {unitStats.MaxHP}";
            unitStrings[i].GetChild(2).GetComponent<TextMeshProUGUI>().text = $"DGE: {unitStats.Dodge}";
            unitStrings[i].GetChild(3).GetComponent<TextMeshProUGUI>().text = $"ATK: {unitStats.Atk}";
            unitStrings[i].GetChild(4).GetComponent<TextMeshProUGUI>().text = $"DEF: {unitStats.Def}";
            unitStrings[i].GetChild(5).GetComponent<TextMeshProUGUI>().text = unitStats.Immunity.ToString();
            unitStrings[i].GetChild(6).GetComponent<TextMeshProUGUI>().text = unitStats.Fortitude.ToString();
            unitStrings[i].GetChild(7).GetComponent<TextMeshProUGUI>().text = unitStats.Durability.ToString();
            unitStrings[i].GetChild(8).GetComponent<TextMeshProUGUI>().text = unitStats.Clarity.ToString();
        }
    }

    void OnEnable() {
        BoardManager.OnTurnOver += UpdateTurnCount;
        BoardManager.OnCurrentUnitChange += UpdateCurrentUnitBar;
        BoardManager.OnGameOver += ShowGameOverPanel;
        BoardManager.OnUnitPositioning += ShowPositioningPanel;
    }

    void OnDisable() {
        BoardManager.OnTurnOver -= UpdateTurnCount;
        BoardManager.OnCurrentUnitChange -= UpdateCurrentUnitBar;
        BoardManager.OnGameOver -= ShowGameOverPanel;
        BoardManager.OnUnitPositioning -= ShowPositioningPanel;
    }
}

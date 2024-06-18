using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    private GameObject[] _board;
    private GameObject[] _boardHighlight;
    private Cell[] _cells;
    private CellHightlight[] _ch;

    private GameObject _currentUnit;
    private GameObject _clickedEnemyUnit;
    private GameObject _clickedObject;
    private List<Vector2Int> _availableMoves;
    private List<Vector2Int> _availableAttacks;
    private bool _isUnitMoving;
    private int _turnCount;
    private Camera _boardCamera;
    private bool _isAttackInitiated;
    private TeamColor _currentTeam;
    private int _redTotalDamage;
    private int _blueTotalDamage;

    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;
    private List<UnitState> _redUnitsState;
    private List<UnitState> _blueUnitsState;
    private GameObject _blueKing;
    private GameObject _redKing;

    public List<GameObject> RedUnits => _redUnits;
    public List<GameObject> BlueUnits => _blueUnits;
    public int TurnCount => _turnCount;
    public TeamColor CurrentTeam => _currentTeam;
    public int RedTotalDamage => _redTotalDamage;
    public int BlueTotalDamage => _blueTotalDamage;

    public static event Action OnTurnOver;
    public static event Action<Stats> OnCurrentUnitChange;
    public static event Action<TeamColor> OnGameOver;
    public static event Action<TeamColor> OnUnitPositioning;

    [SerializeField] private GameObject[] _uiObjects;
    [SerializeField] private GameObject[] _unitLines;
    [SerializeField] private GameObject _positioningPanel;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent _onUnitMoved;
    [SerializeField] private UnityEvent _onUnitAttacked;

    [Header("Buttons")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _skipTurnButton;
    [SerializeField] private Button _specialAttackButton;

    void Awake() {
        _boardCamera = Camera.main;
        _availableMoves = new List<Vector2Int>();
        _turnCount = 1;
        _board = GameObject.FindGameObjectsWithTag("cell").OrderBy(cell => int.Parse(cell.name)).ToArray();
        _boardHighlight = GameObject.FindGameObjectsWithTag("cellHighlight").OrderBy(cell => int.Parse(cell.name)).ToArray();
        _cells = new Cell[_board.Length];
        _ch = new CellHightlight[_board.Length];
        _currentTeam = TeamColor.Red;

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = _board[i].GetComponent<Cell>();
            _ch[i] = _boardHighlight[i].GetComponent<CellHightlight>();
        }
    }

    void Start() {
        _redUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Stats>().Team == TeamColor.Red).ToList();
        _blueUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Stats>().Team == TeamColor.Blue).ToList();
        _redUnitsState = new List<UnitState>();
        _blueUnitsState = new List<UnitState>();
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        _specialAttackButton.interactable = false;

        foreach (GameObject unit in _redUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            if (unit.GetComponent<Stats>().IsKing) _redKing = unit;
            _redUnitsState.Add(unit.GetComponent<UnitState>());
        }

        foreach (GameObject unit in _blueUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            if (unit.GetComponent<Stats>().IsKing) _blueKing = unit;
            _blueUnitsState.Add(unit.GetComponent<UnitState>());
        }

        InitiatePositioningPhase();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _clickedObject = GetClickedObject();

            if (_clickedObject) Debug.Log($"You clicked {_clickedObject.name}");

            if (_clickedObject && _clickedObject.CompareTag("unit")) OnCurrentUnitChange?.Invoke(_clickedObject.GetComponent<Stats>());

            // Only set black color to the current unit.
            if (_currentUnit && _clickedObject != _currentUnit) _currentUnit.GetComponent<UnitColor>().SetColorToOriginal();

            // If clicked object is a unit, assign _currentUnit variable to it.
            if (_clickedObject && !_clickedObject.CompareTag("cell") && _clickedObject.GetComponent<Stats>().Team == _currentTeam && !_isAttackInitiated) {
                UnhighlightArea();
                _currentUnit = _clickedObject;

                if (!_currentUnit.GetComponent<UnitState>().HasFinishedTurn()) {
                    _currentUnit.GetComponent<UnitColor>().SetColorToCurrentUnit();
                    _attackButton.interactable = true;
                    _skipTurnButton.interactable = true;
                    _specialAttackButton.interactable = true;
                    if (!_currentUnit.GetComponent<UnitState>().HasMoved) HighlightArea();
                }
                else {
                    _currentUnit = null;
                }
            }

            // Current Unit movement.
            if (_clickedObject && _clickedObject.CompareTag("cell") && _currentUnit && !_isAttackInitiated && !_currentUnit.GetComponent<UnitState>().HasMoved) {
                Vector3 moveLocation = new Vector3(_clickedObject.transform.position.x, 3, _clickedObject.transform.position.z); 
                UnhighlightArea();

                int x = (int)_currentUnit.transform.position.x;
                int z = (int)_currentUnit.transform.position.z;
                int currentCoords = CoordinateConverter.Convert(x, z);

                bool unitMoved = _currentUnit.GetComponent<MovementBase>().Move(
                    moveLocation, _cells[CoordinateConverter.Convert((int)moveLocation.x, (int)moveLocation.z)]);

                if (unitMoved) {
                    _cells[currentCoords].Deoccupy();
                    x = (int)_currentUnit.transform.position.x;
                    z = (int)_currentUnit.transform.position.z;
                    currentCoords = CoordinateConverter.Convert(x, z);
                    _cells[currentCoords].Occupy(_currentUnit);
                    _currentUnit.GetComponent<UnitState>().HasMoved = true;
                    _onUnitMoved.Invoke();       
                }

                _currentUnit = null;
                _attackButton.interactable = false;
                _skipTurnButton.interactable = false;
                _specialAttackButton.interactable = false;
                DetermineTurnCompletion();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _currentTeam = TeamColor.Red;
            _turnCount++;
            OnTurnOver?.Invoke();
            ResetUnitsStates(TeamColor.Red);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            _currentTeam = TeamColor.Blue;
            _turnCount++;
            OnTurnOver?.Invoke();
            ResetUnitsStates(TeamColor.Blue);
        }

        if (Input.GetKeyDown(KeyCode.E)) GetComponent<BoardUI>().ShowGameOverPanel(TeamColor.Red);
    }

    private GameObject GetClickedObject() {
        Ray ray = _boardCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f)) {
            Debug.DrawLine(ray.origin, hit.point, Color.blue, 2.5f);
            return hit.transform.gameObject;
        }
        else return null;
    }

    // Highlights area around current unit according to its moving pattern.
    private void HighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = _cells[CoordinateConverter.Convert(x, z)];
        _availableMoves = _currentUnit.GetComponent<MovementBase>().CalculateAvailableMoves(currentCell);

        foreach (Vector2Int availableMove in _availableMoves) {
            int i = CoordinateConverter.Convert(availableMove.x, availableMove.y);
            Cell cell = _cells[i];
            CellHightlight cellHighlight = _ch[i];

            if (!cell.IsOccupied) cellHighlight.PaintMoveCell();
        }
    }

    // Highlights area around current unit according to its attack pattern.
    public void HighlightAttackArea(bool isSpecial) {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = _cells[CoordinateConverter.Convert(x, z)];

        if (isSpecial) _availableAttacks = _currentUnit.GetComponent<SpecialBase>().CalculateAvailableSpecials(currentCell);
        else _availableAttacks = _currentUnit.GetComponent<AttackBase>().CalculateAvailableAttacks(currentCell);

        foreach (Vector2Int attack in _availableAttacks) {
            int i = CoordinateConverter.Convert(attack.x, attack.y);
            Cell cell = _cells[i];
            CellHightlight cellHighlight = _ch[i];

            if (!cell.IsOccupied) cellHighlight.PaintAttackCell();
            else cellHighlight.PaintAttackOccupiedCell();
        }
    }

    private void UnhighlightArea() {
        foreach (CellHightlight ch in _ch) ch.PaintCellToOriginalColor();
    }

    // Moves unit to a new location over a period of time.
    private IEnumerator MoveClickedObject(GameObject clickedObject, Vector3 endPosition) {
        if (_isUnitMoving) yield break;
        _isUnitMoving = true;
        Vector3 startPosition = clickedObject.transform.position;
        float percComplete = 0f;
        float duration = 2f;
        while (percComplete < 1) {
            percComplete += Time.deltaTime / duration;
            clickedObject.transform.position = Vector3.Lerp(startPosition, endPosition, percComplete);
            yield return null;
        }
        _isUnitMoving = false;
    }

    public void DetermineTurnCompletion() {
        int unitCount = _currentTeam == TeamColor.Red ? _redUnits.Count : _blueUnits.Count;
        int unitsCompletedTurn = 0;
        
        if (_currentTeam == TeamColor.Red) 
            foreach (var unitState in _redUnitsState) 
                if (unitState.HasFinishedTurn()) unitsCompletedTurn++;

        if (_currentTeam == TeamColor.Blue) 
            foreach (var unitState in _blueUnitsState) 
                if (unitState.HasFinishedTurn()) unitsCompletedTurn++;

        if (unitCount == unitsCompletedTurn) {
            _turnCount++;

            if (_currentTeam == TeamColor.Red) {
                _currentTeam = TeamColor.Blue;
                _boardCamera.transform.SetPositionAndRotation(new Vector3(0, 75, -8), Quaternion.Euler(new Vector3(90, 180, 0)));
                ResetUnitsStates(TeamColor.Blue);
            }
            else {
                _currentTeam = TeamColor.Red;
                _boardCamera.transform.SetPositionAndRotation(new Vector3(0, 75, -12), Quaternion.Euler(new Vector3(90, 0, 0)));
                ResetUnitsStates(TeamColor.Red);
            }

            OnTurnOver?.Invoke();
        }
    }

    public void DetermineGameOver() {
        if (_redUnits.Count == 0 || !_redKing) OnGameOver?.Invoke(TeamColor.Blue);
        else if (_blueUnits.Count == 0 || !_blueKing) OnGameOver?.Invoke(TeamColor.Red);
    }

    public void SkipTurnButton() {
        _currentUnit.GetComponent<UnitState>().HasSkipped = true;
        _currentUnit.GetComponent<UnitColor>().SetColorToFinishedTurn();
        UnhighlightArea();
        _currentUnit = null;
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        _specialAttackButton.interactable = false;
        DetermineTurnCompletion();
    }

    public void AttackButton(bool isSpecial) {
        if (!_isAttackInitiated) {
            _isAttackInitiated = true;
            StartCoroutine(WaitForAttack(isSpecial));
        }
    }

    public void ChooseUnitPositionButton(int unitIndex) {
        if (_clickedObject) _clickedObject = null;
        if (_currentTeam == TeamColor.Red) {
            for (int i = 15; i < _board.Length; i++) {
                _board[i].SetActive(false);
                _boardHighlight[i].SetActive(false);
            }
        }
        else {
            for (int i = 19; i > 0; i--) {
                _board[i].SetActive(false);
                _boardHighlight[i].SetActive(false);
            }
        }

        StartCoroutine(PositioningCourutine(unitIndex));
    }

    public void MainMenuButton() {
        SceneManager.LoadScene(0);
    }

    public void QuitGameButton() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private IEnumerator PositioningCourutine(int unitIndex) {
        while (!_clickedObject) {
            yield return null;
        }

        int clickedCell = int.Parse(_clickedObject.name);
        int x = CoordinateConverter.GetKeyFromValue(clickedCell).Item1;
        int z = CoordinateConverter.GetKeyFromValue(clickedCell).Item2;

        if (_currentTeam == TeamColor.Red) {
            _redUnits[unitIndex].transform.position = new Vector3(x, 2.5f, z);
            _cells[clickedCell].Occupy(_redUnits[unitIndex]);
        }
        else {
            _blueUnits[unitIndex].transform.position = new Vector3(x, 2.5f, z);
            _cells[clickedCell].Occupy(_blueUnits[unitIndex]);
        }

        _unitLines[unitIndex].SetActive(false);

        if (unitIndex == 4 && _currentTeam == TeamColor.Red) {
            for (int i = 15; i < _board.Length; i++) {
                _board[i].SetActive(true);
                _boardHighlight[i].SetActive(true);
            }  

            for (int i = 0; i < 10; i++) _ch[i].PaintCellToOriginalColor();
            for (int i = 34; i > 24; i--) _ch[i].PaintPositioningCell();
            foreach (GameObject unitLine in _unitLines) unitLine.SetActive(true);
            _boardCamera.transform.SetPositionAndRotation(new Vector3(0, 75, -8), Quaternion.Euler(new Vector3(90, 180, 0)));
            _currentTeam = TeamColor.Blue;
            OnUnitPositioning?.Invoke(TeamColor.Blue);
        }

        else if (unitIndex == 4 && _currentTeam == TeamColor.Blue) {
            for (int i = 19; i > 0; i--) {
                _board[i].SetActive(true);
                _boardHighlight[i].SetActive(true);
            }

            for (int i = 34; i > 24; i--) _ch[i].PaintCellToOriginalColor();
            foreach (GameObject uiObject in _uiObjects) uiObject.SetActive(true);
            _positioningPanel.SetActive(false);
            _boardCamera.transform.SetPositionAndRotation(new Vector3(0, 75, -12), Quaternion.Euler(new Vector3(90, 0, 0)));
            _currentTeam = TeamColor.Red;
        }
    }

    private void ResetUnitsStates(TeamColor teamColor) {
        if (teamColor == TeamColor.Red) {
            for (int i = 0; i < _redUnits.Count; i++) {
                _redUnitsState[i].ResetUnitState();
                _redUnits[i].GetComponent<UnitColor>().SetColorToOriginal();
            }
            for (int i = 0; i < _blueUnits.Count; i++) _blueUnits[i].GetComponent<UnitColor>().SetColorToOriginal();
        }
        else {
            for (int i = 0; i < _blueUnits.Count; i++) {
                _blueUnitsState[i].ResetUnitState();
                _blueUnits[i].GetComponent<UnitColor>().SetColorToOriginal();
            }
            for (int i = 0; i < _redUnits.Count; i++) _redUnits[i].GetComponent<UnitColor>().SetColorToOriginal();
        }
    }

    // Waits for user's click and then attacks clicked enemy unit.
    private IEnumerator WaitForAttack(bool isSpecial) {
        UnhighlightArea();
        HighlightAttackArea(isSpecial);
        SpecialType unitSpecialType = _currentUnit.GetComponent<SpecialBase>().SpecialType;
        bool hasUnitAttacked = false;

        while (!_clickedEnemyUnit) {
            if (Input.GetMouseButtonDown(0)) {
                if (!isSpecial) {
                    if (_clickedObject.CompareTag("unit") && _clickedObject.GetComponent<Stats>().Team != _currentUnit.GetComponent<Stats>().Team) {
                        _clickedEnemyUnit = _clickedObject;
                        hasUnitAttacked = _currentUnit.GetComponent<AttackBase>().AttackUnit(_clickedEnemyUnit);
                        break;
                    }
                    else break;
                }

                else if (unitSpecialType == SpecialType.NonTargeted) {
                    List<GameObject> unitsToAttack = new List<GameObject>();
                    foreach (Vector2Int attack in _availableAttacks) {
                        int i = CoordinateConverter.Convert(attack.x, attack.y);
                        if (_cells[i].IsOccupied) unitsToAttack.Add(_cells[i].IsOccupied);
                    }
                    
                    hasUnitAttacked = _currentUnit.GetComponent<SpecialBase>().SpecialAttackNonTargetedUnit(unitsToAttack);
                    break;
                }

                else {
                    if (_clickedObject.CompareTag("unit") && _clickedObject.GetComponent<Stats>().Team != _currentUnit.GetComponent<Stats>().Team) {
                        _clickedEnemyUnit = _clickedObject;
                        hasUnitAttacked = _currentUnit.GetComponent<SpecialBase>().SpecialAttackTargetedUnit(_clickedEnemyUnit);
                        break;
                    }
                    else break;
                }
            }
            yield return null;
        }

        if (hasUnitAttacked) {
            _currentUnit.GetComponent<UnitColor>().SetColorToFinishedTurn();
            _onUnitAttacked.Invoke();
        }

        UnhighlightArea();
        _currentUnit = null;
        _clickedEnemyUnit = null;
        _isAttackInitiated = false;
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        _specialAttackButton.interactable = false;
        DetermineTurnCompletion();
    }

    private void RemoveDeadUnit(GameObject enemyUnit, TeamColor team) {
        int x = (int)enemyUnit.transform.position.x;
        int z = (int)enemyUnit.transform.position.z;
        int unitCoords = CoordinateConverter.Convert(x, z);

        if (team == TeamColor.Red) {
            _redUnits.Remove(enemyUnit);
            _redUnitsState.Remove(enemyUnit.GetComponent<UnitState>());
        }
        else {
            _blueUnits.Remove(enemyUnit);
            _blueUnitsState.Remove(enemyUnit.GetComponent<UnitState>());
        }

        _cells[unitCoords].Deoccupy();

        Destroy(enemyUnit);
        Invoke("DetermineGameOver", 0.1f);
    }

    private void IncrementTotalDamage(int damageDone, TeamColor team) {
        if (team == TeamColor.Red) _redTotalDamage += damageDone;
        else _blueTotalDamage += damageDone;
    }

    private void InitiatePositioningPhase() {
        foreach (GameObject uiObject in _uiObjects) uiObject.SetActive(false);
        for (int i = 0; i < 10; i++) _ch[i].PaintPositioningCell();
    }

    void OnEnable() {
        Health.OnUnitDied += RemoveDeadUnit;
        AttackBase.OnUnitTakeDamage += IncrementTotalDamage;
        SpecialBase.OnUnitTakeDamage += IncrementTotalDamage;
    }

    void OnDisable() {
        Health.OnUnitDied -= RemoveDeadUnit;
        AttackBase.OnUnitTakeDamage -= IncrementTotalDamage;
        SpecialBase.OnUnitTakeDamage -= IncrementTotalDamage;
    }
}

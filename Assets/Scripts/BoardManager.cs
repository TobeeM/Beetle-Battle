using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    private GameObject[] _board;
    private Cell[] _cells;
    private CellHightlight[] _ch;

    private GameObject _currentUnit;
    private GameObject _clickedEnemyUnit;
    private GameObject _clickedObject;
    private List<Vector2Int> _availableMoves;
    private List<Vector2Int> _availableAttacks;
    private bool _isUnitMoving;
    private bool _isYourTurn;
    private int _turnCount;
    private Camera _boardCamera;
    private bool _isAttackInitiated;

    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;
    private List<UnitState> _redUnitsState;
    private List<UnitState> _blueUnitsState;
    private GameObject _blueKing;
    private GameObject _redKing;

    public List<GameObject> RedUnits => _redUnits;
    public List<GameObject> BlueUnits => _blueUnits;
    public int TurnCount => _turnCount;
    public bool IsYourTurn => _isYourTurn;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent _onUnitMoved;
    [SerializeField] private UnityEvent _onUnitAttacked;
    [SerializeField] private UnityEvent _onTurnOver;
    [SerializeField] private UnityEvent _onGameOver;

    [Header("Buttons")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _skipTurnButton;
    [SerializeField] private Button _specialAttackButton;

    void Awake() {
        _boardCamera = Camera.main;
        _availableMoves = new List<Vector2Int>();
        _turnCount = 1;
        _board = GameObject.FindGameObjectsWithTag("cell").OrderBy(cell => int.Parse(cell.name)).ToArray();
        _cells = new Cell[_board.Length];
        _ch = new CellHightlight[_board.Length];
        _isYourTurn = true;

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = _board[i].GetComponent<Cell>();
            _ch[i] = _board[i].GetComponent<CellHightlight>();
        }

        Debug.Log("Press 1 to check units' states.");
        Debug.Log("Press 2 to skip enemy turn.");
        Debug.Log("Press 3 to skip your turn.");
        Debug.Log("Press Q to change camera angle to vertical.");
        Debug.Log("Press W to return camera angle to normal.");
    }

    void Start() {
        //GameManager.OnGameStateChanged += OnGameStateChanged;
        _redUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 1).ToList();
        _blueUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 0).ToList();
        _redUnitsState = new List<UnitState>();
        _blueUnitsState = new List<UnitState>();
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        _specialAttackButton.interactable = false;

        foreach (GameObject unit in _redUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.Convert(x, z)].Occupy();
            _redUnitsState.Add(unit.GetComponent<UnitState>());
        }

        foreach (GameObject unit in _blueUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.Convert(x, z)].Occupy();
            _blueUnitsState.Add(unit.GetComponent<UnitState>());
        }

        _blueKing = GameObject.Find("Blue King");
        _redKing = GameObject.Find("Red King");
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _clickedObject = GetClickedObject();
            if (_clickedObject) Debug.Log($"You clicked {_clickedObject.name}");

            // Only set black color to the current unit.
            if (_currentUnit && _clickedObject != _currentUnit) _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

            // If clicked object is a unit, assign _currentUnit variable to it.
            if (_clickedObject && !_clickedObject.CompareTag("cell") && _clickedObject.GetComponent<Stats>().Team == TeamColor.Red && !_isAttackInitiated) {
                UnhighlightArea();
                _currentUnit = _clickedObject;

                if (!_currentUnit.GetComponent<UnitState>().HasFinishedTurn()) {
                    _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    _attackButton.interactable = true;
                    _skipTurnButton.interactable = true;
                    _specialAttackButton.interactable = true;
                    if (!_currentUnit.GetComponent<UnitState>().HasMoved) HighlightArea();
                }
                else _currentUnit = null;
            }

            // Current Unit movement.
            if (_clickedObject && _clickedObject.CompareTag("cell") && _currentUnit && _isYourTurn && !_isAttackInitiated &&
            !_currentUnit.GetComponent<UnitState>().HasMoved) {
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
                    _cells[currentCoords].Occupy();
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

        // Debug Button 1
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            for (int i = 0; i < 5; i++) {
                Debug.Log($"Red unit {i + 1} state is {_redUnitsState[i].HasFinishedTurn()}");
                Debug.Log($"Blue unit {i + 1} state is {_blueUnitsState[i].HasFinishedTurn()}");
            }
        }

        // Debug Button 2
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _isYourTurn = true;
            _turnCount++;
            _onTurnOver.Invoke();
            foreach (UnitState us in _redUnitsState) us.ResetUnitState();
            foreach (GameObject unit in _redUnits) {
                unit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }

        // Debug Button 3
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            _isYourTurn = false;
            _turnCount++;
            _onTurnOver.Invoke();
            foreach (UnitState us in _redUnitsState) us.HasMoved = true;
        }

        // Debug Button 4
        if (Input.GetKeyDown(KeyCode.Q)) {
            _boardCamera.transform.position = new Vector3(0, 90, -10);
            _boardCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        // Debug Button 5
        if (Input.GetKeyDown(KeyCode.W)) {
            _boardCamera.transform.position = new Vector3(0, 35, -70);
            _boardCamera.transform.rotation = Quaternion.Euler(40, 0, 0);
        }
    }

    private void OnGameStateChanged(GameState gameState) {
        
    }

    // Returns clicked GameObject.
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

    // Paints all cells to its original colors.
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

    // Determines whether the turn was completed.
    public void DetermineTurnCompletion() {
        int unitCount = _redUnits.Count;
        int unitsCompletedTurn = 0;

        foreach (var unitState in _redUnitsState) if (unitState.HasFinishedTurn()) unitsCompletedTurn++;
        if (unitCount == unitsCompletedTurn) {
            _isYourTurn = false;
            _turnCount++;
            _onTurnOver.Invoke();
        }
    }

    public void DetermineGameOver() {
        if (_redUnits.Count == 0 || _blueUnits.Count == 0 || !_blueKing || !_redKing) {
            _onGameOver.Invoke();
            Debug.Log("Game over!");
        }
    }

    // Skips current unit's turn.
    public void SkipTurnButton() {
        _currentUnit.GetComponent<UnitState>().HasSkipped = true;
        UnhighlightArea();
        _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        _currentUnit = null;
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        DetermineTurnCompletion();
    }

    // Initiates the attack.
    public void AttackButton(bool isSpecial) {
        if (!_isAttackInitiated) {
            _isAttackInitiated = true;
            StartCoroutine(WaitForAttack(isSpecial));
            DetermineTurnCompletion();
            DetermineGameOver();
        }
    }

    // Waits for user's click and then attacks clicked enemy unit.
    private IEnumerator WaitForAttack(bool isSpecial) {
        Debug.Log("Coroutine is now running!");
        bool unitAttacked = false;
        UnhighlightArea();
        HighlightAttackArea(isSpecial);

        while (!_clickedEnemyUnit) {
            if (Input.GetMouseButtonDown(0)) {
                if (_clickedObject.CompareTag("unit") && 
                _clickedObject.GetComponent<Stats>().Team != _currentUnit.GetComponent<Stats>().Team) {
                    _clickedEnemyUnit = _clickedObject;
                    if (isSpecial) unitAttacked = _currentUnit.GetComponent<SpecialBase>().SpecialAttackUnit(_clickedEnemyUnit);
                    else unitAttacked = _currentUnit.GetComponent<AttackBase>().AttackUnit(_clickedEnemyUnit);

                    if (unitAttacked) {
                        _currentUnit.GetComponent<UnitState>().HasAttacked = true;
                        _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        _clickedEnemyUnit.GetComponent<Health>().CalculateHealth(_clickedEnemyUnit.
                        GetComponent<Stats>().Team == TeamColor.Red ? _redUnits : _blueUnits);
                        _onUnitAttacked.Invoke();
                    }
                }
                else break;
            }
            yield return null;
        }

        UnhighlightArea();
        if (!unitAttacked) _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        _currentUnit = null;
        _clickedEnemyUnit = null;
        _isAttackInitiated = false;
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        _specialAttackButton.interactable = false;
        Debug.Log("Coroutine is no longer running!");
    }

    void OnDestroy() {
        //GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
}

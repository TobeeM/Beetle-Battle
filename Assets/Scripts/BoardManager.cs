using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity.VisualScripting;
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
    private List<int> _availableMoves;
    private List<int> _availableAttacks;
    private bool _isUnitMoving;
    private bool _isYourTurn;
    private int _turnCount;
    private Camera _boardCamera;
    private bool _isAttackInitiated;

    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;
    private List<UnitState> _redUnitsState;
    private List<UnitState> _blueUnitsState;

    public List<GameObject> RedUnits => _redUnits;
    public List<GameObject> BlueUnits => _blueUnits;
    public int TurnCount => _turnCount;
    public bool IsYourTurn => _isYourTurn;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent _onUnitMoved;
    [SerializeField] private UnityEvent _onUnitAttacked;
    [SerializeField] private UnityEvent _onTurnOver;

    [Header("Buttons")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _skipTurnButton;

    void Awake() {
        _boardCamera = Camera.main;
        _availableMoves = new List<int>();
        _turnCount = 1;
        _board = GameObject.FindGameObjectsWithTag("cell").OrderBy(cell => int.Parse(cell.name)).ToArray();
        _cells = new Cell[_board.Length];
        _ch = new CellHightlight[_board.Length];
        _isYourTurn = true;

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = _board[i].GetComponent<Cell>();
            _ch[i] = _board[i].GetComponent<CellHightlight>();
        }
    }

    void Start() {
        //GameManager.OnGameStateChanged += OnGameStateChanged;
        _redUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 1).ToList();
        _blueUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 0).ToList();
        _redUnitsState = new List<UnitState>();
        _blueUnitsState = new List<UnitState>();
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;

        foreach (GameObject unit in _redUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.ConvertCoordinates(x, z)].Occupy();
            _redUnitsState.Add(unit.GetComponent<UnitState>());
        }

        foreach (GameObject unit in _blueUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.ConvertCoordinates(x, z)].Occupy();
            _blueUnitsState.Add(unit.GetComponent<UnitState>());
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _clickedObject = GetClickedObject();
            if (_clickedObject) Debug.Log($"You clicked {_clickedObject.name}");

            // If clicked object is a unit, assign _currentUnit variable to it.
            if (_clickedObject && _clickedObject.CompareTag("unit") && !_currentUnit) {
                _currentUnit = _clickedObject;
                if (!_currentUnit.GetComponent<UnitState>().HasFinishedTurn()) {
                    _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    _attackButton.interactable = true;
                    _skipTurnButton.interactable = true;
                    if (!_currentUnit.GetComponent<UnitState>().HasMoved) HighlightArea();
                }
                else _currentUnit = null;
            }

            // Current Unit movement.
            if (_clickedObject && _clickedObject.CompareTag("cell") && _currentUnit && _isYourTurn && !_isAttackInitiated &&
            !_currentUnit.GetComponent<UnitState>().HasMoved) {
                _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                Vector3 moveLocation = new Vector3(_clickedObject.transform.position.x, 3, 
                _clickedObject.transform.position.z); 
                UnhighlightArea();

                int x = (int)_currentUnit.transform.position.x;
                int z = (int)_currentUnit.transform.position.z;
                int currentCoords = CoordinateConverter.ConvertCoordinates(x, z);
                _cells[currentCoords].Deoccupy();

                _currentUnit.GetComponent<Movement>().Move(moveLocation, _cells);

                x = (int)_currentUnit.transform.position.x;
                z = (int)_currentUnit.transform.position.z;
                currentCoords = CoordinateConverter.ConvertCoordinates(x, z);
                _cells[currentCoords].Occupy();
        
                _currentUnit.GetComponent<UnitState>().HasMoved = true;

                _currentUnit = null;
                _attackButton.interactable = false;
                _skipTurnButton.interactable = false;
                _onUnitMoved.Invoke();
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
        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            int i = availableMove + int.Parse(currentCell.name);
            Cell cell = _cells[i];
            CellHightlight cellHighlight = _ch[i];

            if (!cell.IsOccupied) cellHighlight.PaintMoveCell();
        }
    }

    // Highlights area around current unit according to its attack pattern.
    public void HighlightAttackArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableAttacks = _currentUnit.GetComponent<Attack>().CalculateAvailableAttacks(currentCell);

        foreach (int availableAttack in _availableAttacks) {
            int i = availableAttack + int.Parse(currentCell.name);
            Cell cell = _cells[i];
            CellHightlight cellHighlight = _ch[i];

            if (!cell.IsOccupied) cellHighlight.PaintAttackCell();
            else cellHighlight.PaintAttackOccupiedCell();
        }
    }

    // Unhighlights area around current unit (returns cells' colors to its original values).
    private void UnhighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;

        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            _ch[availableMove + int.Parse(currentCell.name)].PaintCellToOriginalColor();
        }
    }

    // Unhighlights area around current unit (returns cells' colors to its original values).
    private void UnhighlightAttackArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;

        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableAttacks = _currentUnit.GetComponent<Attack>().CalculateAvailableAttacks(currentCell);

        foreach (int availableAttack in _availableAttacks) {
            _ch[availableAttack + int.Parse(currentCell.name)].PaintCellToOriginalColor();
        }
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
    public void AttackButton() {
        if (!_isAttackInitiated) {
            _isAttackInitiated = true;
            StartCoroutine(WaitForAttack());
            DetermineTurnCompletion();
        }
    }

    // Waits for user's click and then attacks clicked enemy unit.
    private IEnumerator WaitForAttack() {
        Debug.Log("Coroutine is now running!");
        bool unitAttacked = false;
        HighlightAttackArea();

        while (!_clickedEnemyUnit) {
            if (Input.GetMouseButtonDown(0)) {
                if (_clickedObject.CompareTag("unit") && 
                _clickedObject.GetComponent<Stats>().Team != _currentUnit.GetComponent<Stats>().Team) {
                    _clickedEnemyUnit = _clickedObject;
                    unitAttacked = _currentUnit.GetComponent<Attack>().AttackUnit(_clickedEnemyUnit);
                    if (unitAttacked) {
                        _currentUnit.GetComponent<UnitState>().HasAttacked = true;
                        _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        _clickedEnemyUnit.GetComponent<Health>().CalculateHealth(_clickedEnemyUnit.
                        GetComponent<Stats>().Team == TeamColor.Red ? _redUnits : _blueUnits);
                        _onUnitAttacked.Invoke();
                    }
                }
                else if (_clickedObject.CompareTag("cell")) break;
            }
            yield return null;
        }
        UnhighlightAttackArea();
        if (!unitAttacked) _currentUnit.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        _currentUnit = null;
        _clickedEnemyUnit = null;
        _isAttackInitiated = false;
        _attackButton.interactable = false;
        _skipTurnButton.interactable = false;
        Debug.Log("Coroutine is no longer running!");
    }

    void OnDestroy() {
        //GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
}

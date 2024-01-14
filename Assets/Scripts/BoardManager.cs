using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BoardManager : MonoBehaviour
{
    private GameObject[] _board;
    private Cell[] _cells;
    private CellHightlight[] _ch;

    private GameObject _currentUnit;
    private GameObject _clickedEnemyUnit;
    private List<int> _availableMoves;
    private bool _isUnitMoving;
    private bool _isYourTurn;
    private bool _isEnemyTurn;
    private int _turnCount;
    private Camera _boardCamera;

    private List<GameObject> _redUnits;
    private List<GameObject> _blueUnits;

    public List<GameObject> RedUnits => _redUnits;
    public List<GameObject> BlueUnits => _blueUnits;
    public int TurnCount => _turnCount;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent _onUnitDied;
    [SerializeField] private UnityEvent _onUnitMoved;


    void Awake()
    {
        _boardCamera = Camera.main;
        _availableMoves = new List<int>();
        _turnCount = 1;
        _board = GameObject.FindGameObjectsWithTag("cell").OrderBy(cell => int.Parse(cell.name)).ToArray();
        _cells = new Cell[_board.Length];
        _ch = new CellHightlight[_board.Length];

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = _board[i].GetComponent<Cell>();
            _ch[i] = _board[i].GetComponent<CellHightlight>();
        }
    }

    void Start()
    {
        //GameManager.OnGameStateChanged += OnGameStateChanged;
        _redUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 1).ToList();
        _blueUnits = GameObject.FindGameObjectsWithTag("unit").Where(unit => unit.GetComponent<Renderer>().material.color.r == 0).ToList();

        foreach (GameObject unit in _redUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.ConvertCoordinates(x, z)].Occupy();
        }

        foreach (GameObject unit in _blueUnits) {
            int x = (int)unit.transform.position.x;
            int z = (int)unit.transform.position.z;

            _cells[CoordinateConverter.ConvertCoordinates(x, z)].Occupy();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            GameObject clickedObject = GetClickedObject();
            Debug.Log($"You clicked {clickedObject.name}");

            if (clickedObject.CompareTag("unit") && !_currentUnit) {
                _currentUnit = clickedObject;
                HighlightArea();
                _isYourTurn = true;
            }

            if (clickedObject.CompareTag("cell") && _currentUnit && _isYourTurn) {
                Vector3 moveLocation = new Vector3(clickedObject.transform.position.x, 3, 
                clickedObject.transform.position.z); 
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

                _isYourTurn = false;
                _turnCount += 1;
                _currentUnit = null;
                _onUnitMoved.Invoke();
            }

            if (clickedObject.CompareTag("unit") && _currentUnit != clickedObject) {
                _clickedEnemyUnit = clickedObject;
                float color = _clickedEnemyUnit.GetComponent<Renderer>().material.color.r;
                //_currentUnit.GetComponent<Attack>().AttackUnit(_clickedEnemyUnit);
                _currentUnit.GetComponent<Attack>().InstaKill(_clickedEnemyUnit);
                _clickedEnemyUnit.GetComponent<Health>().CalculateHealth(color == 1 ? _redUnits : _blueUnits);
            }
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            _boardCamera.transform.Translate(0, 0, 50);
            _boardCamera.transform.rotation = new Quaternion(0.5f, -0.9f, 0, 1);
        }
    }

    private void OnGameStateChanged(GameState gameState) {
        
    }

    private GameObject GetClickedObject() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f)) {
            Debug.DrawLine(ray.origin, hit.point, Color.blue, 2.5f);
            return hit.transform.gameObject;
        }
        else return null;
    }

    private void HighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            int i = availableMove + int.Parse(currentCell.name);
            Cell cell = _cells[i];
            CellHightlight cellHighlight = _ch[i];

            if (!cell.IsOccupied) cellHighlight.PaintCell();
        }
    }

    private void UnhighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;

        Cell currentCell = _cells[CoordinateConverter.ConvertCoordinates(x, z)];
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            _ch[availableMove + int.Parse(currentCell.name)].PaintCellToOriginalColor();
        }
    }

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

    void OnDestroy() {
        //GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private GameObject _currentUnit;
    private bool _isUnitChosen;
    private bool _isUnitMoving;
    private GameObject[] _cells;
    private CoordinateConverter _converter;
    private List<int> _availableMoves;

    void Awake()
    {
        _availableMoves = new List<int>();
        _converter = gameObject.GetComponent<CoordinateConverter>();
    }

    void Start()
    {
        _cells = GameObject.FindGameObjectsWithTag("cell").OrderBy(cell => int.Parse(cell.name)).ToArray();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !_isUnitChosen) {
            _isUnitChosen = true;
            _currentUnit = GetClickedUnit();
            HighlightArea();
        }
        if (Input.GetMouseButtonDown(1) && _isUnitChosen) {
            _isUnitChosen = false;
            UnhighlightArea();
            Vector3 clickedLocation = GetClickedUnit().transform.position;
            Vector3 moveLocation = new Vector3(clickedLocation.x, 3, clickedLocation.z);
            Debug.Log(moveLocation);
            //MoveClickedObject(_currentUnit, moveLocation);
            _currentUnit.transform.position = moveLocation;
        }
    }

    private GameObject GetClickedUnit() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit, 1000f);
        Debug.DrawLine(ray.origin, hit.point, Color.blue, 2.5f);
        return hit.transform.gameObject;
    }

    private void HighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = GameObject.Find(_converter.ConvertCoordinates(x, z).ToString()).GetComponent<Cell>();
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            _cells[availableMove + currentCell.Number].GetComponent<CellHightlight>().PaintCell();
        }
    }

    private void UnhighlightArea() {
        int x = (int)_currentUnit.transform.position.x;
        int z = (int)_currentUnit.transform.position.z;
        Cell currentCell = GameObject.Find(_converter.ConvertCoordinates(x, z).ToString()).GetComponent<Cell>();
        _availableMoves = _currentUnit.GetComponent<Movement>().CalculateAvailableMoves(currentCell);

        foreach (int availableMove in _availableMoves) {
            _cells[availableMove + currentCell.Number].GetComponent<CellHightlight>().PaintCellToOriginalColor();
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
}

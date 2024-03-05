using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    private List<int> _availableMoves = new List<int>();
    public List<int> CalculateAvailableMoves(Cell currentCell) {
        _availableMoves.Clear();
        switch (currentCell.Type) {
            case CellType.Inner:
                _availableMoves.AddRange(new List<int> {-6, -5, -4, -1, 1, 4, 5, 6});
                break;
            case CellType.Left:            
                _availableMoves.AddRange(new List<int> {-5, -4, 1, 5, 6});
                break;
            case CellType.Right:
                _availableMoves.AddRange(new List<int> {-6, -5, -1, 4, 5});
                break;
            case CellType.Top:
                _availableMoves.AddRange(new List<int> {-1, 1, -6, -5, -4});
                break;
            case CellType.Bottom:
                _availableMoves.AddRange(new List<int> {4, 5, 6, -1, 1});
                break;
            case CellType.TopLeft:
                _availableMoves.AddRange(new List<int> {1, -5, -4});
                break;
            case CellType.TopRight:
                _availableMoves.AddRange(new List<int> {-1, -6, -5});
                break;
            case CellType.BottomLeft:
                _availableMoves.AddRange(new List<int> {1, 6, 5});
                break;
            case CellType.BottomRight:
                _availableMoves.AddRange(new List<int> {-1, 4, 5});
                break;
        }
        return _availableMoves;
    } 

    public bool Move(Vector3 targetCoords, Cell[] cells) {
        int unitCoords = CoordinateConverter.Convert((int)transform.position.x, (int)transform.position.z);
        int moveTo = CoordinateConverter.Convert((int)targetCoords.x, (int)targetCoords.z);
        Vector3 moveLocation = new Vector3(targetCoords.x, 3, targetCoords.z);

        foreach (int move in _availableMoves) {
            int finalMove = unitCoords + move;
            Cell cell = cells[finalMove];
            
            if (finalMove == moveTo && !cell.IsOccupied) {
                transform.position = moveLocation;
                return true;
            }
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private List<int> _availableMoves = new List<int>();
    private int _position;

    public int Position => _position;

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

    public void Move(Cell targetCell) {
        _position = targetCell.Number;
    }
}

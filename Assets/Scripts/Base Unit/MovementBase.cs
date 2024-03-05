using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBase : MonoBehaviour
{
    protected List<Vector2Int> _availableMoves = new List<Vector2Int>();
    protected Vector2Int[] _movementPattern;

    public List<Vector2Int> CalculateAvailableMoves(Cell currentCell) {
        _availableMoves.Clear();

        Vector2Int currentPosition = new Vector2Int((int)currentCell.transform.position.x, (int)currentCell.transform.position.z);
        foreach (Vector2Int move in _movementPattern) {
            int finalX = currentPosition.x + move.x;
            int finalY = currentPosition.y + move.y;

            if (finalX >= -20 && finalX <= 20 && finalY >= -40 && finalY <= 20)
                _availableMoves.Add(new Vector2Int(finalX, finalY));
        }

        return _availableMoves;
    }

    public virtual bool Move(Vector3 targetCoords, Cell targetCell) {
        Vector2Int moveTo = new Vector2Int((int)targetCoords.x, (int)targetCoords.z);

        foreach (Vector2Int move in _availableMoves) {       
            if (move == moveTo && !targetCell.IsOccupied) {
                transform.position = new Vector3(targetCoords.x, 3, targetCoords.z);
                return true;
            }
        }

        return false;
    }
}
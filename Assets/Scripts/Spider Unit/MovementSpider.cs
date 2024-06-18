using UnityEngine;

public class MovementSpider : MovementBase
{
    void Awake() {
        _movementPattern = new Vector2Int[16];

        _movementPattern[0] = new Vector2Int(-40, -40);
        _movementPattern[1] = new Vector2Int(-30, -30);
        _movementPattern[2] = new Vector2Int(-20, -20);
        _movementPattern[3] = new Vector2Int(-10, -10);
        _movementPattern[4] = new Vector2Int(40, -40);
        _movementPattern[5] = new Vector2Int(30, -30);
        _movementPattern[6] = new Vector2Int(20, -20);
        _movementPattern[7] = new Vector2Int(10, -10);
        _movementPattern[8] = new Vector2Int(10, 10);
        _movementPattern[9] = new Vector2Int(20, 20);
        _movementPattern[10] = new Vector2Int(30, 30);
        _movementPattern[11] = new Vector2Int(40, 40);
        _movementPattern[12] = new Vector2Int(-10, 10);
        _movementPattern[13] = new Vector2Int(-20, 20);
        _movementPattern[14] = new Vector2Int(-30, 30);
        _movementPattern[15] = new Vector2Int(-40, 40);
    }
}
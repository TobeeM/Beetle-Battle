using UnityEngine;

public class MovementBee : MovementBase
{
    void Awake() {
        int count = 0;
        _movementPattern = new Vector2Int[24];

        for (int x = -20; x <= 20; x += 10) {
            for (int y = -20; y <= 20; y += 10) {
                if (x == 0 && y == 0) continue;
                _movementPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }
    }
}
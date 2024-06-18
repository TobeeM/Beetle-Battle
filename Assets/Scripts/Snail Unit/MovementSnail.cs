using UnityEngine;

public class MovementSnail : MovementBase
{
    void Awake() {
        int count = 0;
        _movementPattern = new Vector2Int[8];

        for (int x = -10; x <= 10; x += 10) {
            for (int y = -10; y <= 10; y += 10) {
                if (x == 0 && y == 0) continue;
                _movementPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }
    }
}

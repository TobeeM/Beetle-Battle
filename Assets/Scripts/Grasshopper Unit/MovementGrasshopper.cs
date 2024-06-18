using UnityEngine;

public class MovementGrasshopper : MovementBase
{
    void Awake() {
        int count = 0;
        _movementPattern = new Vector2Int[12];

        for (int x = -10; x <= 10; x += 10) {
            for (int y = -10; y <= 10; y += 10) {
                if (x == 0 && y == 0) continue;
                _movementPattern[count] = new Vector2Int(x, y);
                count++;
            }
        }

        _movementPattern[8] = new Vector2Int(-20, -20);
        _movementPattern[9] = new Vector2Int(20, -20);
        _movementPattern[10] = new Vector2Int(20, -20);
        _movementPattern[11] = new Vector2Int(20, 20);
    }
}

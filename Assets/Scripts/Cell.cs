using UnityEngine;

public class Cell : MonoBehaviour
{
    private GameObject _isOccupied;
    public GameObject IsOccupied => _isOccupied;

    public void Occupy(GameObject unit) {
        _isOccupied = unit;
    }

    public void Deoccupy() {
        _isOccupied = null;
    }
}

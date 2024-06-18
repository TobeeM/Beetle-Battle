using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitColor : MonoBehaviour
{
    private Color _originalColor;
    private Material _material;
    private List<Material> _bodyParts = new List<Material>();

    void Awake() {
        _material = GetComponent<Renderer>().material;

        _bodyParts.Add(_material);
        foreach (Transform child in transform) {
            _bodyParts.Add(child.GetComponent<Renderer>().material);
        }
    }

    public void SetOriginalColor(Color color) {
        _originalColor = color;
    }

    public void SetColorToOriginal() {
        foreach (Material bodyPart in _bodyParts) bodyPart.SetColor("_Color", _originalColor);
    }

    public void SetColorToFinishedTurn() {
        foreach (Material bodyPart in _bodyParts) bodyPart.SetColor("_Color", Color.gray);
    }

    public void SetColorToCurrentUnit() {
        foreach (Material bodyPart in _bodyParts) bodyPart.SetColor("_Color", Color.black);
    }
}

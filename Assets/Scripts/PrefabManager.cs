using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject _baseUnitPrefab;
    [SerializeField] private GameObject _beeUnitPrefab;
    [SerializeField] private GameObject _redParent;
    [SerializeField] private GameObject _blueParent;

    private GameObject _instantiatedUnit;
    private Dictionary<int, Vector3> _redPositionDict;
    private Dictionary<int, Vector3> _bluePositionDict;


    void Awake() {
        _redPositionDict = new Dictionary<int, Vector3>() {
            {0, new Vector3(0, 2.5f, -40)},
            {1, new Vector3(0, 2.5f, -30)},
            {2, new Vector3(10, 2.5f, -30)},
            {3, new Vector3(20, 2.5f, -30)},
            {4, new Vector3(10, 2.5f, -40)},
        };

        _bluePositionDict = new Dictionary<int, Vector3>() {
            {0, new Vector3(0, 2.5f, 20)},
            {1, new Vector3(10, 2.5f, 10)},
            {2, new Vector3(20, 2.5f, 0)},
            {3, new Vector3(20, 2.5f, 20)},
            {4, new Vector3(-20, 2.5f, 20)},
        };

        InstantiateRedUnits();
        InstantiateBlueUnits();
    }

    private void InstantiateRedUnits() {
        Material unitMaterial;
        int kingIndex = Random.Range(0, 5);

        for (int i = 0; i < 5; i++) {
            if (i == 0) _instantiatedUnit = Instantiate(_beeUnitPrefab, _redPositionDict[i], Quaternion.identity, _redParent.transform);
            else _instantiatedUnit = Instantiate(_baseUnitPrefab, _redPositionDict[i], Quaternion.identity, _redParent.transform);

            if (i == kingIndex) {
                _instantiatedUnit.GetComponent<Stats>().IsKing = true;
                _instantiatedUnit.transform.name = $"Red King";
                _instantiatedUnit.GetComponent<Stats>().Team = TeamColor.Red;
                unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
                unitMaterial.SetColor("_Color", Color.yellow);
                continue;
            }

            _instantiatedUnit.transform.name = $"Red Unit {i + 1}";
            _instantiatedUnit.GetComponent<Stats>().Team = TeamColor.Red;
            unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
            unitMaterial.SetColor("_Color", Color.red);

        }
    }

    private void InstantiateBlueUnits() {
        Material unitMaterial;
        int kingIndex = Random.Range(0, 5);

        for (int i = 0; i < 5; i++) {
            _instantiatedUnit = Instantiate(_baseUnitPrefab, _bluePositionDict[i], Quaternion.identity, _blueParent.transform);

            if (i == kingIndex) {
                _instantiatedUnit.GetComponent<Stats>().IsKing = true;
                _instantiatedUnit.transform.name = $"Blue King";
                _instantiatedUnit.GetComponent<Stats>().Team = TeamColor.Blue;
                unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
                unitMaterial.SetColor("_Color", Color.cyan);
                continue;
            }

            _instantiatedUnit.transform.name = $"Blue Unit {i + 1}";
            _instantiatedUnit.GetComponent<Stats>().Team = TeamColor.Blue;
            unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
            unitMaterial.SetColor("_Color", Color.blue);   
        }
    }
}

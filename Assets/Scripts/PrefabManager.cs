using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject _baseUnitPrefab;
    [SerializeField] private GameObject _beeUnitPrefab;
    [SerializeField] private GameObject _cannonUnitPrefab;
    [SerializeField] private GameObject _mantisUnitPrefab;
    [SerializeField] private GameObject _dragonflyUnitPrefab;
    [SerializeField] private GameObject _spiderUnitPrefab;
    [SerializeField] private GameObject _snailUnitPrefab;
    [SerializeField] private GameObject _grasshopperUnitPrefab;
    [SerializeField] private GameObject _redParent;
    [SerializeField] private GameObject _blueParent;

    private GameObject _instantiatedUnit;
    private Dictionary<int, Vector3> _redPositionDict;
    private Dictionary<int, Vector3> _bluePositionDict;
    private GameObject[] _unitTypes;


    void Awake() {
        _redPositionDict = new Dictionary<int, Vector3>() {
            {0, new Vector3(200, 2.5f, -40)},
            {1, new Vector3(200, 2.5f, -30)},
            {2, new Vector3(200, 2.5f, -30)},
            {3, new Vector3(200, 2.5f, -30)},
            {4, new Vector3(200, 2.5f, -40)},
        };

        _bluePositionDict = new Dictionary<int, Vector3>() {
            {0, new Vector3(200, 2.5f, 20)},
            {1, new Vector3(200, 2.5f, 10)},
            {2, new Vector3(200, 2.5f, 0)},
            {3, new Vector3(200, 2.5f, 20)},
            {4, new Vector3(200, 2.5f, 20)},
        };

        _unitTypes = new GameObject[8] { _baseUnitPrefab, _beeUnitPrefab, _cannonUnitPrefab, _mantisUnitPrefab,
            _dragonflyUnitPrefab, _spiderUnitPrefab, _snailUnitPrefab, _grasshopperUnitPrefab };

        InstantiateRedUnits();
        InstantiateBlueUnits();
    }

    private void InstantiateRedUnits() {
        Material unitMaterial;
        UnitColor unitColor;
        Stats unitStats;
        int kingIndex = Random.Range(0, 5);
        int randomType;
        Quaternion redQuaternion = new Quaternion(0, -90, 0, 1);

        for (int i = 0; i < 5; i++) {
            randomType = Random.Range(0, 8);
            _instantiatedUnit = Instantiate(_unitTypes[randomType], _redPositionDict[i], redQuaternion, _redParent.transform);

            unitStats = _instantiatedUnit.GetComponent<Stats>();
            unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
            unitColor = _instantiatedUnit.GetComponent<UnitColor>();

            unitStats.Team = TeamColor.Red;

            if (unitStats.Type == UnitType.Spider || unitStats.Type == UnitType.Snail || unitStats.Type == UnitType.Hopper) 
                _instantiatedUnit.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 180));

            if (i == kingIndex) {
                unitStats.IsKing = true;
                _instantiatedUnit.transform.name = $"Red King";
                unitMaterial.SetColor("_Color", Color.yellow);
                unitColor.SetOriginalColor(Color.yellow);
                unitColor.SetColorToOriginal();
                continue;
            }

            _instantiatedUnit.transform.name = $"Red Unit {i + 1}";
            unitMaterial.SetColor("_Color", Color.red);
            unitColor.SetOriginalColor(Color.red);
            unitColor.SetColorToOriginal();
        }
    }

    private void InstantiateBlueUnits() {
        Material unitMaterial;
        UnitColor unitColor;
        Stats unitStats;
        int kingIndex = Random.Range(0, 5);
        int randomType;

        for (int i = 0; i < 5; i++) {
            randomType = Random.Range(0, 8);
            _instantiatedUnit = Instantiate(_unitTypes[randomType], _bluePositionDict[i], Quaternion.identity, _blueParent.transform);

            unitStats = _instantiatedUnit.GetComponent<Stats>();
            unitMaterial = _instantiatedUnit.GetComponent<Renderer>().material;
            unitColor = _instantiatedUnit.GetComponent<UnitColor>();

            unitStats.Team = TeamColor.Blue;

            if (_instantiatedUnit.TryGetComponent(out SpecialMantis sm)) sm.OverrideSpecialPatternBlue();
            if (unitStats.Type == UnitType.Spider || unitStats.Type == UnitType.Snail || unitStats.Type == UnitType.Hopper) 
                _instantiatedUnit.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 180));

            if (i == kingIndex) {
                unitStats.IsKing = true;
                _instantiatedUnit.transform.name = $"Blue King";
                unitMaterial.SetColor("_Color", Color.cyan);
                unitColor.SetOriginalColor(Color.cyan);
                unitColor.SetColorToOriginal();
                continue;
            }

            _instantiatedUnit.transform.name = $"Blue Unit {i + 1}";
            unitMaterial.SetColor("_Color", Color.blue);
            unitColor.SetOriginalColor(Color.blue);
            unitColor.SetColorToOriginal();
        }
    }
}

using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHP; // increases maximum hp pool
    [SerializeField] private int _hp; // current hp

    [Header("Offensive and Defensive stats")]
    [SerializeField] private int _atk; // increases damage done
    [SerializeField] private int _def; // decreases incoming damage
    [SerializeField] private int _dodge; // increases chance to dodge attacks (in %)

    [Header("Resistances")]
    [SerializeField] private int _immunity; // increases chance to dodge BEE and SPIDER specials (in %)
    [SerializeField] private int _fortitude; // increases chance to dodge MANTIS and KNIGHT specials (in %)
    [SerializeField] private int _durability; // increases chance to dodge SNAIL and CANNON specials (in %)
    [SerializeField] private int _clarity; // increases chance to dodge GRASSHOPPER and DRAGONFLY specials (in %)

    [Header("Extra Variables")]
    private TeamColor _team;
    private bool _isKing;
    [SerializeField] private UnitType _unitType;

    public int Hp {
        get => _hp;
        set => _hp = value;
    }
    public int MaxHP => _maxHP;
    public int Atk => _atk;
    public int Def => _def;
    public int Dodge => _dodge;
    public int Immunity => _immunity;
    public int Fortitude => _fortitude;
    public int Durability => _durability;
    public int Clarity => _clarity;
    public UnitType Type => _unitType;
    
    public TeamColor Team {
        get => _team;
        set => _team = value;
    }

    public bool IsKing {
        get => _isKing;
        set => _isKing = value;
    }

    void Awake() {
        _maxHP = Random.Range(100, 251);
        _hp = _maxHP;
        _atk = Random.Range(20, 51);
        _def = Random.Range(0, 21);
        _dodge = Random.Range(0, 26);
        _immunity = Random.Range(0, 26);
        _fortitude = Random.Range(0, 26);
        _durability = Random.Range(0, 26);
        _clarity = Random.Range(0, 26);
    }
}

public enum TeamColor {
    Red,
    Blue
}

public enum UnitType {
    Mantis,
    Knight,
    Bee,
    Hopper,
    Cannon,
    Snail,
    Spider,
    Dragonfly
}

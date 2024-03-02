using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private int _immunity; // increases chance to dodge BEE and HOPPER specials (in %)
    [SerializeField] private int _fortitude; // increases chance to dodge THROWER and KNIFE specials (in %)
    [SerializeField] private int _durability; // increases chance to dodge SNAIL and CANNON specials (in %)
    [SerializeField] private int _clarity; // increases chance to dodge FAIRY and DRAGONFLY specials (in %)

    [Header("Extra Variables")]
    private TeamColor _team;
    private bool _isKing;

    public int Hp {
        get => _hp;
        set => _hp = value;
    }
    public int MaxHP => _maxHP;
    public int Atk => _atk;
    public int Def => _def;
    public int Dodge => _dodge;
    public TeamColor Team {
        get => _team;
        set => _team = value;
    }

    public bool IsKing {
        get => _isKing;
        set => _isKing = value;
    }
}

public enum TeamColor {
    Red,
    Blue
}

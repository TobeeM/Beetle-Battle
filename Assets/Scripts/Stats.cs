using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Health and Position")]
    [SerializeField] private int _hp; // increases maximum hp pool

    [Header("Offensive and Defensive stats")]
    [SerializeField] private int _atk; // increases damage done
    [SerializeField] private int _def; // decreases incoming damage
    [SerializeField] private int _dodge; // increases chance to dodge attacks (in %)

    [Header("Resistances")]
    [SerializeField] private int _immunity; // increases chance to dodge BEE and HOPPER specials (in %)
    [SerializeField] private int _fortitude; // increases chance to dodge THROWER and KNIFE specials (in %)
    [SerializeField] private int _durability; // increases chance to dodge SNAIL and CANNON specials (in %)
    [SerializeField] private int _clarity; // increases chance to dodge FAIRY and DRAGONFLY specials (in %)

    public int Hp => _hp;
    public int Atk => _atk;
    public int Def => _def;
    public int Dodge => _dodge;

    public Stats(int hp, int atk, int def, int dodge, int immunity, int fortitude, int durability, int clarity) {
        this._hp = hp;
        this._atk = atk;
        this._def = def;
        this._dodge = dodge;
        this._immunity = immunity;
        this._fortitude = fortitude;
        this._durability = durability;
        this._clarity = clarity;
    }
}

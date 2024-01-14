using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class CoordinateConverter
{
    private static Dictionary<(int, int), int> _unityToArrayCoordinates;

    static CoordinateConverter()
    {
        _unityToArrayCoordinates = new Dictionary<(int, int), int>() {
            // 1 ряд
            {(-20,20), 30},
            {(-10,20), 31},
            {(0,20),   32},
            {(10,20),  33},
            {(20,20),  34},
            // 2 ряд
            {(-20,10), 25},
            {(-10,10), 26},
            {(0,10),   27},
            {(10,10),  28},
            {(20,10),  29},
            // 3 ряд
            {(-20,0), 20},
            {(-10,0), 21},
            {(0,0),   22},
            {(10,0),  23},
            {(20,0),  24},
            // 4 ряд
            {(-20,-10), 15},
            {(-10,-10), 16},
            {(0,-10),   17},
            {(10,-10),  18},
            {(20,-10),  19},
            // 5 ряд
            {(-20,-20), 10},
            {(-10,-20), 11},
            {(0,-20),   12},
            {(10,-20),  13},
            {(20,-20),  14},
            // 6 ряд
            {(-20,-30), 5},
            {(-10,-30), 6},
            {(0,-30),   7},
            {(10,-30),  8},
            {(20,-30),  9},
            // 7 ряд
            {(-20,-40), 0},
            {(-10,-40), 1},
            {(0,-40),   2},
            {(10,-40),  3},
            {(20,-40),  4},
        };
    }

    public static int ConvertCoordinates(int x, int z) {
        return _unityToArrayCoordinates[(x, z)];
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject board;
    private GameObject[] gameBoard;

    void Start()
    {
        gameBoard = new GameObject[35];
        // cells = GameObject.FindGameObjectsWithTag("cell");
    }
}
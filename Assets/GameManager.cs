using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject board;
    public GameObject[,] gameBoard;
    private GameObject redTeam;
    private GameObject blueTeam;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.Find("Countdown").GetComponent<CountdownScript>().timer;
        int cellCount = board.transform.childCount * board.transform.GetChild(1).childCount;
        gameBoard = new GameObject[cellCount / 5, cellCount / 5];

        redTeam = GameObject.Find("Red Beetle 1");
        blueTeam = GameObject.Find("Blue Beetle 1");

        gameBoard[0,0] = redTeam;
        redTeam.transform.position = new Vector3(20, 0, 0);

        gameBoard[2,3] = blueTeam;
        blueTeam.transform.position = new Vector3(-10, 0, 20);

    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;

        if (timer <= 55) {
            redTeam.transform.position = new Vector3(10, 0, 10);
            blueTeam.transform.position = new Vector3(20, 0, 0);
        }
    }
}

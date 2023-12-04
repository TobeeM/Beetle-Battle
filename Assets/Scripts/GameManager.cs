using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject board;
    private GameObject[,] gameBoard;
    private GameObject redTeam;
    private GameObject blueTeam;
    private GameObject currentUnit;
    private GameObject[] cells;

    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameObject[board.transform.GetChild(1).childCount, board.transform.childCount];
        cells = GameObject.FindGameObjectsWithTag("cell");

        redTeam = GameObject.Find("Red Beetle 1");
        blueTeam = GameObject.Find("Blue Beetle 1");

        currentUnit = redTeam;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            currentUnit = GetClickedObject();
            // DyeCells(((int)currentUnit.GetComponentInParent<Transform>().position.z, (int)currentUnit.transform.position.x));
            StartCoroutine(MoveClickedObject(currentUnit, new Vector3(20, 0, 20)));
        
        }
    }

    private GameObject GetClickedObject() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 500f);
        if (hit.transform.gameObject.tag == "unit") return hit.transform.gameObject;
        if (hit.transform.gameObject.tag == "cell") return hit.transform.gameObject;
        return null;
    }

    private IEnumerator MoveClickedObject(GameObject clickedObject, Vector3 endPosition) {
        if (moving) yield break;
        moving = true;
        Vector3 startPosition = clickedObject.transform.position;
        float percComplete = 0f;
        float duration = 2f;
        while (percComplete < 1) {
            percComplete += Time.deltaTime / duration;
            clickedObject.transform.position = Vector3.Lerp(startPosition, endPosition, percComplete);
            yield return null;
        }
        moving = false;
    }

    private void DyeCells(ValueTuple<int, int> currentPosition) {
        Debug.Log(GetCellPosition(currentPosition));
        foreach (var cell in cells) {
            if (cell.name.Substring(1, 1) == GetCellPosition(currentPosition).Item2.ToString()) cell.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    private ValueTuple<int, int> GetCellPosition(ValueTuple<int, int> currentPosition) {
        var positionDict = new Dictionary<(int, int), (int, int)>() {
            // 1 ряд
            {(20,-20), (0,0)},
            {(20,-10), (0,1)},
            {(20,0), (0,2)},
            {(20,10), (0,3)},
            {(20,20), (0,4)},
            // 2 ряд
            {(10,-20), (1,0)},
            {(10,-10), (1,1)},
            {(10,0), (1,2)},
            {(10,10), (1,3)},
            {(10,20), (1,4)},
            // 3 ряд
            {(0,-20), (2,0)},
            {(0,-10), (2,1)},
            {(0,0), (2,2)},
            {(0,10), (2,3)},
            {(0,20), (2,4)},
            // 4 ряд
            {(-10,-20), (3,0)},
            {(-10,-10), (3,1)},
            {(-10,0), (3,2)},
            {(-10,10), (3,3)},
            {(-10,20), (3,4)},
            // 5 ряд
            {(-20,-20), (4,0)},
            {(-20,-10), (4,1)},
            {(-20,0), (4,2)},
            {(-20,10), (4,3)},
            {(-20,20), (4,4)},
        };
        return positionDict[currentPosition];
    }
}

/*
    cell.GetComponent<Renderer>().material.color = Color.blue; --> dye a material
    clickedObject.transform.position += new Vector3(0, 0, 10); --> 1st way of moving objects
*/

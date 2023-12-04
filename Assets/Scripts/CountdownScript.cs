using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CountdownScript : MonoBehaviour
{
    public TextMeshProUGUI countdown;
    public float timer = 60;

    void Start()
    {
        countdown.text = "Оставшееся время: 60";
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;
        countdown.text = "Оставшееся время: " + Math.Round(timer);
    }
}

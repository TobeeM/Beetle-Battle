using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this; 
    }

    void Start() {
        UpdateGameState(GameState.PlayerTurn);
    }

    public void UpdateGameState(GameState newState) {
        State = newState;

        switch (newState) {
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState {
    PlayerTurn,
    EnemyTurn
}
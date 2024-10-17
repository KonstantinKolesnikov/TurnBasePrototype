using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.GenerateGrid);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        OnGameStateChange?.Invoke(state);
    }
}

public enum GameState
{
    GenerateGrid,
    AttachToGrid,
    EnemySelect,
    PlayerSelect,
    FastPhase,
    SlowPhase
}

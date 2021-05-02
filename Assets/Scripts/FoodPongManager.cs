using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR;
using Vuforia;

public class FoodPongManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameBallPrefab;
    [SerializeField]
    private Transform gameBallSpawn;
    [SerializeField]
    private UnityEvent OnPlayerHasLost;
    [SerializeField]
    private UnityEvent OnPlayerHasWon;
    [SerializeField]
    private UnityEvent OnGameStarted;
    [SerializeField]
    private UnityEvent OnGamePaused;
    [SerializeField]
    private UnityEvent OnGameContinue;
    [SerializeField]
    private UnityEvent OnGamePlaced;
    [SerializeField]
    private UnityEvent OnGameExit;
    [SerializeField]
    private UnityEvent<int, int> OnUpdatePlayersLife;
    [SerializeField]
    private PlaneFinderBehaviour planeFinderBehaviour;
    [SerializeField]
    private GameObject groundPlaneStage;

    private int _playerLife;
    private int _enemyLife;
    private GameObject _gameBallObject;
    private GameBall _gameBall;
    private GameState _currentGameState = GameState.NotStarted;
    
    public void StartGame()
    {
        _playerLife = 3;
        _enemyLife = 3;
        _currentGameState = GameState.Started;
        _gameBallObject = Instantiate(gameBallPrefab, gameBallSpawn);
        _gameBall = _gameBallObject.GetComponent<GameBall>();
        
        if (_gameBall.OnPlayerGoalHit == null)
            _gameBall.OnPlayerGoalHit = new UnityEvent();
        
        if (_gameBall.OnEnemyGoalHit == null)
            _gameBall.OnEnemyGoalHit = new UnityEvent();
        
        _gameBall.OnPlayerGoalHit.RemoveListener(RegisterPlayerGoalHit);
        _gameBall.OnEnemyGoalHit.RemoveListener(RegisterEnemyGoalHit);
        _gameBall.OnPlayerGoalHit.AddListener(RegisterPlayerGoalHit);
        _gameBall.OnEnemyGoalHit.AddListener(RegisterEnemyGoalHit);
        OnGameStarted.Invoke();
        OnUpdatePlayersLife.Invoke(3, 3);
        _gameBall.Invoke(nameof(_gameBall.initForce), 3);
    }

    public void RegisterEnemyGoalHit()
    {
        _enemyLife -= 1;
        if (IsGameEnd())
        {
            OnPlayerHasWon.Invoke(); 
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            Destroy(_gameBallObject);
        }
        else
        {
            _gameBall.Reset(gameBallSpawn);
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            _gameBall.Invoke(nameof(_gameBall.initForce), 3);
        }
    }

    public void RegisterPlayerGoalHit()
    {
        _playerLife -= 1;
        if (IsGameEnd())
        {
            OnPlayerHasLost.Invoke();
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            Destroy(_gameBallObject);
        }
        else
        {
            _gameBall.Reset(gameBallSpawn);
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            _gameBall.Invoke(nameof(_gameBall.initForce), 3);
        }
    }

    public void ExitGame()
    {
        MeshRenderer[] meshRenderers = groundPlaneStage.GetComponentsInChildren<MeshRenderer>();
        Collider[] colliders = groundPlaneStage.GetComponentsInChildren<Collider>();

        foreach (var currentCollider in colliders)
        {
            currentCollider.enabled = false;
        }
        
        foreach (var currentMeshRenderer in meshRenderers)
        {
            currentMeshRenderer.enabled = false;
        }
        Destroy(_gameBallObject);
        OnGameExit.Invoke();
    }

    public void PlaceGame()
    {
        planeFinderBehaviour.PerformHitTest(new Vector2(0,0));
        OnGamePlaced.Invoke();
    }
    
    public void Pause()
    {
        OnGamePaused.Invoke();
        _gameBall.Pause();
    }
    
    public void Continue()
    {
        OnGameContinue.Invoke();
        _gameBall.Continue();
    }

    private bool IsGameEnd()
    {
        if (_playerLife == 0)
        {
            _currentGameState = GameState.PlayerHasLost;
            return true;
        }
        
        if (_enemyLife == 0)
        {
            _currentGameState = GameState.PlayerHasWon;
            return true;
        }

        return false;
    }
}

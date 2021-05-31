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

/// <summary>
/// This class is responsible for managing the FoodPong game.
/// </summary>
/// <remarks>
/// Author: Felix Riehm
/// </remarks>
public class FoodPongManager : MonoBehaviour
{
    /// <summary>
    /// Throw strength for throwing the throwable object (muffin)
    /// </summary>
    [SerializeField]
    private float throwStrength = 4;
    /// <summary>
    /// Countdown time before ech round.
    /// </summary>
    [SerializeField]
    private int countdownTime = 3;
    /// <summary>
    /// Game ball prefab (donut).
    /// </summary>
    [SerializeField]
    private GameObject gameBallPrefab;
    /// <summary>
    /// Game ball spawn.
    /// </summary>
    [SerializeField]
    private Transform gameBallSpawn;
    /// <summary>
    /// Enemy prefab.
    /// </summary>
    [SerializeField]
    private GameObject enemyPrefab;
    /// <summary>
    /// Enemy spawn.
    /// </summary>
    [SerializeField]
    private Transform enemySpawn;
    /// <summary>
    /// Rescue point where the game ball will be placed when the game ball is stuck.
    /// </summary>
    [SerializeField]
    private Transform rescuePoint;
    /// <summary>
    /// Throwable object prefab.
    /// </summary>
    [SerializeField]
    private GameObject throwableObstaclePrefab;
    /// <summary>
    /// Throwable object spawn.
    /// </summary>
    [SerializeField]
    private Transform throwableObstacleSpawn;
    
    /// Game events
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
    /// <summary>
    /// Will be called when the life a the player or enemy changes. First value of the event arguments is the enemy life.
    /// Second value of the event arguments is player life.
    /// </summary>
    [SerializeField]
    private UnityEvent<int, int> OnUpdatePlayersLife;
    /// <summary>
    /// Will be called at each countdown timer tick. First value of the event arguments is the remaining time in seconds.
    /// </summary>
    [SerializeField]
    private UnityEvent<int> OnUpdateCountdown;
    [SerializeField]
    private UnityEvent OnThrowObstacle;
    [SerializeField]
    private UnityEvent OnResetThrowableObstacle;
    
    /// <summary>
    /// Vuforia plane finder behaviour.
    /// </summary>
    [SerializeField]
    private PlaneFinderBehaviour planeFinderBehaviour;
    /// <summary>
    /// Vuforia ground plane stage.
    /// </summary>
    [SerializeField]
    private GameObject groundPlaneStage;

    private int _playerLife;
    private int _enemyLife;
    private GameObject _gameBallObject;
    private GameBall _gameBall;
    private GameObject _enemyObject;
    private GameObject _throwableObstacleObject;
    private AIPlayer _aiPlayer;
    private GameState _currentGameState = GameState.NotStarted;
    private GameObject _tmpThrowableObstacleObject;

    /// <summary>Describes the behavior of the timer which is started at the start of every round.</summary>
    /// <returns>IEnumerator for a coroutine.</returns>
    IEnumerator CountdownToStart()
    {
        int countdownTimer = countdownTime;
        
        while (countdownTimer > 0 )
        {
            OnUpdateCountdown.Invoke(countdownTimer);
            yield return new WaitForSeconds(1f);
            countdownTimer--;
        }

        OnUpdateCountdown.Invoke(0);
        _gameBall.initForce();
    }
    
    /// <summary>Starts the game.</summary>
    /// <returns>Void.</returns>
    public void StartGame()
    {
        // reset game state
        _playerLife = 3;
        _enemyLife = 3;
        _currentGameState = GameState.Started;
        // spawn game ball
        _gameBallObject = Instantiate(gameBallPrefab, gameBallSpawn);
        _gameBall = _gameBallObject.GetComponent<GameBall>();
        // spawn ai player
        _enemyObject = Instantiate(enemyPrefab, enemySpawn);
        _aiPlayer = _enemyObject.GetComponent<AIPlayer>();
        // spawn throwable obstacle (muffin)
        _throwableObstacleObject = Instantiate(throwableObstaclePrefab, throwableObstacleSpawn);
        _throwableObstacleObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        
        // manage events
        if (_gameBall.OnPlayerGoalHit == null)
            _gameBall.OnPlayerGoalHit = new UnityEvent();
        
        if (_gameBall.OnEnemyGoalHit == null)
            _gameBall.OnEnemyGoalHit = new UnityEvent();
        
        if (_gameBall.OnThrowableObstacleHit == null)
            _gameBall.OnThrowableObstacleHit = new UnityEvent();
        
        _gameBall.OnPlayerGoalHit.RemoveListener(RegisterPlayerGoalHit);
        _gameBall.OnEnemyGoalHit.RemoveListener(RegisterEnemyGoalHit);
        _gameBall.OnThrowableObstacleHit.RemoveListener(ResetThrowableObstacle);
        _gameBall.OnPlayerGoalHit.AddListener(RegisterPlayerGoalHit);
        _gameBall.OnEnemyGoalHit.AddListener(RegisterEnemyGoalHit);
        _gameBall.OnThrowableObstacleHit.AddListener(ResetThrowableObstacle);
        
        // set rescue point if game ball is stuck
        _gameBall.SetRescuePoint(rescuePoint);
        // reset throwable obstacle
        ResetThrowableObstacle();
        // start AI
        _aiPlayer.StartAI(_gameBall.transform);
        // update UI
        OnGameStarted.Invoke();
        OnUpdatePlayersLife.Invoke(3, 3);
        // start timer at beginning
        StartCoroutine(CountdownToStart());
    }

    /// <summary>Registers that the enemy goal is hit. Game ends or next round begins.</summary>
    /// <returns>Void.</returns>
    public void RegisterEnemyGoalHit()
    {
        // decrease enemy life
        _enemyLife -= 1;
        // game ends
        if (IsGameEnd())
        {
            // update UI
            OnPlayerHasWon.Invoke(); 
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            // clean up
            Destroy(_enemyObject);
            Destroy(_gameBallObject);
        }
        else
        {
            // new round begins
            _gameBall.Reset(gameBallSpawn);
            _aiPlayer.ResetPosition();
            // update UI
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            // start timer
            StartCoroutine(CountdownToStart());
        }
    }

    /// <summary>Registers that the player goal is hit. Game ends or next round begins.</summary>
    /// <returns>Void.</returns>
    public void RegisterPlayerGoalHit()
    {
        // decrease player life
        _playerLife -= 1;
        // game ends
        if (IsGameEnd())
        {
            // update UI
            OnPlayerHasLost.Invoke();
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            // clean up
            Destroy(_enemyObject);
            Destroy(_gameBallObject);
        }
        else
        {
            // new round begins
            _gameBall.Reset(gameBallSpawn);
            _aiPlayer.ResetPosition();
            // update UI
            OnUpdatePlayersLife.Invoke(_enemyLife, _playerLife);
            // start timer
            StartCoroutine(CountdownToStart());
        }
    }

    /// <summary>This function is called when the user presses "Exit" on the UI which will end the currently running game.</summary>
    /// <returns>Void.</returns>
    public void ExitGame()
    {
        // hide all game objects which are attached to the ground plane
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
        
        // clean up
        Destroy(_enemyObject);
        Destroy(_gameBallObject);
        Destroy(_throwableObstacleObject);
        if (_tmpThrowableObstacleObject != null)
        {
            Destroy(_tmpThrowableObstacleObject);
        }
        // update UI
        OnGameExit.Invoke();
    }

    /// <summary>This function is called when the user presses "Place game" on the UI which will place the game on the surface provided by Vuforia.</summary>
    /// <returns>Void.</returns>
    public void PlaceGame()
    {
        // call Vuforia function
        planeFinderBehaviour.PerformHitTest(new Vector2(0,0));
        // update UI
        OnGamePlaced.Invoke();
    }

    /// <summary>Resets the throwable object (muffin) to its original position.</summary>
    /// <returns>Void.</returns>
    public void ResetThrowableObstacle()
    {
        _throwableObstacleObject.SetActive(true);
        if (_tmpThrowableObstacleObject != null)
        {
            Destroy(_tmpThrowableObstacleObject);
        }
        OnResetThrowableObstacle.Invoke();
    }
    
    /// <summary>Throws the throwable obstacle. This is done by hiding the original obstacle. A copy will be thrown.</summary>
    /// <returns>Void.</returns>
    public void ThrowObstacle()
    {
        // hide throwable object
        _throwableObstacleObject.SetActive(false);
        if (_tmpThrowableObstacleObject == null)
        {
            // make a copy of it
            _tmpThrowableObstacleObject = Instantiate(throwableObstaclePrefab, throwableObstacleSpawn.position,
                throwableObstacleSpawn.transform.rotation ,groundPlaneStage.transform);
        }
        
        // throw the copy with force
        _tmpThrowableObstacleObject.GetComponent<Rigidbody>().AddForce(_tmpThrowableObstacleObject.transform.forward * throwStrength, ForceMode.Impulse);
        // update UI
        OnThrowObstacle.Invoke();
    }
    
    /// <summary>Pauses the game.</summary>
    /// <returns>Void.</returns>
    public void Pause()
    {
        // update UI
        OnGamePaused.Invoke();
        // pause all other objects
        _aiPlayer.Pause();
        _gameBall.Pause();
    }
    
    /// <summary>Continues the game.</summary>
    /// <returns>Void.</returns>
    public void Continue()
    {
        // update UI
        OnGameContinue.Invoke();
        // continues all other objects
        _aiPlayer.Continue();
        _gameBall.Continue();
    }

    /// <summary>Checks if the game ended and sets the corresponding game state.</summary>
    /// <returns>Returns true if the game has ended. False if not.</returns>
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

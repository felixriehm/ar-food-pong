using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for controlling an AI player.
/// </summary>
/// <remarks>
/// Author: Felix Riehm
/// </remarks>
public class AIPlayer : MonoBehaviour
{
    /// <summary>
    /// Speed of the AI player.
    /// </summary>
    [SerializeField]
    private float speed = 1;
    /// <summary>
    /// Transform of the game ball. This is needed to track the game ball.
    /// </summary>
    private Transform _gameBallTransform;
    /// <summary>
    /// Enables or disables the AI player.
    /// </summary>
    private bool _enabled = false;
    /// <summary>
    /// Step the AI player moves per frame.
    /// </summary>
    private float _step;
    /// <summary>
    /// Current local position of the AI player.
    /// </summary>
    private Vector3 _localPosition;
    /// <summary>
    /// Local x coordinate of the AI player. Needed to compare with the game ball x coordinate.
    /// </summary>
    private float _positionX;
    /// <summary>
    /// Local x coordinate of the game ball. Needed to compare with the AI player x coordinate.
    /// </summary>
    private float _gameBallPositionX;
    /// <summary>
    /// Start position of the AI player. Needed to be able to reset the AI player.
    /// </summary>
    private Vector3 _startPosition;
    /// <summary>
    /// Start rotation of the AI player. Needed to be able to reset the AI player.
    /// </summary>
    private Quaternion _startRotation;
    /// <summary>
    /// Next x coordinate where the AI player will move.
    /// </summary>
    private float _nextX;

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (_enabled)
        {
            _step = speed * Time.deltaTime;
            _localPosition = transform.localPosition;
            _gameBallPositionX = transform.InverseTransformPoint(_gameBallTransform.position).x;
            _positionX = transform.localPosition.x;

            // move AI player to left hand side
            if (_gameBallPositionX > _positionX)
            {
                _nextX = _localPosition.x + _step;
                // if calculated next ai player position over shoots the game ball position, stay
                if (_nextX > _gameBallPositionX)
                {
                    _nextX = _gameBallPositionX;
                }
                // only change the x coordinate 
                transform.localPosition = new Vector3(_nextX, _localPosition.y, _localPosition.z);
            }
            // move AI player to right hand side
            if (_gameBallPositionX < _positionX)
            {
                _nextX = _localPosition.x - _step;
                // if calculated next ai player position over shoots the game ball position, stay
                if (_nextX < _gameBallPositionX)
                {
                    _nextX = _gameBallPositionX;
                }
                // only change the x coordinate 
                transform.localPosition = new Vector3(_nextX, _localPosition.y, _localPosition.z);
            }
        }
    }

    /// <summary>Enables the AI player. Has to be called after initiation.</summary>
    /// <param name="gameBallTransform">Transform of the game ball. This is needed to track the game ball.</param>
    /// <returns>Void</returns>
    public void StartAI(Transform gameBallTransform)
    {
        _gameBallTransform = gameBallTransform;
        _enabled = true;
    }

    /// <summary>Pauses the game.</summary>
    /// <returns>Void</returns>
    public void Pause()
    {
        _enabled = false;
    }
    
    /// <summary>Let the game continue.</summary>
    /// <returns>Void</returns>
    public void Continue()
    {
        _enabled = true;
    }

    /// <summary>Resets the position of the AI player.</summary>
    /// <returns>Void</returns>
    public void ResetPosition()
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
    }
}

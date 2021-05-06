using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;
    
    private Transform _gameBallTransform;
    private bool _enabled = false;
    private float _step;
    private Vector3 _localPosition;
    private float _positionX;
    private float _gameBallPositionX;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
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

            if (_gameBallPositionX > _positionX)
            {
                _nextX = _localPosition.x + _step;
                if (_nextX > _gameBallPositionX)
                {
                    _nextX = _gameBallPositionX;
                }
                transform.localPosition = new Vector3(_nextX, _localPosition.y, _localPosition.z);
            }
            
            if (_gameBallPositionX < _positionX)
            {
                _nextX = _localPosition.x - _step;
                if (_nextX < _gameBallPositionX)
                {
                    _nextX = _gameBallPositionX;
                }
                transform.localPosition = new Vector3(_nextX, _localPosition.y, _localPosition.z);
            }
        }
    }

    public void StartAI(Transform gameBallTransform)
    {
        _gameBallTransform = gameBallTransform;
        _enabled = true;
    }

    public void Pause()
    {
        _enabled = false;
    }
    
    public void Continue()
    {
        _enabled = true;
    }

    public void ResetPosition()
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
    }
}

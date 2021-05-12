using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameBall : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    public UnityEvent OnPlayerGoalHit { get; set; }
    public UnityEvent OnEnemyGoalHit { get; set; }
    public UnityEvent OnThrowableObstacleHit { get; set; }
    private Vector3 _targetLocal;
    private Vector3 _lastTargetLocal;
    private Vector3 _direction;
    private bool _move = false;
    private float _step;
    private int _bounceLayerMask;

    private void Start()
    {
        _bounceLayerMask = LayerMask.GetMask("Bounce");
        _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
    }

    public void initForce()
    {
        float xDirection = Random.Range(-1.0f, 1.0f);
        float zDirection = Random.Range(0.4f, 1f);
        _direction = new Vector3(xDirection, 0, zDirection);

        RaycastHit hit;
        Ray ray = new Ray(transform.position, _direction);
        if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("hit");
            Debug.Log(hit.collider.name);
            _targetLocal = transform.parent.InverseTransformPoint(hit.point);
            _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
        }
        
        _move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_move)
        {
            _step = speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.TransformPoint(_targetLocal), _step);
        }
        
    }

    public void Reset(Transform transform)
    {
        _move = false;
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public void Pause()
    {
        _move = false;
    }
    
    public void Continue()
    {
        _move = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.parent.TransformPoint(_targetLocal),0.01f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.parent.TransformPoint(_targetLocal));
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.parent.TransformPoint(_lastTargetLocal),0.01f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerGoal"))
        {
            OnPlayerGoalHit.Invoke();;
        }

        if (other.CompareTag("EnemyGoal"))
        {
            OnEnemyGoalHit.Invoke();
        }

        if (other.CompareTag("Bounce"))
        {
            Debug.Log("Trigger");
            Debug.Log(other.name);
            RaycastHit hit;
            Ray ray = new Ray(transform.parent.TransformPoint(_lastTargetLocal), _direction);
            if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask ,QueryTriggerInteraction.Ignore))
            {
                _direction = Vector3.Reflect(_direction, hit.normal);
                _direction = new Vector3(_direction.x, 0f, _direction.z);
                CalculateNextTarget();
            }
        }
        
        if (other.CompareTag("ThrowableObstacle"))
        {
            _direction = new Vector3(-1 * _direction.x, -1 * _direction.y, -1 * _direction.z);
            CalculateNextTarget();
            OnThrowableObstacleHit.Invoke();
        }
    }

    private void CalculateNextTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, _direction);
        if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask ,QueryTriggerInteraction.Ignore))
        {
            Debug.Log("next: " + hit.collider.name);

            _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
            _targetLocal = transform.parent.InverseTransformPoint(hit.point);
            Debug.Log("_lastTargetLocal: " + _lastTargetLocal);
            Debug.Log("_targetLocal: " + _targetLocal);
            //_target = new Vector3(hit.point.x, 0 , hit.point.z);
            //_target = (transform.position + direction) * 10f;
        }
    }
}

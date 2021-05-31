using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// This class is responsible for controlling an game ball (donut).
/// </summary>
/// <remarks>
/// Author: Felix Riehm
/// </remarks>
public class GameBall : MonoBehaviour
{
    /// <summary>
    /// Speed of the game ball
    /// </summary>
    [SerializeField]
    private float speed = 10;
    /// <summary>
    /// Event which fires when the player goal is hit.
    /// </summary>
    public UnityEvent OnPlayerGoalHit { get; set; }
    /// <summary>
    /// Event which fires when the enemy goal is hit.
    /// </summary>
    public UnityEvent OnEnemyGoalHit { get; set; }
    /// <summary>
    /// Event which fires when the throwable obstacle is hit.
    /// </summary>
    public UnityEvent OnThrowableObstacleHit { get; set; }
    /// <summary>
    /// Current target.
    /// </summary>
    private Vector3 _targetLocal;
    /// <summary>
    /// Last target.
    /// </summary>
    private Vector3 _lastTargetLocal;
    /// <summary>
    /// Current direction.
    /// </summary>
    private Vector3 _direction;
    /// <summary>
    /// Indicates if the game ball should move.
    /// </summary>
    private bool _move = false;
    /// <summary>
    /// Step which the game ball will travel each frame.
    /// </summary>
    private float _step;
    /// <summary>
    /// Layer mask of "Bounce" which is needed so raycasts only hits objects with the layer name "Bounce"
    /// </summary>
    private int _bounceLayerMask;
    /// <summary>
    /// Rescue point for the game ball when the game ball is stuck.
    /// </summary>
    private Transform _rescuePoint;

    private void Start()
    {
        _bounceLayerMask = LayerMask.GetMask("Bounce");
        _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
    }

    /// <summary>Lets the game ball move in a random direction faced at the enemy. Called when a round begins.</summary>
    /// <returns>Void.</returns>
    public void initForce()
    {
        // Calculate a random direction which faces the enemy
        float xDirection = Random.Range(-1.0f, 1.0f);
        float zDirection = Random.Range(0.4f, 1f);
        _direction = new Vector3(xDirection, 0, zDirection);

        // Raycast to determine the next target location
        RaycastHit hit;
        Ray ray = new Ray(transform.position, _direction);
        if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask, QueryTriggerInteraction.Ignore))
        {
            _targetLocal = transform.parent.InverseTransformPoint(hit.point);
            _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
        }
        
        // now the game ball can move
        _move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_move)
        {
            // If the game ball reached the target location, set new target to rescue point.
            // Usually this never happens because a trigger event is called with the 
            // game ball collider before the origin of the game ball hits the target location.
            if (transform.position == transform.parent.TransformPoint(_targetLocal))
            {
                _targetLocal = transform.parent.InverseTransformPoint(_rescuePoint.position);
            }

            // move the game ball
            _step = speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.TransformPoint(_targetLocal), _step);
        }
        
    }

    /// <summary>Sets the rescue point of the game ball.</summary>
    /// <returns>Void.</returns>
    public void SetRescuePoint(Transform rescuePoint)
    {
        _rescuePoint = rescuePoint;
    }
    
    /// <summary>Resets the game ball.</summary>
    /// <param name="transform">Transform of the point where the game ball should be reset.</param>
    /// <returns>Void.</returns>
    public void Reset(Transform transform)
    {
        _move = false;
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    /// <summary>Pauses the game ball.</summary>
    /// <returns>Void.</returns>
    public void Pause()
    {
        _move = false;
    }
    
    /// <summary>Continues the game ball.</summary>
    /// <returns>Void.</returns>
    public void Continue()
    {
        _move = true;
    }

    /// <summary>Draws debug information as gizmos. A red sphere where the game ball is heading,
    /// a white line on which the game ball travels and a blue sphere where the game balls last target was.</summary>
    /// <returns>Void.</returns>
    private void OnDrawGizmos()
    {
        // target
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.parent.TransformPoint(_targetLocal),0.01f);
        // direction
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.parent.TransformPoint(_targetLocal));
        // last target
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.parent.TransformPoint(_lastTargetLocal),0.01f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // if the game ball hits the player goal
        if (other.CompareTag("PlayerGoal"))
        {
            OnPlayerGoalHit.Invoke();;
        }

        // if the game ball hits the enemy goal
        if (other.CompareTag("EnemyGoal"))
        {
            OnEnemyGoalHit.Invoke();
        }

        // if the game ball hits the a wall, enemy or player
        if (other.CompareTag("Bounce"))
        {
            //Debug.Log("Trigger");
            //Debug.Log(other.name);
            // get the position where the game ball would hit the object
            RaycastHit hit;
            Ray ray = new Ray(transform.parent.TransformPoint(_lastTargetLocal), _direction);
            if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask ,QueryTriggerInteraction.Ignore))
            {
                // now reflect the current direction from the surface the game ball will hit and save it as new direction
                _direction = Vector3.Reflect(_direction, hit.normal);
                _direction = new Vector3(_direction.x, 0f, _direction.z);
                // with the new direction calculate a new target
                CalculateNextTarget();
            }
        }
        
        // if the game ball hits the throwable object (muffin) just travel in the other direction it came from
        if (other.CompareTag("ThrowableObstacle"))
        {
            _direction = new Vector3(-1 * _direction.x, -1 * _direction.y, -1 * _direction.z);
            CalculateNextTarget();
            OnThrowableObstacleHit.Invoke();
        }
    }

    /// <summary>Calculates the next target based on the current direction and sets the target internally.</summary>
    /// <returns>Void.</returns>
    private void CalculateNextTarget()
    {
        // Raycast to get the next target based on the _direction
        RaycastHit hit;
        Ray ray = new Ray(transform.position, _direction);
        if (Physics.Raycast(ray, out hit,10f, _bounceLayerMask ,QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("next: " + hit.collider.name);
            _lastTargetLocal = transform.parent.InverseTransformPoint(transform.position);
            _targetLocal = transform.parent.InverseTransformPoint(hit.point);
        }
    }
}

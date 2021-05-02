using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameBall : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    public UnityEvent OnPlayerGoalHit { get; set; }
    public UnityEvent OnEnemyGoalHit { get; set; }
    private Vector3 _targetLocal;
    private Vector3 direction;
    private bool move = false;
    private float _step;

    public void initForce()
    {
        float xDirection = 1;//Random.Range(0.0f, 1.0f);
        float zDirection = 0.7f;//Random.Range(-1.0f, 1.0f);
        direction = new Vector3(xDirection, 0, zDirection);

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("Bounce");
        Debug.Log(layer_mask);
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out hit,10f,layer_mask,QueryTriggerInteraction.Ignore))
        {
            Debug.Log("hit");
            Debug.Log(hit.collider.name);
            _targetLocal = transform.InverseTransformPoint(hit.point);
        }
        
        move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        { 
            _step = speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, transform.TransformPoint(_targetLocal), _step);
        }
        
    }

    public void Reset(Transform transform)
    {
        move = false;
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    public void Pause()
    {
        move = false;
    }
    
    public void Continue()
    {
        move = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(_targetLocal),0.01f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(_targetLocal));
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
            int layer_mask = LayerMask.GetMask("Bounce");
            Ray ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out hit,10f, layer_mask ,QueryTriggerInteraction.Ignore))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                direction = new Vector3(direction.x, 0f, direction.z);

                ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out hit,10f, layer_mask ,QueryTriggerInteraction.Ignore))
                {
                    Debug.Log("next: " + hit.collider.name);
                    
                    _targetLocal = transform.InverseTransformPoint(hit.point);
                    //_target = new Vector3(hit.point.x, 0 , hit.point.z);
                    //_target = (transform.position + direction) * 10f;
                }
            }
        }
    }
}

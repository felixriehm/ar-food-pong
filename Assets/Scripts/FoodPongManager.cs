using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using Vuforia;

public class FoodPongManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameBall;
    [SerializeField]
    private Transform gameBallSpawn;

    private GameObject tesst;

    private bool test = false;

    // Update is called once per frame
    void Update()
    {
        /*if (test)
        {
            int startForce = 2000;
            // change this to -1.0f, 1.0f for it to also fire the game ball to the player sometimes
            float xDirection = 1;//Random.Range(0.0f, 1.0f);
            float yDirection = 1;//Random.Range(-1.0f, 1.0f);
            Debug.Log("asdasd");
            test = true;
            tesst.GetComponent<Rigidbody>().AddForce(xDirection * startForce,0,yDirection * startForce);
        }*/
    }

    private void StartGame()
    {
        /*float startForce = 1.4f;
        // change this to -1.0f, 1.0f for it to also fire the game ball to the player sometimes
        float xDirection = 1;//Random.Range(0.0f, 1.0f);
        float yDirection = 0.7f;//Random.Range(-1.0f, 1.0f);
        Debug.Log("asdasd");
        test = true;
        //gameBall.GetComponent<Rigidbody>().MovePosition(new Vector3(200,200,200));
        //tesst.GetComponent<Rigidbody>().AddForce(transform.forward * 200 ,ForceMode.Impulse);
        //gameBall.GetComponent<Rigidbody>().AddExplosionForce(222, Vector3.left, 22);
        tesst.GetComponent<Rigidbody>().AddForce(xDirection * startForce,0,yDirection * startForce, ForceMode.Impulse);*/
        tesst.GetComponent<BounceBall>().initForce();
    }

    public void PlaceGameBall()
    {
        tesst = Instantiate(gameBall, gameBallSpawn);
        Invoke("StartGame", 5);
    }
}

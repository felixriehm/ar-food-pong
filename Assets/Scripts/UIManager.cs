using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnPauseGameClick;
    [SerializeField]
    private UnityEvent OnStartGameClick;
    [SerializeField]
    private UnityEvent OnExitGameClick;
    [SerializeField]
    private UnityEvent OnContinueGameClick;
    [SerializeField]
    private UnityEvent OnPlaceGameClick;
    [SerializeField]
    private UnityEvent OnResetThrowableObstacleClick;
    [SerializeField]
    private UnityEvent OnThrowObstacleClick;

    
    private GameObject gameUI;
    private GameObject gameStats;
    private GameObject placeGameButtonObject;
    private Button placeGameButton;
    private TextMeshProUGUI appInfo;
    private TextMeshProUGUI playerLifeValue;
    private TextMeshProUGUI enemyLifeValue;
    private Button startGameButton;
    private TextMeshProUGUI startGameButtonText;
    private Button pauseGameButton;
    private TextMeshProUGUI pauseGameButtonText;
    private Button throwObstacleButton;
    private TextMeshProUGUI throwObstacleButtonText;
    private GameObject playerWonObject;
    private TextMeshProUGUI playerWonText;
    void Start()
    {
        gameUI = GameObject.Find("/UI/Game");
        gameStats = GameObject.Find("/UI/Game/GameStats");
        playerLifeValue = GameObject.Find("/UI/Game/GameStats/PlayerLifeValue").GetComponent<TextMeshProUGUI>();
        enemyLifeValue = GameObject.Find("/UI/Game/GameStats/EnemyLifeValue").GetComponent<TextMeshProUGUI>();
        startGameButton = GameObject.Find("/UI/Game/StartGameButton").GetComponent<Button>();
        startGameButtonText = GameObject.Find("/UI/Game/StartGameButton/StartGameButtonText").GetComponent<TextMeshProUGUI>();
        pauseGameButton = GameObject.Find("/UI/Game/PauseGameButton").GetComponent<Button>();
        pauseGameButtonText = GameObject.Find("/UI/Game/PauseGameButton/PauseGameButtonText").GetComponent<TextMeshProUGUI>();
        throwObstacleButton = GameObject.Find("/UI/Game/ThrowObstacleButton").GetComponent<Button>();
        throwObstacleButtonText = GameObject.Find("/UI/Game/ThrowObstacleButton/ThrowObstacleButtonText").GetComponent<TextMeshProUGUI>();
        playerWonObject = GameObject.Find("/UI/Game/PlayerWon");
        playerWonText = GameObject.Find("/UI/Game/PlayerWon").GetComponent<TextMeshProUGUI>();
        placeGameButtonObject = GameObject.Find("/UI/PlaceTheGameButton");
        placeGameButton = GameObject.Find("/UI/PlaceTheGameButton").GetComponent<Button>();
        appInfo = GameObject.Find("/UI/AppInfo").GetComponent<TextMeshProUGUI>();

        ShowPlaceGameUI();
    }

    public void ShowGameStartedUI()
    {
        placeGameButtonObject.SetActive(false);
        gameStats.SetActive(true);
        
        throwObstacleButton.gameObject.SetActive(true);
        throwObstacleButton.interactable = true;
        throwObstacleButton.onClick.RemoveAllListeners();
        throwObstacleButton.onClick.AddListener(() => OnThrowObstacleClick.Invoke());
        pauseGameButtonText.text = "Pause";
        pauseGameButton.interactable = true;
        pauseGameButton.onClick.RemoveAllListeners();
        pauseGameButton.onClick.AddListener(() => OnPauseGameClick.Invoke());
        startGameButtonText.text = "Exit";
        startGameButton.interactable = true;
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => OnExitGameClick.Invoke());
        playerWonObject.SetActive(false);
        appInfo.text = "Game runs.";
        gameUI.SetActive(true);
    }
    
    
    
    public void ShowGamePausedUI()
    {
        placeGameButtonObject.SetActive(false);
        gameStats.SetActive(true);

        throwObstacleButton.gameObject.SetActive(true);
        throwObstacleButton.interactable = false;
        pauseGameButtonText.text = "Continue";
        pauseGameButton.interactable = true;
        pauseGameButton.onClick.RemoveAllListeners();
        pauseGameButton.onClick.AddListener(() => OnContinueGameClick.Invoke());
        startGameButtonText.text = "Exit";
        startGameButton.interactable = true;
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => OnExitGameClick.Invoke());
        playerWonObject.SetActive(false);
        appInfo.text = "Game paused.";
        gameUI.SetActive(true);
    }
    
    public void ShowPlayerHasLostUI()
    {
        placeGameButtonObject.SetActive(false);
        gameStats.SetActive(true);
        
        throwObstacleButton.gameObject.SetActive(true);
        throwObstacleButton.interactable = false;
        pauseGameButtonText.text = "Exit";
        pauseGameButton.interactable = true;
        pauseGameButton.onClick.RemoveAllListeners();
        pauseGameButton.onClick.AddListener(() => OnExitGameClick.Invoke());
        startGameButtonText.text = "Restart";
        startGameButton.interactable = true;
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => OnStartGameClick.Invoke());
        playerWonObject.SetActive(true);
        playerWonText.text = "You have lost!";
        playerWonText.color = Color.red;
        appInfo.text = "Game ended.";
        gameUI.SetActive(true);
    }
    
    public void ShowPlayerHasWonUI()
    {
        placeGameButtonObject.SetActive(false);
        gameStats.SetActive(true);
        
        throwObstacleButton.gameObject.SetActive(true);
        throwObstacleButton.interactable = false;
        pauseGameButtonText.text = "Exit";
        pauseGameButton.interactable = true;
        pauseGameButton.onClick.RemoveAllListeners();
        pauseGameButton.onClick.AddListener(() => OnExitGameClick.Invoke());
        startGameButtonText.text = "Restart";
        startGameButton.interactable = true;
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => OnStartGameClick.Invoke());
        playerWonObject.SetActive(true);
        playerWonText.text = "You have won!";
        playerWonText.color = Color.green;
        appInfo.text = "Game ended.";
        gameUI.SetActive(true);
    }

    public void UpdatePlayersLife(int enemyLife, int playerLife)
    {
        playerLifeValue.text = playerLife.ToString();
        enemyLifeValue.text = enemyLife.ToString();
        appInfo.text = "Players life changed.";
    }

    public void UpdateCountdown(int countdown)
    {
        playerWonText.color = Color.white;
        playerWonText.text = countdown.ToString();
        if (countdown == 0)
        {
            playerWonObject.SetActive(false);
        }
        else
        {
            playerWonObject.SetActive(true);
        }
    }

    public void ThrowObstacle()
    {
        throwObstacleButtonText.text = "Reset muffin";
        throwObstacleButton.onClick.RemoveAllListeners();
        throwObstacleButton.onClick.AddListener(() => OnResetThrowableObstacleClick.Invoke());
    }
    
    public void ResetThrowObstacleButton()
    {
        throwObstacleButtonText.text = "Throw muffin";
        throwObstacleButton.onClick.RemoveAllListeners();
        throwObstacleButton.onClick.AddListener(() => OnThrowObstacleClick.Invoke());
    }
    
    public void ShowGameNotStartedUI()
    {
        placeGameButtonObject.SetActive(false);
        gameStats.SetActive(false);
       
        throwObstacleButton.gameObject.SetActive(false);
        throwObstacleButton.interactable = true;
        pauseGameButtonText.text = "Pause";
        pauseGameButton.interactable = false;
        pauseGameButton.onClick.RemoveAllListeners();
        pauseGameButton.onClick.AddListener(() => OnPauseGameClick.Invoke());
        startGameButtonText.text = "Start";
        startGameButton.interactable = true;
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(() => OnStartGameClick.Invoke());
        playerWonObject.SetActive(false);
        appInfo.text = "Game not started.";
        gameUI.SetActive(true);
    }

    public void ChangeApplicationInformation(string message)
    {
        appInfo.text = message;
    }

    public void ShowPlaceGameUI()
    {
        placeGameButtonObject.SetActive(true);
        gameStats.SetActive(false);
        placeGameButton.onClick.RemoveAllListeners();
        placeGameButton.onClick.AddListener(() => OnPlaceGameClick.Invoke());
        appInfo.text = "Place game.";
        gameUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

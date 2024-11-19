using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates { countDown, running, raceOver };

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject carPrefab; // Prefab for the car
    public Transform[] spawnPoints; // Array of spawn points (ensure at least 2 points exist)

    private GameStates gameState = GameStates.countDown;
    private float raceStartedTime = 0;
    private float raceCompletedTime = 0;
    private List<DriverInfo> driverInfoList = new List<DriverInfo>();

    public event Action<GameManager> OnGameStateChanged;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
    }

    private void Start()
    {
        // Only initialize drivers if we're not already running
        if (gameState == GameStates.countDown)
            InitializeDrivers();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string selectedCourse = PlayerPrefs.GetString("SelectedCourse", "Course1"); // Default to Course1
        Debug.Log($"Loaded Scene: {scene.name}, Selected Track: {selectedCourse}");

        // Reset game state and initialize the track
        if (scene.name == "Course1" || scene.name == "Course2" || scene.name == "Course3" || scene.name == "Course4" || scene.name == "Course5") // Added Course5
        {
            ResetGameState();
        }
    }

    public GameStates GetGameState()
    {
        return gameState;
    }

    private void ChangeGameState(GameStates newGameState)
    {
        if (gameState != newGameState)
        {
            gameState = newGameState;
            OnGameStateChanged?.Invoke(this);
            Debug.Log($"Game state changed to: {gameState}");
        }
    }

    public float GetRaceTime()
    {
        if (gameState == GameStates.countDown)
            return 0;
        if (gameState == GameStates.raceOver)
            return raceCompletedTime - raceStartedTime;
        return Time.time - raceStartedTime;
    }

    public void ClearDriversList()
    {
        driverInfoList.Clear();
    }

    public void AddDriverToList(int playerNumber, string name, int carUniqueID, bool isAI)
    {
        driverInfoList.Add(new DriverInfo(playerNumber, name, carUniqueID, isAI));
    }

    public void SetDriversLastRacePosition(int playerNumber, int position)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        if (driverInfo != null)
            driverInfo.lastRacePosition = position;
    }

    public void AddPointsToChampionship(int playerNumber, int points)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        if (driverInfo != null)
            driverInfo.championshipPoints += points;
    }

    private DriverInfo FindDriverInfo(int playerNumber)
    {
        foreach (DriverInfo driverInfo in driverInfoList)
        {
            if (playerNumber == driverInfo.playerNumber)
                return driverInfo;
        }

        Debug.LogError($"FindDriverInfo failed for player number {playerNumber}");
        return null;
    }

    public List<DriverInfo> GetDriverList()
    {
        return driverInfoList;
    }

    public void OnRaceStart()
    {
        Debug.Log("OnRaceStart called. Race begins.");
        raceStartedTime = Time.time;
        ChangeGameState(GameStates.running);

        // Instantiate cars at their spawn points
        SpawnCars();
    }

    public void OnRaceCompleted()
    {
        Debug.Log("OnRaceCompleted called. Race over.");
        raceCompletedTime = Time.time;
        ChangeGameState(GameStates.raceOver);
    }

    public void ResetGameState()
    {
        gameState = GameStates.countDown;
        raceStartedTime = 0;
        raceCompletedTime = 0;
        InitializeDrivers();
        StartCoroutine(StartCountdown());
        Debug.Log("Game state reset to countDown.");
    }

    private void InitializeDrivers()
    {
        ClearDriversList();
        AddDriverToList(1, "P1", 0, false); // Initialize Player 1
        AddDriverToList(2, "P2", 1, false); // Initialize Player 2
    }

    private void SpawnCars()
    {
        if (spawnPoints.Length >= 2)
        {
            // Spawn Player 1
            Instantiate(carPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
            // Spawn Player 2
            Instantiate(carPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
        }
        else
        {
            Debug.LogError("Not enough spawn points defined.");
        }
    }

    private IEnumerator StartCountdown()
    {
        Debug.Log("Starting countdown...");
        gameState = GameStates.countDown;

        yield return new WaitForSeconds(3); // Simulate a 3-second countdown

        OnRaceStart();
    }
}


















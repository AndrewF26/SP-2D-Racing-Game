using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;

/**/
/*
    The GameManager class is a central component in managing the overall game state and player data across different levels 
    and scenes in the game. It operates as a singleton, ensuring only one instance exists throughout the game's lifecycle. 
    This class handles various aspects of gameplay, including tracking the game state, managing race timings, player scores, 
    and AI difficulty levels, as well as keeping records of purchased cars and unlocked tracks.  
    GameManager also provides interfaces for other game components to access and modify game-related data, such as player 
    information, points, and game states.
*/
/**/
public enum GameStates { countdown, running, raceOver };

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    GameStates gameState = GameStates.countdown;

    float raceStartedTime = 0;
    float raceFinishedTime = 0;
    private float aiDifficulty = 1.0f;
    private int lastRaceScore = 0;
    public int totalPoints = 0;

    private HashSet<int> purchasedCarIDs = new HashSet<int>();

    private HashSet<int> unlockedTracks = new HashSet<int>();

    List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

    public event Action<GameManager> OnGameStateChanged;

    /**/
    /*
    GameManager::Awake() GameManager::Awake()

    NAME
        GameManager::Awake - Initializes the singleton instance of the GameManager.

    SYNOPSIS
        void GameManager::Awake();

    DESCRIPTION
        This method is responsible for setting up the GameManager instance in order to maintain a consistent game state across 
        different scenes. It ensures that only one instance of the GameManager exists throughout the game using the singleton pattern. 

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) 
        { 
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    /**/
    /*
    GameManager::Start() GameManager::Start()

    NAME
        GameManager::Start - Initializes player information at the start.

    SYNOPSIS
        void GameManager::Start();

    DESCRIPTION
        This method initializes the player information list with default values for testing and initial gameplay setup. 
        This method is adapted to dynamically set player information based on game settings or saved data.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        playerInfoList.Add(new PlayerInfo(1, "Player1", 0, false));
    }

    /**/
    /*
    GameManager::LevelStart() GameManager::LevelStart()

    NAME
        GameManager::LevelStart - Prepares the game state for a new level.

    SYNOPSIS
        void GameManager::LevelStart();

    DESCRIPTION
        This method is called to initialize the game state at the beginning of a new level or race.
        It sets the game state to 'countdown', indicating the preparation phase before the race begins.

    RETURNS
        Nothing.
    */
    /**/
    void LevelStart()
    {
        gameState = GameStates.countdown;
    }
	
	/**/
	/*
    GameManager::GetGameState() GameManager::GetGameState()

    NAME
        GameManager::GetGameState - Retrieves the current game state.

    SYNOPSIS
        GameStates GameManager::GetGameState();

    DESCRIPTION
        This method returns the current state of the game, such as countdown, running, or race over.
        It is used throughout the game to determine the current phase of gameplay and to make decisions
        based on the game's progress.

    RETURNS
        The current state of the game.
    */
	/**/
    public GameStates GetGameState()
    {
        return gameState;
    }

    /**/
    /*
    GameManager::ChangeGameState(GameStates newGameState) GameManager::ChangeGameState(GameStates newGameState)

    NAME
        GameManager::ChangeGameState - Changes the game state to a new state.

    SYNOPSIS
        void GameManager::ChangeGameState(GameStates newGameState);
            newGameState   --> The new state to set the game to.

    DESCRIPTION
        This method changes the game's state to the specified new state. It triggers the OnGameStateChanged event
        if the state has been changed. 

    RETURNS
        Nothing.
    */
    /**/
    void ChangeGameState(GameStates newGameState)
    {
        if (gameState != newGameState)
        {
            gameState = newGameState;

            OnGameStateChanged?.Invoke(this);
        }
    }

    /**/
    /*
    GameManager::OnEnable() GameManager::OnEnable()

    NAME
        GameManager::OnEnable - Event registration when the script is enabled.

    SYNOPSIS
        void GameManager::OnEnable();

    DESCRIPTION
        This method registers the GameManager to listen for the 'sceneLoaded' event from the SceneManager.
        It is used to respond to scene load events, allowing it to initialize or reset game states appropriately.

    RETURNS
        Nothing.
    */
    /**/
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /**/
    /*
    GameManager::AddPoints(int points) GameManager::AddPoints(int points)

    NAME
        GameManager::AddPoints - Adds points to the player's total score.

    SYNOPSIS
        void GameManager::AddPoints(int points);
            points   --> The number of points to add to the total score.

    DESCRIPTION
        This method increases the player's total points by the specified amount. 

    RETURNS
        Nothing.
    */
    /**/
    public void AddPoints(int points)
    {
        totalPoints += points;
    }
	
	/**/
	/*
    GameManager::GetRaceTime() GameManager::GetRaceTime()

    NAME
        GameManager::GetRaceTime - Calculates the elapsed time of the current race.

    SYNOPSIS
        float GameManager::GetRaceTime();

    DESCRIPTION
        This method calculates and returns the elapsed time since the race started. The time calculation
        depends on the current game state: during the countdown or race over, it returns 0, and during the
        race, it calculates the time since the race started.

    RETURNS
        The elapsed time of the current race in seconds.
    */
	/**/
    public float GetRaceTime()
    {
        if (gameState == GameStates.countdown)
        {
            return 0;
        }
        else if (gameState == GameStates.raceOver)
        {
            return raceFinishedTime - raceStartedTime;
        }
        else
        {
            return Time.time - raceStartedTime;
        }
    }

    /**/
    /*
    GameManager::AIDifficulty GameManager::AIDifficulty

    NAME
        GameManager::AIDifficulty - Property for getting and setting AI difficulty.

    SYNOPSIS
        float GameManager::AIDifficulty

    DESCRIPTION
        This property allows getting and setting the difficulty level of AI players in the game.
        The difficulty value is clamped between 0.0 (easiest) and 1.0 (hardest) to ensure it remains
        within a valid range.

    RETURNS
        Float aiDifficulty: The AI difficulty level.
    */
    /**/
    public float AIDifficulty
    {
        get { return aiDifficulty; }
        set { aiDifficulty = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    /**/
    /*
    GameManager::AddPlayerToList(int playerNum, string name, int carUniqueID, bool isAI) GameManager::AddPlayerToList(int playerNum, string name, int carUniqueID, bool isAI)

    NAME
        GameManager::AddPlayerToList - Adds a player to the game's player list.

    SYNOPSIS
        void GameManager::AddPlayerToList(int playerNum, string name, int carUniqueID, bool isAI);
            playerNum     --> The number identifying the player.
            name          --> The name of the player.
            carUniqueID   --> The unique ID of the player's chosen car.
            isAI          --> Indicates whether the player is an AI.

    DESCRIPTION
        This method adds a new player to the game's player list, with details such as player number,
        name, selected car ID, and whether the player is an AI. This information is used throughout the 
        game to manage players' data and states.

    RETURNS
        Nothing.
    */
    /**/
    public void AddPlayerToList(int playerNum, string name, int carUniqueID, bool isAI)
    {
        playerInfoList.Add(new PlayerInfo(playerNum, name, carUniqueID, isAI));
    }

    /**/
    /*
    GameManager::ClearPlayerList() GameManager::ClearPlayerList()

    NAME
        GameManager::ClearPlayerList - Clears the list of player information.

    SYNOPSIS
        void GameManager::ClearPlayerList();

    DESCRIPTION
        This method clears all entries in the player information list. It's typically used during game 
        initialization or when resetting the game state to ensure that outdated player data is removed
        before starting a new game or level.

    RETURNS
        Nothing.
    */
    /**/
    public void ClearPlayerList()
    {
        playerInfoList.Clear();
    }
	
	/**/
	/*
    GameManager::GetPlayerList() GameManager::GetPlayerList()

    NAME
        GameManager::GetPlayerList - Retrieves the list of player information.

    SYNOPSIS
        List<PlayerInfo> GameManager::GetPlayerList();

    DESCRIPTION
        This method returns the current list of players in the game, including their details like player 
        number, name, car ID, and AI status. 

    RETURNS
        The list of players currently in the game.
    */
	/**/
    public List<PlayerInfo> GetPlayerList()
    {
        return playerInfoList;
    }

    /**/
    /*
    GameManager::OnRaceStart() GameManager::OnRaceStart()

    NAME
        GameManager::OnRaceStart - Handles the beginning of a race.

    SYNOPSIS
        void GameManager::OnRaceStart();

    DESCRIPTION
        This method is called to mark the start of a race. It records the start time of the race and
        manages the transition to the 'running' game state. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnRaceStart()
    {
        raceStartedTime = Time.time;

        ChangeGameState(GameStates.running);
    }

    /**/
    /*
    GameManager::UpdatePlayerTopSpeed() GameManager::UpdatePlayerTopSpeed()

    NAME
        GameManager::UpdatePlayerTopSpeed - Updates the top speed record for the player.

    SYNOPSIS
        void GameManager::UpdatePlayerTopSpeed();

    DESCRIPTION
        This method updates the top speed achieved by the player during the race. It retrieves the 
        CarController component of the player's car to access the top speed and updates the corresponding
        player's information in the playerInfoList. 

    RETURNS
        Nothing.
    */
    /**/
    public void UpdatePlayerTopSpeed()
    {
        CarController playerCarController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        if (playerCarController != null)
        {
            PlayerInfo playerInfo = playerInfoList.Find(p => p.playerNum == 1); // Assuming playerNum == 1 is the player
            if (playerInfo != null)
            {
                playerInfo.topSpeed = playerCarController.TopSpeed;
            }
        }
    }

    /**/
    /*
    GameManager::UpdatePlayerRacePosition(int playerNum, int position) GameManager::UpdatePlayerRacePosition(int playerNum, int position)

    NAME
        GameManager::UpdatePlayerRacePosition - Updates the race position of a specific player.

    SYNOPSIS
        void GameManager::UpdatePlayerRacePosition(int playerNum, int position);
            playerNum   --> The number identifying the player.
            position    --> The player's position in the race.

    DESCRIPTION
        This method updates the race position of a player identified by playerNum. It is called typically
        at the end of the race to record the player's final position. 

    RETURNS
        Nothing.
    */
    /**/
    public void UpdatePlayerRacePosition(int playerNum, int position)
    {
        PlayerInfo playerInfo = playerInfoList.Find(p => p.playerNum == playerNum);
        if (playerInfo != null)
        {
            playerInfo.lastRacePosition = position;
        }
    }

    /**/
    /*
    GameManager::OnRaceFinish() GameManager::OnRaceFinish()

    NAME
        GameManager::OnRaceFinish - Handles the end of a race.

    SYNOPSIS
        void GameManager::OnRaceFinish();

    DESCRIPTION
        This method is called when a race is finished. It records the finish time, updates the player's top
        speed, calculates and updates the player's race position, and changes the game state to 'raceOver'.

    RETURNS
        Nothing.
    */
    /**/
    public void OnRaceFinish()
    {
        raceFinishedTime = Time.time;

        UpdatePlayerTopSpeed();

        LapCounter playerLapCounter = FindObjectsOfType<LapCounter>().FirstOrDefault(lc => lc.CompareTag("Player"));
        if (playerLapCounter != null)
        {
            UpdatePlayerRacePosition(1, playerLapCounter.GetCarPosition());
        }

        lastRaceScore = CalculatePlayerScore();
        totalPoints += lastRaceScore;

        ChangeGameState(GameStates.raceOver);

        Debug.Log("Race Finished. Updating Position UI Handlers.");
        UpdateAllPositionUIHandlers();
    }

    /**/
    /*
    GameManager::UpdateAllPositionUIHandlers() GameManager::UpdateAllPositionUIHandlers()

    NAME
        GameManager::UpdateAllPositionUIHandlers - Updates all UI handlers with position information.

    SYNOPSIS
        void GameManager::UpdateAllPositionUIHandlers();

    DESCRIPTION
        This method updates all PositionUIHandler instances in the game with the latest position 
        information of all cars. It sorts the LapCounter instances by their final positions and 
        passes this sorted list to each PositionUIHandler for display. 

    RETURNS
        Nothing.
    */
    /**/
    private void UpdateAllPositionUIHandlers()
    {
        PositionUIHandler[] allHandlers = FindObjectsOfType<PositionUIHandler>();

        // Create a list of LapCounters sorted by their final positions
        List<LapCounter> sortedLapCounters = FindObjectsOfType<LapCounter>()
            .OrderBy(lc => lc.GetCarPosition())
            .ToList();

        // Update each PositionUIHandler with the sorted list
        foreach (var handler in allHandlers)
        {
            handler.UpdateList(sortedLapCounters);
        }
    }

    /**/
    /*
    GameManager::GetLastRaceScore() GameManager::GetLastRaceScore()

    NAME
        GameManager::GetLastRaceScore - Retrieves the score from the last completed race.

    SYNOPSIS
        int GameManager::GetLastRaceScore();

    DESCRIPTION
        This method returns the score achieved by the player in the most recently completed race.
        It is used for displaying post-race results and for any calculations that depend on the 
        player's performance in the last race.

    RETURNS
        Int lastRaceScore: The score from the last race.
    */
    /**/
    public int GetLastRaceScore()
    {
        return lastRaceScore;
    }
	
	/**/
	/*
    GameManager::CalculatePlayerScore() GameManager::CalculatePlayerScore()

    NAME
        GameManager::CalculatePlayerScore - Calculates the player's score based on race performance.

    SYNOPSIS
        private int GameManager::CalculatePlayerScore();

    DESCRIPTION
        This private method calculates the player's score based on various factors like race time,
        position, and top speed. It is typically called at the end of a race to determine the 
        player's score for that race, which can then be used for updating the total points.

    RETURNS
        The calculated score for the player.
    */
	/**/
    private int CalculatePlayerScore()
    {
        // Retrieve the player's info
        PlayerInfo playerInfo = playerInfoList.Find(p => p.playerNum == 1);
        Debug.Log("Player's final position: " + playerInfo.lastRacePosition);

        float raceTime = GetRaceTime();
        int timeScore = Mathf.Max(0, 100 - (int)raceTime); 
        int positionScore = (8 - playerInfo.lastRacePosition) * 10; 
        int speedScore = (int)playerInfo.topSpeed * 1; 

        Debug.Log($"Time Score: {timeScore}, Position Score: {positionScore}, Speed Score: {speedScore}");

        return timeScore + positionScore + speedScore;
    }

    /**/
    /*
    GameManager::MarkCarAsPurchased(int carID) GameManager::MarkCarAsPurchased(int carID)

    NAME
        GameManager::MarkCarAsPurchased - Records the purchase of a car.

    SYNOPSIS
        void GameManager::MarkCarAsPurchased(int carID);
            carID   --> The unique identifier of the car that was purchased.

    DESCRIPTION
        This method marks a car as purchased by adding its unique identifier to the purchasedCarIDs set.
        It is used to track which cars have been purchased by the player.

    RETURNS
        Nothing.
    */
    /**/
    public void MarkCarAsPurchased(int carID)
    {
        purchasedCarIDs.Add(carID);
    }

    /**/
    /*
    GameManager::IsCarPurchased(int carID) GameManager::IsCarPurchased(int carID)

    NAME
        GameManager::IsCarPurchased - Checks if a car has been purchased.

    SYNOPSIS
        bool GameManager::IsCarPurchased(int carID);
            carID   --> The unique identifier of the car.

    DESCRIPTION
        This method checks if a car, identified by its unique ID, has been purchased by the player.
        It returns a boolean value indicating whether the car is in the set of purchasedCarIDs.

    RETURNS
        True if the car has been purchased, False otherwise.
    */
    /**/
    public bool IsCarPurchased(int carID)
    {
        return purchasedCarIDs.Contains(carID);
    }

    /**/
    /*
    GameManager::IsTrackUnlocked(int trackID) GameManager::IsTrackUnlocked(int trackID)

    NAME
        GameManager::IsTrackUnlocked - Checks if a track is unlocked.

    SYNOPSIS
        bool GameManager::IsTrackUnlocked(int trackID);
            trackID   --> The unique identifier of the track.

    DESCRIPTION
        This method checks whether a particular track, identified by its unique ID, has been unlocked
        by the player. It returns a boolean value indicating the unlocked status of the track. 

    RETURNS
        True if the track is unlocked, False otherwise.
    */
    /**/
    public bool IsTrackUnlocked(int trackID)
    {
        return unlockedTracks.Contains(trackID);
    }

    /**/
    /*
    GameManager::UnlockTrack(int trackID) GameManager::UnlockTrack(int trackID)

    NAME
        GameManager::UnlockTrack - Unlocks a track for the player.

    SYNOPSIS
        void GameManager::UnlockTrack(int trackID);
            trackID   --> The unique identifier of the track to be unlocked.

    DESCRIPTION
        This method unlocks a track, identified by its unique ID, for the player. It adds the track ID
        to the set of unlockedTracks, and deducts points from the player's total as a cost for unlocking it. 

    RETURNS
        Nothing.
    */
    /**/
    public void UnlockTrack(int trackID)
    {
        if (!unlockedTracks.Contains(trackID))
        {
            unlockedTracks.Add(trackID);
            totalPoints -= 100; 
        }
    }

    /**/
    /*
    GameManager::OnSceneLoaded(Scene scene, LoadSceneMode mode) GameManager::OnSceneLoaded(Scene scene, LoadSceneMode mode)

    NAME
        GameManager::OnSceneLoaded - Responds to scene loading events.

    SYNOPSIS
        void GameManager::OnSceneLoaded(Scene scene, LoadSceneMode mode);
            scene   --> The loaded scene.
            mode    --> The mode in which the scene was loaded.

    DESCRIPTION
        This method is used to trigger the LevelStart method, ensuring that the game state is set correctly when a new race begins.

    RETURNS
        Nothing.
    */
    /**/
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelStart();
    }
}

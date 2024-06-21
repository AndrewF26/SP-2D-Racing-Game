using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/**/
/*
    This class is responsible for managing the display of race results in the game. It controls a canvas 
    that shows the player's score for the most recent race, their total accumulated score, and provides options
    to navigate to different menus.
*/
/**/
public class ResultUIHandler : MonoBehaviour
{
    Canvas canvas;

    public TMP_Text scoreText;
    public TMP_Text totalScoreText;

    /**/
    /*
    ResultUIHandler::Awake() ResultUIHandler::Awake()

    NAME
        ResultUIHandler::Awake - Initializes the Result UI Handler.

    SYNOPSIS
        private void ResultUIHandler::Awake();

    DESCRIPTION
        This function initializes the canvas component and prepares the UI handler to display the race results 
        correctly once the race is over.

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;

        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }

    /**/
    /*
    ResultUIHandler::OnRaceAgain() ResultUIHandler::OnRaceAgain()

    NAME
        ResultUIHandler::OnRaceAgain - Reloads the current race scene.

    SYNOPSIS
        public void ResultUIHandler::OnRaceAgain();

    DESCRIPTION
        This method is called when the player chooses to race again. It reloads the current active scene, 
        restarting the race.

    RETURNS
        Nothing.
    */
    /**/
    public void OnRaceAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /**/
    /*
    ResultUIHandler::OnCarSelectExit() ResultUIHandler::OnCarSelectExit()

    NAME
        ResultUIHandler::OnCarSelectExit - Navigates to the car selection menu.

    SYNOPSIS
        public void ResultUIHandler::OnCarSelectExit();

    DESCRIPTION
        This method is called when the player decides to exit to the car selection menu after a race.

    RETURNS
        Nothing.
    */
    /**/
    public void OnCarSelectExit()
    {
        SceneManager.LoadScene("Select-Menu");
    }

    /**/
    /*
    ResultUIHandler::OnTrackSelectExit() ResultUIHandler::OnTrackSelectExit()

    NAME
        ResultUIHandler::OnTrackSelectExit - Navigates to the track selection menu.

    SYNOPSIS
        public void ResultUIHandler::OnTrackSelectExit();

    DESCRIPTION
        This function is invoked when the player decides to go to the track selection menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnTrackSelectExit()
    {
        SceneManager.LoadScene("Level-Menu");
    }

    /**/
    /*
    ResultUIHandler::OnMainMenuExit() ResultUIHandler::OnMainMenuExit()

    NAME
        ResultUIHandler::OnMainMenuExit - Returns to the main menu.

    SYNOPSIS
        public void ResultUIHandler::OnMainMenuExit();

    DESCRIPTION
        This method is triggered when the player decides to return to the main menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnMainMenuExit()
    {
        SceneManager.LoadScene("Main-Menu");
    }

    /**/
    /*
    ResultUIHandler::DisplayRaceScore(int raceScore) ResultUIHandler::DisplayRaceScore(int raceScore)

    NAME
        ResultUIHandler::DisplayRaceScore - Displays the score of the most recent race.

    SYNOPSIS
        public void ResultUIHandler::DisplayRaceScore(int raceScore);
            raceScore    --> The score achieved in the most recent race.

    DESCRIPTION
        This method updates the score display to show the score earned in the most recent race.

    RETURNS
        Nothing.
    */
    /**/
    public void DisplayRaceScore(int raceScore)
    {
        if (scoreText != null)
            scoreText.text = raceScore.ToString();
    }

    /**/
    /*
    ResultUIHandler::DisplayTotalScore() ResultUIHandler::DisplayTotalScore()

    NAME
        ResultUIHandler::DisplayTotalScore - Displays the player's total accumulated score.

    SYNOPSIS
        public void ResultUIHandler::DisplayTotalScore();

    DESCRIPTION
        This function retrieves and displays the total points accumulated by the player over multiple races.

    RETURNS
        Nothing.
    */
    /**/
    public void DisplayTotalScore()
    {
        if (totalScoreText != null)
        {
            int totalScore = GameManager.instance.totalPoints;
            totalScoreText.text = totalScore.ToString();
        }
    }

    /**/
    /*
    ResultUIHandler::ShowMenuCO() ResultUIHandler::ShowMenuCO()

    NAME
        ResultUIHandler::ShowMenuCO - Coroutine to display the result menu.

    SYNOPSIS
        IEnumerator ResultUIHandler::ShowMenuCO();

    DESCRIPTION
        This coroutine waits for a set duration before enabling the canvas to display the race results.

    RETURNS
        IEnumerator that temporarily halts execution for a second.
    */
    /**/
    IEnumerator ShowMenuCO()
    {
        yield return new WaitForSeconds(1);

        canvas.enabled = true;
    }

    /**/
    /*
    ResultUIHandler::OnGameStateChanged(GameManager gameManager) ResultUIHandler::OnGameStateChanged(GameManager gameManager)

    NAME
        ResultUIHandler::OnGameStateChanged - Handles changes in the game state.

    SYNOPSIS
        void ResultUIHandler::OnGameStateChanged(GameManager gameManager);
            gameManager    --> Reference to the GameManager instance.

    DESCRIPTION
        This method responds to changes in the game's state. When the race is over, it displays the race score
        and the total score, and initiates the result menu display coroutine.

    RETURNS
        Nothing.
    */
    /**/
    void OnGameStateChanged(GameManager gameManager)
    {
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
        {
            DisplayRaceScore(gameManager.GetLastRaceScore());
            DisplayTotalScore();
            StartCoroutine(ShowMenuCO());
        }
    }

    /**/
    /*
    ResultUIHandler::OnDestroy() ResultUIHandler::OnDestroy()

    NAME
        ResultUIHandler::OnDestroy - Cleanup when the object is destroyed.

    SYNOPSIS
        void ResultUIHandler::OnDestroy();

    DESCRIPTION
        This method ensures that the event handler is properly removed when the object is destroyed.

    RETURNS
        Nothing.
    */
    /**/
    void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for tracking the progress of cars in the game through laps and checkpoints. 
    It keeps count of the number of laps completed, the checkpoints passed, and the time at each checkpoint. 
    This class also manages the logic for determining when a race is finished and updates the UI to reflect 
    the car's current position in the race. It also coordinates with the GameManager to signal the end of 
    the race and manage post-race actions for player-controlled cars.
*/
/**/
public class LapCounter : MonoBehaviour
{
    int passedCheckpointNum = 0;
    float timeAtLastPassedCheckpoint = 0;

    int numOfPassedCheckpoints = 0;

    int lapsCompleted = 0;
    const int lapsToComplete = 4;

    bool isRaceFinished = false;

    int carPosition = 0;

    public Text carPositionText;

    bool isHideRoutineRunning = false;
    float hideUIDelayTime;

    LapUIHandler lapUIHandler;

    public event Action<LapCounter> OnPassCheckpoint;

    /**/
    /*
    LapCounter::Start() LapCounter::Start()

    NAME
        LapCounter::Start - Initialization of the LapCounter component.

    SYNOPSIS
        void LapCounter::Start();

    DESCRIPTION
        This method initializes the LapCounter for the player by setting up the lap UI text and preparing other relevant lap information.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        if (CompareTag("Player"))
        {
            lapUIHandler = FindObjectOfType<LapUIHandler>();
            lapUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");
        }
    }

    /**/
    /*
    LapCounter::SetCarPosition(int position) LapCounter::SetCarPosition(int position)

    NAME
        LapCounter::SetCarPosition - Sets the position of the car in the race.

    SYNOPSIS
        void LapCounter::SetCarPosition(int position);
            position   --> The race position of the car.

    DESCRIPTION
        This method sets the race position of the car. 

    RETURNS
        Nothing.
    */
    /**/
    public void SetCarPosition(int position)
    {
        carPosition = position;
    }
	
	/**/
	/*
    LapCounter::GetCarPosition() LapCounter::GetCarPosition()

    NAME
        LapCounter::GetCarPosition - Retrieves the race position of the car.

    SYNOPSIS
        int LapCounter::GetCarPosition();

    DESCRIPTION
        This method returns the current race position of the car. 

    RETURNS
        Int carPosition: The current race position of the car.
    */
	/**/
    public int GetCarPosition()
    {
        return carPosition;
    }

    /**/
    /*
    LapCounter::GetNumberOfCheckpointsPassed() LapCounter::GetNumberOfCheckpointsPassed()

    NAME
        LapCounter::GetNumberOfCheckpointsPassed - Retrieves the number of checkpoints passed.

    SYNOPSIS
        int LapCounter::GetNumberOfCheckpointsPassed();

    DESCRIPTION
        This method returns the total number of checkpoints passed by the car during the race. 

    RETURNS
        Int numOfPassedCheckpoints: The total number of checkpoints passed.
    */
    /**/
    public int GetNumberOfCheckpointsPassed()
    {
        return numOfPassedCheckpoints;
    }

    /**/
    /*
    LapCounter::GetTimeAtLastCheckPoint() LapCounter::GetTimeAtLastCheckPoint()

    NAME
        LapCounter::GetTimeAtLastCheckPoint - Retrieves the time at the last passed checkpoint.

    SYNOPSIS
        float LapCounter::GetTimeAtLastCheckPoint();

    DESCRIPTION
        This method returns the time (in seconds) recorded when the car last passed a checkpoint.

    RETURNS
        Float timeAtLastPassedCheckpoint: The time at the last checkpoint the car passed.
    */
    /**/
    public float GetTimeAtLastCheckPoint()
    {
        return timeAtLastPassedCheckpoint;
    }

    /**/
    /*
    LapCounter::ShowPositionCO(float delayUntilHidePosition) LapCounter::ShowPositionCO(float delayUntilHidePosition)

    NAME
        LapCounter::ShowPositionCO - Coroutine to display and hide car position UI.

    SYNOPSIS
        IEnumerator LapCounter::ShowPositionCO(float delayUntilHidePosition);
            delayUntilHidePosition   --> The delay in seconds before hiding the position UI.

    DESCRIPTION
        This coroutine displays the car's current race position on the UI and hides it after a specified delay.
        It is used to provide the player with feedback about their current position during the race.

    RETURNS
        IEnumerator that temporarily halts execution for the duration of hideUIDelayTime.
    */
    /**/
    IEnumerator ShowPositionCO(float delayUntilHidePosition)
    {
        hideUIDelayTime += delayUntilHidePosition;

        if (carPositionText == null) yield break;

        carPositionText.text = carPosition.ToString();

        carPositionText.gameObject.SetActive(true);

        if (!isHideRoutineRunning)
        {
            isHideRoutineRunning = true;

            yield return new WaitForSeconds(hideUIDelayTime);
            carPositionText.gameObject.SetActive(false);

            isHideRoutineRunning = false;
        }
    }

    /**/
    /*
    LapCounter::OnTriggerEnter2D(Collider2D collider2D) LapCounter::OnTriggerEnter2D(Collider2D collider2D)

    NAME
        LapCounter::OnTriggerEnter2D - Handles the car passing through a checkpoint.

    SYNOPSIS
        void LapCounter::OnTriggerEnter2D(Collider2D collider2D);
            collider2D   --> The collider of the checkpoint.

    DESCRIPTION
        This method is called when the car enters the trigger collider of a checkpoint or the finish line.
        It updates the lap and checkpoint information, triggers UI updates, and handles race completion logic.
 
    RETURNS
        Nothing.
    */
    /**/
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Checkpoint"))
        {
            if (isRaceFinished)
                return;

            Checkpoint checkpoint = collider2D.GetComponent<Checkpoint>();

            // Checks that the car is passing the checkpoints in the correct order
            if (passedCheckpointNum + 1 == checkpoint.checkpointNum)
            {
                passedCheckpointNum = checkpoint.checkpointNum;

                numOfPassedCheckpoints++;

                timeAtLastPassedCheckpoint = Time.time;

                if (checkpoint.isFinishLine)
                {
                    passedCheckpointNum = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                    {
                        isRaceFinished = true;
                    }
                    if (!isRaceFinished && lapUIHandler != null)
                    {
                        lapUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");
                    }
                }

                OnPassCheckpoint?.Invoke(this);

                // When passing the finish line, whow the car's calculated position
                if (isRaceFinished)
                {
                    StartCoroutine(ShowPositionCO(100));

                    // Allow AI to control the player's car when the race is finished
                    if (CompareTag("Player"))
                    {
                        GameManager.instance.UpdatePlayerRacePosition(1, carPosition);
                        GameManager.instance.OnRaceFinish();

                        GetComponent<CarInputHandler>().enabled = false;
                        GetComponent<CarAIHandler>().enabled = true;
                        GetComponent<AStarPath>().enabled = true;
                    }
                }
                else if (checkpoint.isFinishLine)
                {
                    StartCoroutine(ShowPositionCO(1.5f));
                }
            }
        }
    }
}

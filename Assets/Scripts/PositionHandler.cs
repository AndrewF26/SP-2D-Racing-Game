using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**/
/*
    This class is responsible for tracking and updating the race positions of all cars in the game. 
    It utilizes the LapCounter components attached to each car to determine their current positions based 
    on the number of checkpoints passed and the time at the last checkpoint. This class orchestrates the 
    sorting of cars based on their progress and updates the UI with the current positions using the PositionUIHandler. 
*/
/**/
public class PositionHandler : MonoBehaviour
{
    PositionUIHandler positionUIHandler;

    public List<LapCounter> carLapCounters = new List<LapCounter>();

    /**/
    /*
    PositionHandler::Start() PositionHandler::Start()

    NAME
        PositionHandler::Start - Initializes the PositionHandler component.

    SYNOPSIS
        void PositionHandler::Start();

    DESCRIPTION
        This method initializes the PositionHandler by finding all LapCounter components in the scene, storing them in a list, 
        and setting up event subscriptions for checkpoint passage. Additionally, it initializes the PositionUIHandler
        to update the UI with the car positions.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        // Get all the lap counters in a scen and stroe them in a list
        LapCounter[] carLapCounterArray = FindObjectsOfType<LapCounter>();
        carLapCounters = carLapCounterArray.ToList<LapCounter>();

        foreach (LapCounter lapCounters in carLapCounters)
            lapCounters.OnPassCheckpoint += OnPassCheckpoint;

        positionUIHandler = FindObjectOfType<PositionUIHandler>();

        positionUIHandler.UpdateList(carLapCounters);
    }

    /**/
    /*
    PositionHandler::OnPassCheckpoint(LapCounter carLapCounter) PositionHandler::OnPassCheckpoint(LapCounter carLapCounter)

    NAME
        PositionHandler::OnPassCheckpoint - Updates positions when a car passes a checkpoint.

    SYNOPSIS
        void PositionHandler::OnPassCheckpoint(LapCounter carLapCounter);
            carLapCounter   --> The LapCounter of the car that passed the checkpoint.

    DESCRIPTION
        This method is called when a car passes a checkpoint. It sorts the list of LapCounters based
        on the number of checkpoints passed and the time at the last checkpoint, updating the race
        positions of all cars. It also updates the PositionUIHandler to reflect these changes.

    RETURNS
        Nothing.
    */
    /**/
    void OnPassCheckpoint(LapCounter carLapCounter)
    {
        // Sort the car's positon first based on how many checkpoints they have passed, then sort on time
        carLapCounters = carLapCounters.OrderByDescending(s => s.GetNumberOfCheckpointsPassed()).ThenBy(s => s.GetTimeAtLastCheckPoint()).ToList();

        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;

        carLapCounter.SetCarPosition(carPosition);

        positionUIHandler.UpdateList(carLapCounters);
    }
}

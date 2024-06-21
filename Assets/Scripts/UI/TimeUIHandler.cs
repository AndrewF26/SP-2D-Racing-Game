using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for managing the display of race time in the game's user interface.
    It continuously updates a UI text element to show the elapsed time in minutes and seconds format. 
	This class also uses a coroutine to efficiently update the time display at regular intervals, 
	ensuring the time shown is current and accurate.
*/
/**/
public class TimeUIHandler : MonoBehaviour
{
    Text timeText;

    float lastRaceTimeUpdate = 0;

    /**/
    /*
	TimeUIHandler::Awake() TimeUIHandler::Awake()
	
	NAME
		TimeUIHandler::Awake - Initializes the TimeUIHandler component.
	
	SYNOPSIS
		void TimeUIHandler::Awake();
	
	DESCRIPTION
		This method is responsible for initializing the TimeUIHandler component, primarily by obtaining 
		a reference to the Text component which will be used to display the race time on the UI.
	
	RETURNS
		Nothing.
	*/
    /**/
    private void Awake()
    {
        timeText = GetComponent<Text>();
    }

    /**/
    /*
	TimeUIHandler::Start() TimeUIHandler::Start()
	
	NAME
		TimeUIHandler::Start - Starts the coroutine for updating the race time display.
	
	SYNOPSIS
		void TimeUIHandler::Start();
	
	DESCRIPTION
		This method triggers the start of the coroutine UpdateTimeCO(), which continually updates the race time displayed on the UI. 
	
	RETURNS
		Nothing.
	*/
    /**/
    void Start()
    {
        StartCoroutine(UpdateTimeCO());
    }

    /**/
    /*
	TimeUIHandler::UpdateTimeCO() TimeUIHandler::UpdateTimeCO()
	
	NAME
		TimeUIHandler::UpdateTimeCO - Continuously updates the race time display.
	
	SYNOPSIS
		IEnumerator TimeUIHandler::UpdateTimeCO();
	
	DESCRIPTION
		This coroutine is responsible for regularly updating the race time display in the UI. 
		It queries the current race time from the GameManager and updates the text component to 
		reflect the elapsed minutes and seconds. 
	
	RETURNS
		IEnumerator that temporarily halts execution to properly update the timer text.
	*/
    /**/
    IEnumerator UpdateTimeCO()
    {
        while (true)
        {
            float raceTime = GameManager.instance.GetRaceTime();

            if (lastRaceTimeUpdate != raceTime)
            {
                int raceTimeMinutes = (int)Mathf.Floor(raceTime / 60);
                int raceTimeSeconds = (int)Mathf.Floor(raceTime % 60);

                timeText.text = $"{raceTimeMinutes.ToString("00")}:{raceTimeSeconds.ToString("00")}";

                lastRaceTimeUpdate = raceTime;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}

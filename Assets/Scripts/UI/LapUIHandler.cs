using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for managing and updating the lap information displayed on the game's user interface. 
    It controls a Text element within the UI to show the current lap number and the total number of laps in the race. 
*/
/**/
public class LapUIHandler : MonoBehaviour
{
    Text lapText;

    /**/
    /*
    LapUIHandler::Awake() LapUIHandler::Awake()

    NAME
        LapUIHandler::Awake - Initializes the LapUIHandler component.

    SYNOPSIS
        void LapUIHandler::Awake();

    DESCRIPTION
        This method initializes the LapUIHandler by finding and storing the Text component attached to the same GameObject. 
        This Text component is used to display lap information to the player during the race.

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        lapText = GetComponent<Text>();
    }

    /**/
    /*
    LapUIHandler::SetLapText(string text) LapUIHandler::SetLapText(string text)

    NAME
        LapUIHandler::SetLapText - Updates the lap text display.

    SYNOPSIS
        public void SetLapText(string text);
            text   --> The string to be displayed in the lap text UI.

    DESCRIPTION
        This method updates the lap text UI with the provided string. It's called to display the current lap 
        and the total number of laps.

    RETURNS
        Nothing.
    */
    /**/
    public void SetLapText(string text)
    {
        lapText.text = text;
    }
}

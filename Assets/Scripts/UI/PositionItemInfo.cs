using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for managing the display of race position and car name information
    in a user interface element. It primarily interacts with text components from the Unity UI framework 
    to update and reflect the current race position and the corresponding car's name. 
*/
/**/
public class PositionItemInfo : MonoBehaviour
{
    public Text positionText;
    public Text carNameText;

    /**/
    /*
    PositionItemInfo::SetPositionText(string newPosition) PositionItemInfo::SetPositionText(string newPosition)

    NAME
        PositionItemInfo::SetPositionText - Updates the position display text.

    SYNOPSIS
        public void PositionItemInfo::SetPositionText(string newPosition);
            newPosition  --> The new position to be displayed.

    DESCRIPTION
        This function updates the text displaying the position in the race.

    RETURNS
        Nothing.
    */
    /**/
    public void SetPositionText(string newPosition)
    {
        positionText.text = newPosition;
    }

    /**/
    /*
    PositionItemInfo::SetCarName(string newCarName) PositionItemInfo::SetCarName(string newCarName)

    NAME
        PositionItemInfo::SetCarName - Updates the car name display text.

    SYNOPSIS
        public void PositionItemInfo::SetCarName(string newCarName);
            newCarName  --> The new car name to be displayed.

    DESCRIPTION
        This method is used to update the text displaying the name of the car. 

    RETURNS
        Nothing.
    */
    /**/
    public void SetCarName(string newCarName)
    {
        carNameText.text = newCarName;
    }
}

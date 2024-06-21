using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/**/
/*
    This class manages the track selection interface in the game. It allows players to view, unlock, 
	and select tracks for racing. This class also handles UI interactions for track selection,
    including displaying total points and updating the state of unlock buttons for each track.
*/
/**/
public class SelectTrackMenu : MonoBehaviour
{
    public TMP_Text totalPointsText;

    public GameObject[] unlockButtons;

    /**/
    /*
	SelectTrackMenu::Start() SelectTrackMenu::Start()
	
	NAME
		SelectTrackMenu::Start - Initializes the track selection menu.
	
	SYNOPSIS
		void SelectTrackMenu::Start();
	
	DESCRIPTION
		This method initializes the track selection menu by updating the total points display and 
		the state of the unlock buttons for each track.
	
	RETURNS
		Nothing.
	*/
    /**/
    private void Start()
    {
        UpdateTotalPointsDisplay();
        UpdateUnlockButtons();
    }

    /**/
    /*
	SelectTrackMenu::UpdateTotalPointsDisplay() SelectTrackMenu::UpdateTotalPointsDisplay()
	
	NAME
		SelectTrackMenu::UpdateTotalPointsDisplay - Updates the display of total points.
	
	SYNOPSIS
		void SelectTrackMenu::UpdateTotalPointsDisplay();
	
	DESCRIPTION
		This method updates the text display showing the total points available to the player.
	
	RETURNS
		Nothing.
	*/
    /**/
    public void UpdateTotalPointsDisplay()
    {
        if (totalPointsText != null)
        {
            totalPointsText.text = "Total Points: " + GameManager.instance.totalPoints.ToString();
        }
    }

    /**/
    /*
	SelectTrackMenu::UpdateUnlockButtons() SelectTrackMenu::UpdateUnlockButtons()
	
	NAME
		SelectTrackMenu::UpdateUnlockButtons - Updates the state of unlock buttons for tracks.
	
	SYNOPSIS
		void SelectTrackMenu::UpdateUnlockButtons();
	
	DESCRIPTION
		This method updates each unlock button in the track selection menu. It checks if each track
		is unlocked or if the player has sufficient points to unlock a track. 
	
	RETURNS
		Nothing.
	*/
    /**/
    private void UpdateUnlockButtons()
    {
        for (int i = 0; i < unlockButtons.Length; i++)
        {
            // Hide the button if the track is unlocked or the player has enough points
            bool isUnlocked = GameManager.instance.IsTrackUnlocked(i);
            unlockButtons[i].SetActive(!isUnlocked && GameManager.instance.totalPoints < 1000);
        }
    }

    /**/
    /*
	SelectTrackMenu::UnlockTrack(int trackID) SelectTrackMenu::UnlockTrack(int trackID)
	
	NAME
		SelectTrackMenu::UnlockTrack - Unlocks a specified track.
	
	SYNOPSIS
		void SelectTrackMenu::UnlockTrack(int trackID);
			trackID   --> The ID of the track to unlock.
	
	DESCRIPTION
		This method is called to unlock a specific track. It checks if the player has enough points
		to unlock the track. If sufficient points are available, the track is unlocked via the GameManager,
		and the UI is updated to reflect the new unlock state and total points.
	
	RETURNS
		Nothing.
	*/
    /**/
    public void UnlockTrack(int trackID)
    {
        if (GameManager.instance.totalPoints >= 100)
        {
            GameManager.instance.UnlockTrack(trackID); 
            UpdateUnlockButtons();
            UpdateTotalPointsDisplay();
        }
        else
        {
            Debug.Log("You need at least 100 points to unlock this track.");
        }
    }

    /**/
    /*
	SelectTrackMenu::StartTrack(int trackID) SelectTrackMenu::StartTrack(int trackID)
	
	NAME
		SelectTrackMenu::StartTrack - Initiates the loading of a selected track.
	
	SYNOPSIS
		void SelectTrackMenu::StartTrack(int trackID);
			trackID   --> The ID of the track to be loaded.
	
	DESCRIPTION
		This method is responsible for starting a race on the selected track. It constructs the scene name
		based on the provided track ID and then uses the SceneManager to load the corresponding level scene.
	
	RETURNS
		Nothing.
	*/
    /**/
    public void StartTrack(int trackID)
    {
        string trackName = "Level-" + trackID;

        SceneManager.LoadScene(trackName);
    }
}

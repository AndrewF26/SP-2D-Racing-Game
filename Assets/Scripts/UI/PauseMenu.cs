using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**/
/*
    This class handles the in-game pause functionality, allowing players to halt gameplay and 
    access the pause menu. This class is responsible for toggling the visibility of the pause menu, adjusting 
    the game's time scale to pause or resume the game, and providing options to navigate to different scenes.
*/
/**/
public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    /**/
    /*
    PauseMenu::OnPause() PauseMenu::OnPause()

    NAME
        PauseMenu::OnPause - Activates the pause menu and pauses the game.

    SYNOPSIS
        public void PauseMenu::OnPause();

    DESCRIPTION
        This function is triggered to pause the game. It activates the pause menu UI and sets the 
        game's time scale to 0, which pauses all game actions.

    RETURNS
        Nothing.
    */
    /**/
    public void OnPause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    /**/
    /*
    PauseMenu::OnResume() PauseMenu::OnResume()

    NAME
        PauseMenu::OnResume - Resumes the game from a paused state.

    SYNOPSIS
        public void PauseMenu::OnResume();

    DESCRIPTION
        This method resumes the game from a paused state. It deactivates the pause menu UI and sets 
        the game's time scale back to 1, allowing the game to continue running as normal.

    RETURNS
        Nothing.
    */
    /**/
    public void OnResume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    /**/
    /*
    PauseMenu::OnCarSelectExit() PauseMenu::OnCarSelectExit()

    NAME
        PauseMenu::OnCarSelectExit - Exits to the car selection menu.

    SYNOPSIS
        public void PauseMenu::OnCarSelectExit();

    DESCRIPTION
        This function handles the action of exiting to the car selection menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnCarSelectExit()
    {
        SceneManager.LoadScene("Select-Menu");
        Time.timeScale = 1;
    }

    /**/
    /*
    PauseMenu::OnTrackSelectExit() PauseMenu::OnTrackSelectExit()

    NAME
        PauseMenu::OnTrackSelectExit - Exits to the track selection menu.

    SYNOPSIS
        public void PauseMenu::OnTrackSelectExit();

    DESCRIPTION
        This method handles the action of exiting to the track selection menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnTrackSelectExit()
    {
        SceneManager.LoadScene("Level-Menu");
        Time.timeScale = 1;
    }

    /**/
    /*
    PauseMenu::OnMainMenuExit() PauseMenu::OnMainMenuExit()

    NAME
        PauseMenu::OnMainMenuExit - Exits to the main menu.

    SYNOPSIS
        public void PauseMenu::OnMainMenuExit();

    DESCRIPTION
        This function handles the action of exiting to the main menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnMainMenuExit()
    {
        SceneManager.LoadScene("Main-Menu");
        Time.timeScale = 1;
    }
}

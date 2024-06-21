using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**/
/*
    This class is responsible for handling user interactions in the main menu of the game. 
    It provides functionality for navigating to different parts of the game such as the car selection menu, 
    options menu, and the functionality to exit the game. 
*/
/**/
public class MainMenu : MonoBehaviour
{
    /**/
    /*
    MainMenu::PlayGame() MainMenu::PlayGame()

    NAME
        MainMenu::PlayGame - Loads the car selection menu.

    SYNOPSIS
        public void MainMenu::PlayGame();

    DESCRIPTION
        This function is triggered by a UI event to load the car selection menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void PlayGame()
    {
        SceneManager.LoadScene("Select-Menu");
    }

    /**/
    /*
    MainMenu::SelectOptions() MainMenu::SelectOptions()

    NAME
        MainMenu::SelectOptions - Loads the options menu.

    SYNOPSIS
        public void MainMenu::SelectOptions();

    DESCRIPTION
        This function is called to load the options menu scene and changes the current scene to the options menu.

    RETURNS
        Nothing.
    */
    /**/
    public void SelectOptions()
    {
        SceneManager.LoadScene("Option-Menu");
    }

    /**/
    /*
    MainMenu::SelectMainMenu() MainMenu::SelectMainMenu()

    NAME
        MainMenu::SelectMainMenu - Returns to the main menu.

    SYNOPSIS
        public void MainMenu::SelectMainMenu();

    DESCRIPTION
        This function is used to return to the main menu scene. It changes the current scene back to the main menu. 

    RETURNS
        Nothing.
    */
    /**/
    public void SelectMainMenu()
    {
        SceneManager.LoadScene("Main-Menu");
    }

    /**/
    /*
    MainMenu::ExitGame() MainMenu::ExitGame()

    NAME
        MainMenu::ExitGame - Exits the game application.

    SYNOPSIS
        public void MainMenu::ExitGame();

    DESCRIPTION
        This function is responsible for quitting the game application. 

    RETURNS
        Nothing.
    */
    /**/
    public void ExitGame()
    {
        Application.Quit();
    }
}

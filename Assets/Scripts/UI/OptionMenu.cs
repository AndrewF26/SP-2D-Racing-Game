using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/**/
/*
    This class manages the in-game options menu, providing functionalities such as adjusting 
    game audio settings and setting the difficulty level for AI opponents. It utilizes Unity's 
    AudioMixer to control the game's master volume.
*/
/**/
public class OptionMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    const string MASTER_VOLUME = "MasterVolume";

    /**/
    /*
    OptionMenu::SetVolume(float volume) OptionMenu::SetVolume(float volume)

    NAME
        OptionMenu::SetVolume - Sets the game's master volume.

    SYNOPSIS
        public void OptionMenu::SetVolume(float volume);
            volume   --> The volume level to set, typically between a min and max range.

    DESCRIPTION
        This method adjusts the master volume of the game's audio mixer. 

    RETURNS
        Nothing.
    */
    /**/
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(MASTER_VOLUME, volume);
    }
	
	/**/
	/*
    OptionMenu::GetVolume() OptionMenu::GetVolume()

    NAME
        OptionMenu::GetVolume - Retrieves the current master volume setting.

    SYNOPSIS
        public float OptionMenu::GetVolume();

    DESCRIPTION
        This method fetches the current setting of the master volume from the audio mixer. 

    RETURNS
        Float volume: The current master volume level.
    */
	/**/
    public float GetVolume()
    {
        float volume;
        bool result = audioMixer.GetFloat(MASTER_VOLUME, out volume);

        // If the volume cannot be fetched, it defaults to 0
        if (result)
        {
            return volume;
        }
        else
        {
            return 0; 
        }
    }

    /**/
    /*
    OptionMenu::SetAIDifficulty(string difficulty) OptionMenu::SetAIDifficulty(string difficulty)

    NAME
        OptionMenu::SetAIDifficulty - Configures the difficulty level for AI opponents.

    SYNOPSIS
        public void OptionMenu::SetAIDifficulty(string difficulty);
            difficulty   --> A string indicating the desired difficulty level ("easy", "medium", "hard").

    DESCRIPTION
        This method is used to set the difficulty level of AI opponents in the game. 

    RETURNS
        Nothing.
	*/
    /**/
    public void SetAIDifficulty(string difficulty)
    {
        float skillLevel = 0.9f;

        switch (difficulty.ToLower())
        {
            case "easy":
                skillLevel = 0.8f;
                break;
            case "medium":
                skillLevel = 0.9f;
                break;
            case "hard":
                skillLevel = 1.0f;
                break;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.AIDifficulty = skillLevel;
        }
    }
}

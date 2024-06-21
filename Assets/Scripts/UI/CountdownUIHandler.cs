using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for managing the visual countdown sequence. It controls a UI Text element 
    to display a countdown sequence, indicating the start of the race. 
*/
/**/
public class CountdownUIHandler : MonoBehaviour
{
    public Text countdownText;

    /**/
    /*
    CountdownUIHandler::Awake() CountdownUIHandler::Awake()

    NAME
        CountdownUIHandler::Awake - Initializes the CountdownUIHandler component.

    SYNOPSIS
        void CountdownUIHandler::Awake();

    DESCRIPTION
        This method initializes the CountdownUIHandler by setting the countdown text to an empty string. 
        
    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        countdownText.text = "";
    }

    /**/
    /*
    CountdownUIHandler::Start() CountdownUIHandler::Start()

    NAME
        CountdownUIHandler::Start - Begins the countdown sequence.

    SYNOPSIS
        void CountdownUIHandler::Start();

    DESCRIPTION
        This method starts a coroutine (CountdownCO) to manage the countdown sequence for the race start. 

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        StartCoroutine(CountdownCO());
    }

    /**/
    /*
    CountdownUIHandler::CountdownCO() CountdownUIHandler::CountdownCO()

    NAME
        CountdownUIHandler::CountdownCO - Coroutine for handling the countdown sequence.

    SYNOPSIS
        IEnumerator CountdownUIHandler::CountdownCO();

    DESCRIPTION
        This coroutine handles the countdown sequence for the start of the race. After showing the countdown,
        it notifies the GameManager to start the race and then deactivates the countdown UI.

    RETURNS
        IEnumerator that temporarily halts execution for the duration of each part of the countdown.
    */
    /**/
    IEnumerator CountdownCO()
    {
        int counter = 3;
        yield return new WaitForSeconds(2.3f);

        while (true)
        {
            if (counter != 0)
            {
                countdownText.text = counter.ToString();
            }
            else
            {
                countdownText.text = "GO!";

                GameManager.instance.OnRaceStart();

                break;
            }

            counter--;
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}

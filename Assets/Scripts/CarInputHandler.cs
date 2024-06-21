using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class is responsible for managing player inputs and translating them into actions for the car. 
    It captures and processes inputs such as steering, acceleration, braking, and the use of items. 
    This class serves as the intermediary between the player's input devices and the car's behavior, 
    ensuring that player commands are accurately reflected in the game. 
*/
/**/
public class CarInputHandler : MonoBehaviour
{
    CarController carController;

    /**/
    /*
    CarInputHandler::Awake() CarInputHandler::Awake()

    NAME
        CarInputHandler::Awake - Initializes the CarInputHandler.

    SYNOPSIS
        void CarInputHandler::Awake();

    DESCRIPTION
        This function initializes the CarController component attached to the same GameObject. 

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        carController = GetComponent<CarController>();
    }

    /**/
    /*
    CarInputHandler::Update() CarInputHandler::Update()

    NAME
        CarInputHandler::Update - Handles per-frame input processing.

    SYNOPSIS
        void CarInputHandler::Update();

    DESCRIPTION
        This method is called once per frame and handles the processing of player inputs. It reads the
        horizontal and vertical axis inputs, and triggers the use of the car's current item if the space
        key is pressed. The input is then passed to the car controller for handling car movement.

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && carController.currentItem != null)
        {
            carController.currentItem.Use(carController);
            carController.currentItem = null;
        }

        carController.SetInputVector(inputVector);
    }
}

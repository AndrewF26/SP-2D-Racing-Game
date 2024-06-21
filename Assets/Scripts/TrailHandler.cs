using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    The TrailHandler class is responsible for managing the visual trail effects for the cars. 
    The class listens to the car's drifting and braking status, and activates or deactivates the trail emission accordingly. 
*/
/**/
public class TrailHandler : MonoBehaviour
{
    CarController carController;
    TrailRenderer trailRenderer;

    /**/
    /*
    TrailHandler::Awake() TrailHandler::Awake()

    NAME
        TrailHandler::Awake - Initializes the TrailHandler component.

    SYNOPSIS
        void TrailHandler::Awake();

    DESCRIPTION
        This method initializes the TrailHandler by finding and storing the CarController and TrailRenderer components. 
        The TrailRenderer is set to not emit at the start, and will be activated when certain conditions in the Update method 
        are met, such as when the car is drifting.

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        carController = GetComponentInParent<CarController>();

        trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.emitting = false;
    }

    /**/
    /*
    TrailHandler::Update() TrailHandler::Update()

    NAME
        TrailHandler::Update - Updates the trail emission based on car's behavior.

    SYNOPSIS
        void TrailHandler::Update();

    DESCRIPTION
        This method checks if the car is currently drifting or braking using the CarController's IsTireDrifting method. 
        If the car is drifting, it activates the trail renderer to emit trails, otherwise it stops the emission. 

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        if (carController.IsTireDrifting(out float lateralVelocity, out bool isBraking))
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}

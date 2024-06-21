using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class manages the particle effects associated with a car's movement and actions in the game. 
    It dynamically adjusts the rate and intensity of particle emissions based on the car's current state, 
    such as drifting, braking, or driving on different surfaces. 
*/
/**/
public class CarParticleHandler : MonoBehaviour
{
    float particleEmissionRate = 0;

    CarController carController;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleEM;

    /**/
    /*
    CarParticleHandler::Awake() CarParticleHandler::Awake()

    NAME
        CarParticleHandler::Awake - Initializes the CarParticleHandler.

    SYNOPSIS
        void CarParticleHandler::Awake();

    DESCRIPTION
        This function initializes the CarController and ParticleSystem components. It also retrieves and configures 
        the emission module of the particle system to initially emit no particles.

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        carController = GetComponentInParent<CarController>();

        particleSystemSmoke = GetComponent<ParticleSystem>();

        // Get the emission component and set it to zero emission. 
        particleEM = particleSystemSmoke.emission;
        particleEM.rateOverTime = 0;
    }

    /**/
    /*
    CarParticleHandler::Update() CarParticleHandler::Update()

    NAME
        CarParticleHandler::Update - Updates the particle effects each frame.

    SYNOPSIS
        void CarParticleHandler::Update();

    DESCRIPTION
        This method is called once per frame and handles the emission of particles based on the car's movement.
        It gradually reduces the emission rate over time and increases it when the car is drifting or braking.
        The emission rate is proportional to the car's horizontal velocity.

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleEM.rateOverTime = particleEmissionRate;


        if (carController.IsTireDrifting(out float horizontalVelocity, out bool isBraking))
        {
            if (isBraking)
                particleEmissionRate = 30;

            else particleEmissionRate = Mathf.Abs(horizontalVelocity) * 2;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/**/
/*
    This class manages the sound effects associated with a car's actions and interactions. It controls audio cues 
    for different states and behaviors of the car, such as engine sounds, tire screeching during drifts, collision impacts, 
    and jump landings. The class adjusts sound properties like volume and pitch based on the car's dynamics.
*/
/**/
public class CarSFXHandler : MonoBehaviour
{
    [Header("Audio sources")]
    public AudioSource driftAudioSource;
    public AudioSource engineAudioSource;
    public AudioSource carHitAudioSource;
    public AudioSource carJumpAudioSource;
    public AudioSource carLandAudioSource;

    float desiredEnginePitch = 0.5f;
    float tireDriftPitch = 0.5f;

    CarController carController;

    /**/
    /*
    CarSFXHandler::Awake() CarSFXHandler::Awake()

    NAME
        CarSFXHandler::Awake - Initializes the CarSFXHandler.

    SYNOPSIS
        void CarSFXHandler::Awake();

    DESCRIPTION
        This function is called when the script instance is being loaded. It initializes the CarController
        component to control audio effects based on the car's state and behavior.

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        carController = GetComponentInParent<CarController>();
    }

    /**/
    /*
    CarSFXHandler::Update() CarSFXHandler::Update()

    NAME
        CarSFXHandler::Update - Updates the car's sound effects each frame.

    SYNOPSIS
        void CarSFXHandler::Update();

    DESCRIPTION
        This method is called once per frame and handles the updating of the car's sound effects. It manages
        the engine sound effects and tire drift sounds based on the car's speed, drift status, and braking behavior.

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        UpdateEngineSFX();
        UpdateTireDriftSFX();
    }

    /**/
    /*
    CarSFXHandler::UpdateEngineSFX() CarSFXHandler::UpdateEngineSFX()

    NAME
        CarSFXHandler::UpdateEngineSFX - Manages the engine sound effects.

    SYNOPSIS
        void CarSFXHandler::UpdateEngineSFX();

    DESCRIPTION
        This method adjusts the engine sound effects based on the car's current velocity.
        It modifies both the volume and pitch of the engine sound to reflect changes in the car's speed.

    RETURNS
        Nothing.
    */
    /**/
    void UpdateEngineSFX()
    {
        float velocityMagnitude = carController.GetVelocityMagnitude();

        // Increase the engine volume as the car goes faster
        float desiredEngineVolume = velocityMagnitude * 0.05f;

        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);

        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);

        desiredEnginePitch = velocityMagnitude * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    /**/
    /*
    CarSFXHandler::UpdateTireDriftSFX() CarSFXHandler::UpdateTireDriftSFX()

    NAME
        CarSFXHandler::UpdateTireDriftSFX - Manages the tire drifting sound effects.

    SYNOPSIS
        void CarSFXHandler::UpdateTireDriftSFX();

    DESCRIPTION
        This method adjusts the tire drifting sound effects based on whether the car is
        drifting or braking. It changes the volume and pitch of the drifting sounds to reflect the
        intensity of the car's lateral movements.

    RETURNS
        Nothing.
    */
    /**/
    void UpdateTireDriftSFX()
    {
        // Handle tire screeching SFX
        if (carController.IsTireDrifting(out float lateralVelocity, out bool isBraking))
        {
            // If the car is braking, then change the volume and pitch of the tire screech
            if (isBraking)
            {
                driftAudioSource.volume = Mathf.Lerp(driftAudioSource.volume, 1.0f, Time.deltaTime * 10);
                tireDriftPitch = Mathf.Lerp(tireDriftPitch, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                driftAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireDriftPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        // Fade out the tire screech SFX if we are not screeching 
        else driftAudioSource.volume = Mathf.Lerp(driftAudioSource.volume, 0, Time.deltaTime * 10);
    }

    /**/
    /*
    CarSFXHandler::OnCollisionEnter2D(Collision2D collision2D) CarSFXHandler::OnCollisionEnter2D(Collision2D collision2D)

    NAME
        CarSFXHandler::OnCollisionEnter2D - Handles collision sound effects.

    SYNOPSIS
        void CarSFXHandler::OnCollisionEnter2D(Collision2D collision2D);
            collision2D    --> Collision data from the 2D physics engine.

    DESCRIPTION
        This method is triggered when the car enters a collision. It plays a sound effect based on the
        intensity of the collision, adjusting the pitch and volume to reflect the force of impact.

    RETURNS
        Nothing.
    */
    /**/
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        float relativeVelocity = collision2D.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
        carHitAudioSource.volume = volume;

        if (!carHitAudioSource.isPlaying)
            carHitAudioSource.Play();
    }

    /**/
    /*
    CarSFXHandler::PlayJumpSfx() CarSFXHandler::PlayJumpSfx()

    NAME
        CarSFXHandler::PlayJumpSfx - Plays the jump sound effect.

    SYNOPSIS
        void CarSFXHandler::PlayJumpSfx();

    DESCRIPTION
        This method triggers the sound effect associated with the car's jump action.

    RETURNS
        Nothing.
    */
    /**/
    public void PlayJumpSfx()
    {
        carJumpAudioSource.Play();
    }

    /**/
    /*
    CarSFXHandler::PlayLandingSfx() CarSFXHandler::PlayLandingSfx()

    NAME
        CarSFXHandler::PlayLandingSfx - Plays the sound effect associated with the car landing.

    SYNOPSIS
        void CarSFXHandler::PlayLandingSfx();

    DESCRIPTION
        This method triggers the sound effect for when the car lands after a jump. 

    RETURNS
        Nothing.
    */
    /**/
    public void PlayLandingSfx()
    {
        carLandAudioSource.Play();
    }
}

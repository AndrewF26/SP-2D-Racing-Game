using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    The CarController class manages the physics, control, and behavior of a car in the game environment. It handles
    fundamental car dynamics such as acceleration, steering, drifting, and jumping, as well as interactions with different 
    surfaces and the use of power-ups and items. This class controls the car's response to player inputs and AI commands,
    adjusting its behavior based on factors like surface type, obstacles, and current speed. Special features like 
    jumps and item usage are also managed within this class.
*/
/**/
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float driftFactor = 0.93f;
    public float accelerationFactor = 6.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 8;

    [Header("Sprite Settings")]
    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carShadowRenderer;

    [Header("Jump Settings")]
    public AnimationCurve jumpCurve;

    [Header("Item Settings")]
    public GameObject mudPuddlePrefab;

    [Header("Boost Settings")]
    public float boostSpeed = 10f; 
    public float boostDuration = 5f; 

    public ParticleSystem speedBoostParticles;

    public float originalMaxSpeed;
    private bool isBoosting;

    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVsUp = 0;
    bool isJumping = false;
    private float topSpeed = 0f;

    public RaceItem currentItem;

    Rigidbody2D carRigidbody2D;
    Collider2D carCollider;
    SurfaceHandler surfaceHandler;
    CarSFXHandler carSFXHandler;
	
	/**/
	/*
    CarController::TopSpeed

    NAME
        CarController::TopSpeed - Gets the top speed achieved by the car.

    DESCRIPTION
        This property returns the highest speed that the car has achieved. Used for tracking performance
        and gameplay elements that depend on the car's speed.

    RETURNS
        Float topSpeed: The top speed of the car.
    */
	/**/
    public float TopSpeed
    {
        get { return topSpeed; }
    }

    /**/
    /*
    CarController::AssignItem(RaceItem item) CarController::AssignItem(RaceItem item)

    NAME
        CarController::AssignItem - Assigns a race item to the car.

    SYNOPSIS
        void CarController::AssignItem(RaceItem item);
            item   --> The race item to be assigned to the car.

    DESCRIPTION
        This function assigns a race item to the car.

    RETURNS
        Nothing.
    */
    /**/
    public void AssignItem(RaceItem item)
    {
        currentItem = item;
    }

    /**/
    /*
    CarController::Awake() CarController::Awake()

    NAME
        CarController::Awake - Initializes components for the car controller.

    SYNOPSIS
        void CarController::Awake();

    DESCRIPTION
        This function is called when the script instance is being loaded. It initializes the Rigidbody2D, 
        Colliders, SurfaceHandler, and CarSFXHandler components attached to the car. It also sets the 
        original maximum speed of the car.

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carCollider = GetComponentInChildren<Collider2D>();
        surfaceHandler = GetComponent<SurfaceHandler>();
        carSFXHandler = GetComponent<CarSFXHandler>();
        originalMaxSpeed = maxSpeed;
    }

    /**/
    /*
	CarController::Start() CarController::Start()

    NAME
        CarController::Start - Initialization at the start of the scene.

    SYNOPSIS
        void CarController::Start();

    DESCRIPTION
        This function initializes the car's rotation angle based on its current orientation. 

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
    }

    /**/
    /*
    CarController::FixedUpdate() CarController::FixedUpdate()

    NAME
        CarController::FixedUpdate - Handles the physics updates for the car.

    SYNOPSIS
        void CarController::FixedUpdate();

    DESCRIPTION
        This method is called every fixed framerate frame and is used for handling physics-based updates. 
        It includes applying engine force, steering, and managing horizontal velocity. 

    RETURNS
        Nothing.
    */
    /**/
    void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown)
        {
            return;
        }

        ApplyEngineForce();

        ApplySteering();

        ReduceHorizontalVelocity();

        UpdateTopSpeed();
    }

    /**/
    /*
    CarController::ApplyEngineForce() CarController::ApplyEngineForce()

    NAME
        CarController::ApplyEngineForce - Applies force to the car's engine.

    SYNOPSIS
        void CarController::ApplyEngineForce();

    DESCRIPTION
        This function calculates and applies the force to the car's engine based on the acceleration input. 
        It considers whether the car is jumping, the current speed, and adjusts for different surface types. 
        The drag of the car is also dynamically adjusted based on these factors.

    RETURNS
        Nothing.
    */
    /**/
    void ApplyEngineForce()
    {
        // Prevents the player from braking in the air
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;

        // Calculate how much we are going forward in terms of velocity's direction
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        // Limit to max speed of car
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        // Limit to 50% of max speed when driving in reverse
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        // Limit the speed the car goes in any direction while accelerating
        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
            return;

        // Apply drag if there is no input
        if (accelerationInput == 0)
        {
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 0, Time.fixedDeltaTime * 10);
        }

        // Apply a certain amount of drag depending on the surface being driven on when not using a boost item
        if (!isBoosting)
        {
            switch (GetSurface())
            {
                case Surface.SurfaceTypes.Grass:
                    carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 10.0f, Time.fixedDeltaTime * 3);
                    break;

                case Surface.SurfaceTypes.Sand:
                    carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 9.0f, Time.fixedDeltaTime * 3);
                    break;

                case Surface.SurfaceTypes.Mud:
                    carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 10.0f, Time.fixedDeltaTime * 3);
                    break;
            }
        }

        // Create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        // Applies force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);

    }

    /**/
    /*
    CarController::ApplySteering() CarController::ApplySteering()

    NAME
        CarController::ApplySteering - Manages the steering of the car.

    SYNOPSIS
        void CarController::ApplySteering();

    DESCRIPTION
        This function handles the car's steering. It calculates the steering angle based on user input
        and the current speed of the car. The function ensures that steering is more effective at higher
        speeds and less pronounced when the car is moving slowly.

    RETURNS
        Nothing.
    */
    /**/
    void ApplySteering()
    {
        // Limit car's ability to turn when moving slowly
        float minSpeedBeforeTurn = (carRigidbody2D.velocity.magnitude / 8);
        minSpeedBeforeTurn = Mathf.Clamp01(minSpeedBeforeTurn);

        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurn;

        carRigidbody2D.MoveRotation(rotationAngle);
    }

    /**/
    /*
    CarController::ReduceHorizontalVelocity() CarController::ReduceHorizontalVelocity()

    NAME
        CarController::ReduceHorizontalVelocity - Reduces unwanted sideways movement.

    SYNOPSIS
        void CarController::ReduceHorizontalVelocity();

    DESCRIPTION
        This function reduces the car's sideways velocity to prevent unrealistic sliding during movement. 
        It uses the car's current direction and drift factor to adjust the velocity and maintain more realistic driving physics.

    RETURNS
        Nothing.
    */
    /**/
    void ReduceHorizontalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        float currentDriftFactor = driftFactor;

        switch (GetSurface())
        {
            case Surface.SurfaceTypes.Grass:
                currentDriftFactor *= 1.05f; 
                break;
        }

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * currentDriftFactor;
    }
	
	/**/
	/*
    CarController::GetHorizontalVelocity() CarController::GetHorizontalVelocity()

    NAME
        CarController::GetHorizontalVelocity - Retrieves the car's sideways movement speed.

    SYNOPSIS
        float CarController::GetHorizontalVelocity();

    DESCRIPTION
        This function calculates and returns the sideways velocity of the car. It determines the car's drifting behavior 
        and is used in physics calculations related to car handling and tire screeching.

    RETURNS
        The car's sideways velocity.
    */
	/**/
    float GetHorizontalVelocity()
    { 
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    /**/
    /*
    CarController::IsTireDrifting(out float horizontalVelocity, out bool isBraking) CarController::IsTireDrifting(out float horizontalVelocity, out bool isBraking)

    NAME
        CarController::IsTireDrifting - Checks if the car's tires are drifting.

    SYNOPSIS
        bool CarController::IsTireDrifting(out float horizontalVelocity, out bool isBraking);
            horizontalVelocity   --> Out parameter to store the horizontal velocity.
            isBraking         --> Out parameter to indicate if the car is braking.

    DESCRIPTION
        This function determines whether the car's tires are drifting based on its horizontal velocity and
        braking status. It calculates if the car is screeching due to drifting or braking, which is
        used for triggering audio or visual effects associated with tire screeching.

    RETURNS
        True if the tires are drifting, False otherwise.
    */
    /**/
    public bool IsTireDrifting(out float horizontalVelocity, out bool isBraking)
    {
        horizontalVelocity = GetHorizontalVelocity();
        isBraking = false;

        if (isJumping)
            return false;

        // Checks if the player is moving forward and hitting the brakes.
        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        // If there is a lot of side movement, then the tires should be screeching
        if (Mathf.Abs(GetHorizontalVelocity()) > 4.0f)
            return true;

        return false;
    }

    /**/
    /*
    CarController::UpdateTopSpeed() CarController::UpdateTopSpeed()

    NAME
        CarController::UpdateTopSpeed - Updates the record of the car's top speed.

    SYNOPSIS
        void CarController::UpdateTopSpeed();

    DESCRIPTION
        This function updates the car's top speed record. It checks the current speed and, if it's higher than
        the previously recorded top speed, updates the record.

    RETURNS
        Nothing.
    */
    /**/
    private void UpdateTopSpeed()
    {
        float currentSpeed = GetVelocityMagnitude();
        if (currentSpeed > topSpeed)
        {
            topSpeed = currentSpeed;
        }
    }

    /**/
    /*
    CarController::SetInputVector(Vector2 inputVector) CarController::SetInputVector(Vector2 inputVector)

    NAME
        CarController::SetInputVector - Sets the input vector for car control.

    SYNOPSIS
        void CarController::SetInputVector(Vector2 inputVector);
            inputVector    --> The input vector for steering and acceleration.

    DESCRIPTION
        This method is used to set the steering and acceleration inputs for the car based on player controls.
        The input vector contains values for both steering (x-axis) and acceleration (y-axis), which are 
        used to control the car's movement and direction.

    RETURNS
        Nothing.
    */
    /**/
    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
	
	/**/
	/*
    CarController::GetVelocityMagnitude() CarController::GetVelocityMagnitude()

    NAME
        CarController::GetVelocityMagnitude - Retrieves the car's current velocity magnitude.

    SYNOPSIS
        float CarController::GetVelocityMagnitude();

    DESCRIPTION
        This function calculates and returns the magnitude of the car's current velocity. 

    RETURNS
        The magnitude of the car's current velocity.
    */
	/**/
    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }
	
	/**/
	/*
    CarController::GetSurface() CarController::GetSurface()

    NAME
        CarController::GetSurface - Retrieves the type of surface the car is currently on.

    SYNOPSIS
        Surface.SurfaceTypes CarController::GetSurface();

    DESCRIPTION
        This method returns the type of surface that the car is currently driving on, such as grass, sand, or mud.
        It is utilized to adjust the car's handling and physics according to different surface types.

    RETURNS
        The type of surface the car is on.
    */
	/**/
    public Surface.SurfaceTypes GetSurface()
    {
        return surfaceHandler.GetCurrentSurface();
    }

    /**/
    /*
    CarController::Jump(float jumpHeightScale, float jumpPushScale) CarController::Jump(float jumpHeightScale, float jumpPushScale)

    NAME
        CarController::Jump - Initiates a jump for the car with given parameters.

    SYNOPSIS
        void CarController::Jump(float jumpHeightScale, float jumpPushScale);
            jumpHeightScale   --> The scale of the jump height.
            jumpPushScale     --> The scale of the forward push during the jump.

    DESCRIPTION
        This function triggers a jumping mechanic for the car. It uses provided scale factors to determine
        the height and forward momentum of the jump. 

    RETURNS
        Nothing.
    */
    /**/
    public void Jump(float jumpHeightScale, float jumpPushScale)
    {
        if (!isJumping)
        {
            StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale));
        }
    }

    /**/
    /*
    CarController::JumpCo(float jumpHeightScale, float jumpPushScale) CarController::JumpCo(float jumpHeightScale, float jumpPushScale)

    NAME
        CarController::JumpCo - Coroutine for handling the car's jump behavior.

    SYNOPSIS
        IEnumerator CarController::JumpCo(float jumpHeightScale, float jumpPushScale);
            jumpHeightScale   --> The scale of the jump height.
            jumpPushScale     --> The scale of the forward push during the jump.

    DESCRIPTION
        This coroutine manages the detailed behavior of the car's jump. It includes the jump's animation, 
        collision handling, and the effects of the jump on the car's physics. The coroutine uses scale factors
        for height and push to vary the jump's characteristics.

    RETURNS
        IEnumerator that yields the execution at various points, such as waiting for the jump to complete, and resumes afterwards.
    */
    /**/
    private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidbody2D.velocity.magnitude * 0.05f;

        jumpHeightScale = jumpHeightScale * carRigidbody2D.velocity.magnitude * 0.05f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);

        carCollider.enabled = false;

        carSFXHandler.PlayJumpSfx();

        carSpriteRenderer.sortingLayerName = "Midair";
        carShadowRenderer.sortingLayerName = "Midair";

        carRigidbody2D.AddForce(carRigidbody2D.velocity.normalized * jumpPushScale * 0.1f, ForceMode2D.Impulse);

        // Update the sprite renders of the car and its shadow while jumping
        while (isJumping)
        {
            float jumpCompletedPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);
 
            carSpriteRenderer.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;

            carShadowRenderer.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            if (jumpCompletedPercentage == 1.0f)
                break;

            yield return null;
        }

        carCollider.enabled = false;

        // Do not check for collisions with triggers
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;

        Collider2D[] hitResults = new Collider2D[2];

        int numberOfHitObjects = Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter2D, hitResults);

        carCollider.enabled = true;

        // Check if landing is ok based on whether no objects were hit
        if (numberOfHitObjects != 0)
        {
            isJumping = false;

            Jump(0.1f, 0.3f);
        }
        else
        {
            carSpriteRenderer.transform.localScale = Vector3.one;

            carShadowRenderer.transform.localPosition = Vector3.zero;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;

            carCollider.enabled = true;

            carSpriteRenderer.sortingLayerName = "Car";
            carShadowRenderer.sortingLayerName = "Car";

            isJumping = false;
        }

        carSpriteRenderer.transform.localScale = Vector3.one;

        carShadowRenderer.transform.localPosition = Vector3.zero;
        carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;

        if (jumpHeightScale > 0.2f)
        {
            carSFXHandler.PlayLandingSfx();
        }

        carCollider.enabled = true;

        isJumping = false;
    }

    /**/
    /*
    CarController::BoostSpeed(SpeedBoost speedBoost) CarController::BoostSpeed(SpeedBoost speedBoost)

    NAME
        CarController::BoostSpeed - Activates a speed boost for the car.

    SYNOPSIS
        void CarController::BoostSpeed(SpeedBoost speedBoost);
            speedBoost    --> The SpeedBoost object that contains boost parameters.

    DESCRIPTION
        This method initiates a temporary speed boost for the car. It uses parameters from the provided
        SpeedBoost object to increase the car's maximum speed for a specified duration. 

    RETURNS
        Nothing.
    */
    /**/
    public void BoostSpeed(SpeedBoost speedBoost)
    {
        if (isBoosting) return;

        StartCoroutine(BoostCo(speedBoost));
    }

    /**/
    /*
    CarController::BoostCo(SpeedBoost speedBoost) CarController::BoostCo(SpeedBoost speedBoost)

    NAME
        CarController::BoostCo - Coroutine for managing the speed boost effect.

    SYNOPSIS
        IEnumerator CarController::BoostCo(SpeedBoost speedBoost);
            speedBoost   --> The SpeedBoost object that contains boost parameters.

    DESCRIPTION
        This coroutine handles the duration and effects of a speed boost. It temporarily increases the car's
        maximum speed and ensures that the boost lasts for the specified duration before returning the speed
        to normal. 

    RETURNS
        IEnumerator that allows the function to pause when waiting for the boost duration to elapse and resumes afterwards.
    */
    /**/
    private IEnumerator BoostCo(SpeedBoost speedBoost)
    {
        isBoosting = true;
        maxSpeed = boostSpeed; 

        yield return new WaitForSeconds(boostDuration); 

        speedBoost.EndBoost(this); 
        isBoosting = false;
    }

    /**/
    /*
    CarController::DropMudPuddle(float duration) CarController::DropMudPuddle(float duration)

    NAME
        CarController::DropMudPuddle - Drops a mud puddle on the track.

    SYNOPSIS
        void CarController::DropMudPuddle(float duration);
            duration    --> Duration for which the mud puddle remains on the track.

    DESCRIPTION
        This function creates a mud puddle at the car's current position. The mud puddle persists for a specified
        duration and can affect other cars' speed. 

    RETURNS
        Nothing.
    */
    /**/
    public void DropMudPuddle(float duration)
    {
        GameObject mudPuddle = Instantiate(mudPuddlePrefab, transform.position, Quaternion.identity);

        StartCoroutine(RemoveMudPuddleAfterTime(mudPuddle, duration));
    }

    /**/
    /*
    CarController::RemoveMudPuddleAfterTime(GameObject mudPuddle, float duration) CarController::RemoveMudPuddleAfterTime(GameObject mudPuddle, float duration)

    NAME
        CarController::RemoveMudPuddleAfterTime - Removes a mud puddle after a specified duration.

    SYNOPSIS
        IEnumerator CarController::RemoveMudPuddleAfterTime(GameObject mudPuddle, float duration);
            mudPuddle   --> The mud puddle GameObject to be removed.
            duration    --> The duration after which the mud puddle will be removed.

    DESCRIPTION
        This coroutine waits for a specified duration before removing a mud puddle from the track. 

    RETURNS
        IEnumerator that temporarily halts execution for the duration of the mud puddle's presence.
    */
    /**/
    private IEnumerator RemoveMudPuddleAfterTime(GameObject mudPuddle, float duration)
    {
        yield return new WaitForSeconds(duration); 

        Destroy(mudPuddle);
    }

    /**/
    /*
    CarController::OnTriggerEnter2D(Collider2D collider2d) CarController::OnTriggerEnter2D(Collider2D collider2d)

    NAME
        CarController::OnTriggerEnter2D - Handles trigger collision events.

    SYNOPSIS
        void CarController::OnTriggerEnter2D(Collider2D collider2d);
            collider2d    --> The Collider2D component of the object the car has collided with.

    DESCRIPTION
        This method is called when the car enters a trigger collider. When the car hits a jump trigger, 
        it executes a jump with parameters defined by the trigger.

    RETURNS
        Nothing.
    */
    /**/
    void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.CompareTag("Jump"))
        {
            CarJumpData jumpData = collider2d.GetComponent<CarJumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
    }
}

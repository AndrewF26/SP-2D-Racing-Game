using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**/
/*
    This class is responsible for controlling the AI behavior of cars in the game environment. 
    It encompasses a range of functionalities including following waypoints or players, obstacle avoidance, 
    dynamic speed adjustment based on skill level, and decision-making during different AI modes. 
    The class integrates various aspects such as raycasting for obstacle detection, waypoint navigation, 
    skill-based speed control, stuck detection, and dynamic pathfinding using AStar algorithm. 
    It also handles AI responses to environmental factors, like avoiding other cars and using items randomly.
*/
/**/
public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, followWaypoint};

    [Header("AI Settings")]
    public AIMode mode;
    public float maxSpeed = 8;
    public bool isAvoidingCars = true;
    [Range(0f, 1f)]
    public float skillLevel = 1.0f;

    Vector3 targetPosition = Vector3.zero;
    float origMaxSpeed = 0;

    bool isRunningStuckCheck = false;
    bool isFirstTempWaypoint = false;
    int stuckCheckCounter = 0;
    List<Vector2> tempWaypoints = new List<Vector2>();
    float angleToTarget = 0;

    private Vector2 avoidanceVectorLerped = Vector2.zero;

    Waypoint currentWaypoint = null;
    Waypoint previousWaypoint = null;
    Waypoint[] allWaypoints;

    PolygonCollider2D polygonCollider2D;

    CarController carController;
    AStarPath aStarLite;

    /**/
    /*
    CarAIHandler::Awake() CarAIHandler::Awake()

    NAME
        CarAIHandler::Awake - Initializes components and settings for AI-controlled car.

    SYNOPSIS
        void CarAIHandler::Awake();

    DESCRIPTION
        This function is called when the script instance is being loaded. It initializes the car controller, 
        waypoints, and other components like PolygonCollider2D and AStarPath. It also sets the original 
        maximum speed and adjusts the skill level based on the GameManager's AI difficulty setting.

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        carController = GetComponent<CarController>();
        allWaypoints = FindObjectsOfType<Waypoint>();

        polygonCollider2D = GetComponentInChildren<PolygonCollider2D>();

        origMaxSpeed = maxSpeed;

        aStarLite = GetComponent<AStarPath>();

        skillLevel = GameManager.instance != null ? GameManager.instance.AIDifficulty : 1.0f;
    }

    /**/
    /*
    CarAIHandler::Start() CarAIHandler::Start()

    NAME
        CarAIHandler::Start - Sets the initial maximum speed of the AI-controlled car.

    SYNOPSIS
        void CarAIHandler::Start();

    DESCRIPTION
        This function sets the initial maximum speed of the AI-controlled car based on its skill level. 
        This is used to adjust the car's behavior at the start of the game scene.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        SetSkillMaxSpeed(maxSpeed);
    }

    /**/
    /*
    CarAIHandler::FixedUpdate() CarAIHandler::FixedUpdate()

    NAME
        CarAIHandler::FixedUpdate - Main AI behavior logic for obstacle detection and movement.

    SYNOPSIS
        void CarAIHandler::FixedUpdate();

    DESCRIPTION
        This function is called at a fixed interval and contains the main logic for the AI behavior.
        It manages obstacle detection, waypoint following, input vector adjustments, and stuck check
        routines. It also handles item usage based on random probability.

    RETURNS
        Nothing.
    */
    /**/
    void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown)
            return;

        Vector2 inputVector = Vector2.zero;

        if (IsObstacleAhead(out RaycastHit2D hit, 10f)) // 5f is the detection distance, adjust as needed
        {
            // Logic to avoid the obstacle
            AvoidObstacle(hit, ref inputVector);
        }
        else
        {
            // Regular AI behavior when no obstacle is detected
            switch (mode)
            {
                case AIMode.followWaypoint:
                    if (tempWaypoints.Count == 0)
                        FollowWaypoint();
                    else FollowTempWayPoints();
                    break;
            }

            inputVector.x = TurnToTarget();
            inputVector.y = ApplyBrake(inputVector.x);
        }

        // Checks if the car is stuck if the AI is unable to gain any speed.
        if (carController.GetVelocityMagnitude() < 0.5f && Mathf.Abs(inputVector.y) > 0.01f && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCO());

        // Check if car is still stuck after reversing. If not, drive forward.
        if (stuckCheckCounter >= 4 && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCO());

        if (Random.value < 0.01f && carController.currentItem != null)
        {
            carController.currentItem.Use(carController);
            carController.currentItem = null;
        }

        carController.SetInputVector(inputVector);
    }

    /**/
    /*
    CarAIHandler::FollowWaypoint() CarAIHandler::FollowWaypoint()

    NAME
        CarAIHandler::FollowWaypoint - Logic for following a waypoint in the game.

    SYNOPSIS
        void CarAIHandler::FollowWaypoint();

    DESCRIPTION
        This function manages the AI's behavior when it is set to follow waypoints. It determines the target
        position based on the current and next waypoints, adjusts the car's speed and direction towards the
        target, and switches waypoints once the current one is reached.

    RETURNS
        Nothing.
    */
    /**/
    void FollowWaypoint()
    {
        // Pick the closest waypoint if none are set.
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWaypoint();
            previousWaypoint = currentWaypoint;
        }

        // Set the target on the waypoint's position
        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            float distToWaypoint = (targetPosition - transform.position).magnitude;

            if (distToWaypoint > 10)
            {
                Vector3 nearestPointOnLine = FindNearestPoint(previousWaypoint.transform.position, currentWaypoint.transform.position, transform.position);

                float segments = distToWaypoint / 20.0f;

                targetPosition = (targetPosition + nearestPointOnLine * segments) / (segments + 1);
            }

            // Check if close enough to consider whether the waypoint has been reached
            if (distToWaypoint <= currentWaypoint.minDistance)
            {
                if (currentWaypoint.maxSpeed > 0)
                    SetSkillMaxSpeed(currentWaypoint.maxSpeed);
                else
                    SetSkillMaxSpeed(1000);

                previousWaypoint = currentWaypoint;

                currentWaypoint = currentWaypoint.nextWaypoint[Random.Range(0, currentWaypoint.nextWaypoint.Length)];
            }
        }
    }

    /**/
    /*
    CarAIHandler::FollowTempWayPoints() CarAIHandler::FollowTempWayPoints()

    NAME
        CarAIHandler::FollowTempWayPoints - Follows temporary waypoints generated during stuck checks.

    SYNOPSIS
        void CarAIHandler::FollowTempWayPoints();

    DESCRIPTION
        This function is used when the AI car is following a set of temporary waypoints, usually generated
        after a stuck check. It guides the car along these waypoints until it reaches the target position or
        resumes its normal waypoint following behavior.

    RETURNS
        Nothing.
    */
    /**/
    void FollowTempWayPoints()
    { 
        targetPosition = tempWaypoints[0];

        float distanceToWayPoint = (targetPosition - transform.position).magnitude;

        SetSkillMaxSpeed(5);

        float minDistanceToWaypoint = 1.5f;

        if (!isFirstTempWaypoint)
            minDistanceToWaypoint = 3.0f;

        if (distanceToWayPoint <= minDistanceToWaypoint)
        {
            tempWaypoints.RemoveAt(0);
            isFirstTempWaypoint = false;
        }
    }
	
	/**/
	/*
    CarAIHandler::FindClosestWaypoint() CarAIHandler::FindClosestWaypoint()

    NAME
        CarAIHandler::FindClosestWaypoint - Finds the closest waypoint to the AI car.

    SYNOPSIS
        Waypoint CarAIHandler::FindClosestWaypoint();

    DESCRIPTION
        This function searches through all available waypoints and returns the one that is closest to the
        AI car's current position. It's used to determine the next target waypoint for the AI to follow.

    RETURNS
        The closest waypoint to the AI car.
    */
	/**/
    Waypoint FindClosestWaypoint()
    {
        return allWaypoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }
	
	/**/
	/*
    CarAIHandler::TurnToTarget() CarAIHandler::TurnToTarget()

    NAME
        CarAIHandler::TurnToTarget - Calculates the steering amount towards the target.

    SYNOPSIS
        float CarAIHandler::TurnToTarget();

    DESCRIPTION
        This function calculates the amount of steering needed for the AI to turn towards its current target.
        It considers the current direction of the car and the position of the target, adjusting the steering
        to align the car towards the target.

    RETURNS
        Float steerAmount: The amount of steering required to turn towards the target.
    */
	/**/
    float TurnToTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        // Calculate an angle towards the target
        angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        float steerAmount = angleToTarget / 45.0f;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    /**/
    /*
    CarAIHandler::ApplyBrake(float inputX) CarAIHandler::ApplyBrake(float inputX)

    NAME
        CarAIHandler::ApplyBrake - Determines the brake intensity based on input.

    SYNOPSIS
        float CarAIHandler::ApplyBrake(float inputX);
            inputX        --> The steering input magnitude.

    DESCRIPTION
        This function calculates the intensity of brake to be applied by the AI. It takes into account 
        the current speed of the car, the maximum speed allowed, the input steering magnitude, 
        and the skill level. It adjusts the brake based on cornering needs and stuck checks.

    RETURNS
        Float brake: The calculated brake intensity.
    */
    /**/
    float ApplyBrake(float inputX)
    {
        // Prevent further acceleration if going too fast
        if (carController.GetVelocityMagnitude() > maxSpeed)
            return 0;

        float reduceCornerSpeed = Mathf.Abs(inputX) / 1.0f;

        float brake = 1.05f - reduceCornerSpeed * skillLevel;

        // Handle braking differently when we are following temp waypoints
        if (tempWaypoints.Count() != 0)
        {
            if (angleToTarget > 70)
                brake = brake * -1;
            else if (angleToTarget < -70)
                brake = brake * -1; 
            else if (stuckCheckCounter > 3)
                brake = brake * -1;
        }

        return brake;
    }

    /**/
    /*
    CarAIHandler::SetSkillMaxSpeed(float newSpeed) CarAIHandler::SetSkillMaxSpeed(float newSpeed)

    NAME
        CarAIHandler::SetSkillMaxSpeed - Sets the AI's maximum speed based on skill level.

    SYNOPSIS
        void CarAIHandler::SetSkillMaxSpeed(float newSpeed);
            newSpeed       --> The new maximum speed to be set.

    DESCRIPTION
        This function sets the maximum speed for the AI-controlled car. The speed is adjusted based on the 
        car's skill level. It ensures that the maximum speed does not exceed the original maximum speed 
        set for the car.

    RETURNS
        Nothing.
    */
    /**/
    void SetSkillMaxSpeed(float newSpeed)
    {
        maxSpeed = Mathf.Clamp(newSpeed, 0, origMaxSpeed);

        float skillbasedMaxSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed = maxSpeed * skillbasedMaxSpeed;
    }

    /**/
    /*
    CarAIHandler::SetSkillLevel(float skillLevel) CarAIHandler::SetSkillLevel(float skillLevel)

    NAME
        CarAIHandler::SetSkillLevel - Sets the skill level of the AI.

    SYNOPSIS
        void CarAIHandler::SetSkillLevel(float skillLevel);
            skillLevel    --> The skill level to be set for the AI.

    DESCRIPTION
        This function sets the skill level of the AI, which influences its driving behavior and decision-making.
        A higher skill level typically results in more aggressive and efficient driving behavior.

    RETURNS
        Nothing.
    */
    /**/
    public void SetSkillLevel(float skillLevel)
    {
        this.skillLevel = skillLevel;
        SetSkillMaxSpeed(maxSpeed); // Update max speed based on new skill level
    }

    /**/
    /*
    CarAIHandler::FindNearestPoint(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point) CarAIHandler::FindNearestPoint(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)

    NAME
        CarAIHandler::FindNearestPoint - Finds the nearest point on a line to a given point.

    SYNOPSIS
        Vector2 CarAIHandler::FindNearestPoint(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point);
            lineStartPosition  --> The start position of the line.
            lineEndPosition    --> The end position of the line.
            point              --> The point to which the nearest point on the line is to be found.

    DESCRIPTION
        This function calculates the nearest point on a specified line to a given point. This is used for
        pathfinding and steering calculations, where the AI needs to determine the closest approach to its
        path or target.

    RETURNS
        The nearest point on the line to the specified point.
    */
    /**/
    Vector2 FindNearestPoint(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);

        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);

        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    /**/
    /*
    CarAIHandler::CheckForCars(out Vector3 position, out Vector3 otherCarRightVector) CarAIHandler::CheckForCars(out Vector3 position, out Vector3 otherCarRightVector)

    NAME
        CarAIHandler::CheckForCars - Checks for the presence of other cars nearby.

    SYNOPSIS
        bool CarAIHandler::CheckForCars(out Vector3 position, out Vector3 otherCarRightVector);
            position              --> Out parameter to store the position of the detected car.
            otherCarRightVector   --> Out parameter to store the right vector of the detected car.

    DESCRIPTION
        This function performs a check to see if there are other cars in close proximity to the AI car.
        It uses raycasting to detect other cars and, if found, provides their position and orientation.
        This is used for collision avoidance and pathfinding.

    RETURNS
        True if another car is detected, False otherwise.
    */
    /**/
    bool CheckForCars(out Vector3 position, out Vector3 otherCarRightVector)
    {
        polygonCollider2D.enabled = false;

        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 0.5f, transform.up, 8, 1 << LayerMask.NameToLayer("Car"));

        polygonCollider2D.enabled = true;

        if (raycastHit2D.collider != null)
        {
            position = raycastHit2D.collider.transform.position;
            otherCarRightVector = raycastHit2D.collider.transform.right;
            return true;
        }
       
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    /**/
    /*
    CarAIHandler::AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget) CarAIHandler::AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)

    NAME
        CarAIHandler::AvoidCars - Adjusts the car's path to avoid other cars.

    SYNOPSIS
        void CarAIHandler::AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget);
            vectorToTarget        --> The current vector towards the target.
            newVectorToTarget     --> Out parameter for the adjusted vector after avoiding other cars.

    DESCRIPTION
        This function modifies the car's current path to avoid collisions with other cars. It calculates an
        avoidance vector and combines it with the current path vector to steer the AI car away from other vehicles.

    RETURNS
        Nothing.
    */
    /**/
    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (CheckForCars(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;
 
            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            float driveToTargetInfluence = 6.0f / distanceToTarget;
  
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.30f, 1.0f);

            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            newVectorToTarget = vectorToTarget * driveToTargetInfluence + avoidanceVector * avoidanceInfluence;
            newVectorToTarget.Normalize();

            return;
        }

        newVectorToTarget = vectorToTarget;
    }

    /**/
    /*
    CarAIHandler::IsObstacleAhead(out RaycastHit2D hit, float detectionDistance) CarAIHandler::IsObstacleAhead(out RaycastHit2D hit, float detectionDistance)

    NAME
        CarAIHandler::IsObstacleAhead - Checks for obstacles ahead of the car.

    SYNOPSIS
        bool CarAIHandler::IsObstacleAhead(out RaycastHit2D hit, float detectionDistance);
            hit                  --> RaycastHit2D object to store information about the obstacle hit.
            detectionDistance    --> Distance within which to check for obstacles.

    DESCRIPTION
        This function uses raycasting to detect if there are any obstacles in the car's path within a specified
        distance. It helps in determining whether the car needs to take evasive action to avoid a collision.

    RETURNS
        True if an obstacle is detected, False otherwise.
    */
    /**/
    bool IsObstacleAhead(out RaycastHit2D hit, float detectionDistance)
    {
        hit = Physics2D.Raycast(transform.position, transform.up, detectionDistance, LayerMask.GetMask("Obstacle")); 
        return hit.collider != null;
    }

    /**/
    /*
    CarAIHandler::AvoidObstacle(RaycastHit2D hit, ref Vector2 inputVector) CarAIHandler::AvoidObstacle(RaycastHit2D hit, ref Vector2 inputVector)

    NAME
        CarAIHandler::AvoidObstacle - Manages the car's response to detected obstacles.

    SYNOPSIS
        void CarAIHandler::AvoidObstacle(RaycastHit2D hit, ref Vector2 inputVector);
            hit            --> The RaycastHit2D object containing information about the detected obstacle.
            inputVector    --> The current input vector for the car, to be adjusted for obstacle avoidance.

    DESCRIPTION
        This function adjusts the car's input vector to avoid an obstacle detected in its path. It calculates
        an avoidance vector based on the obstacle's position and orientation, and smoothly transitions the
        car's current direction to this new vector to steer clear of the obstacle.

    RETURNS
        Nothing.
    */
    /**/
    void AvoidObstacle(RaycastHit2D hit, ref Vector2 inputVector)
    {
        Vector2 directionToObstacle = hit.point - (Vector2)transform.position;
        Vector2 avoidanceVector = Vector2.Reflect(directionToObstacle.normalized, hit.normal);

        avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);

        float angle = Vector2.SignedAngle(transform.up, avoidanceVectorLerped);
        inputVector.x = Mathf.Clamp(-angle / 45.0f, -1.0f, 1.0f);
    }

    /**/
    /*
    CarAIHandler::StuckCheckCO() CarAIHandler::StuckCheckCO()

    NAME
        CarAIHandler::StuckCheckCO - Coroutine for checking if the AI car is stuck.

    SYNOPSIS
        IEnumerator CarAIHandler::StuckCheckCO();

    DESCRIPTION
        This coroutine is executed to determine if the AI-controlled car is stuck. It checks the car's position
        over a period of time to see if there has been significant movement. If the car is deemed to be stuck,
        it calculates a new path using the AStar algorithm to navigate around the obstacle.

    RETURNS
        IEnumerator that yields execution for a set duration and then performs checks and actions based on the AI car's movement.
    */
    /**/
    IEnumerator StuckCheckCO()
    {
        Vector3 initialStuckPosition = transform.position;

        isRunningStuckCheck = true;

        yield return new WaitForSeconds(0.7f);

        // If the car has not moved after waiting, then its stuck
        if ((transform.position - initialStuckPosition).sqrMagnitude < 3)
        {
            tempWaypoints = aStarLite.FindPath(currentWaypoint.transform.position);

            if (tempWaypoints == null)
                tempWaypoints = new List<Vector2>();

            stuckCheckCounter++;

            isFirstTempWaypoint = true;
        }
        else stuckCheckCounter = 0;

        isRunningStuckCheck = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class is responsible for detecting and managing the type of surface a car is driving on in the game. 
    It uses physics collision detection to determine the surface underneath the car and updates the current surface type accordingly. 
*/
/**/
public class SurfaceHandler : MonoBehaviour
{
    [Header("Surface Detection")]
    public LayerMask surfaceLayer;

    Collider2D[] surfaceCollidersHit = new Collider2D[10];
    Vector3 lastSurfacePosition = Vector3.one * 10000;

    Surface.SurfaceTypes onSurface = Surface.SurfaceTypes.Road;

    Collider2D carCollider;

    /**/
    /*
    SurfaceHandler::Awake() SurfaceHandler::Awake()

    NAME
        SurfaceHandler::Awake - Initializes the SurfaceHandler component.

    SYNOPSIS
        void SurfaceHandler::Awake();

    DESCRIPTION
        This method initializes the SurfaceHandler by finding and storing the Collider2D component of the car. 

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        carCollider = GetComponentInChildren<Collider2D>();
    }

    /**/
    /*
    SurfaceHandler::Update() SurfaceHandler::Update()

    NAME
        SurfaceHandler::Update - Updates the car's surface detection.

    SYNOPSIS
        void SurfaceHandler::Update();

    DESCRIPTION
        This method checks the car's position and detects the surface type the car is currently on. 
        If the car has moved sufficiently, it updates the 'onSurface' variable to reflect the new surface type. 

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        if ((transform.position - lastSurfacePosition).sqrMagnitude < 0.75f)
            return;

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.layerMask = surfaceLayer;
        contactFilter2D.useLayerMask = true;
        contactFilter2D.useTriggers = true;

        int numOfHits = Physics2D.OverlapCollider(carCollider, contactFilter2D, surfaceCollidersHit);

        float lastSurfaceValue = -1000;

        for (int i = 0; i < numOfHits; i++)
        {
            Surface surface = surfaceCollidersHit[i].GetComponent<Surface>();

            if (surface.transform.position.z > lastSurfaceValue)
            {
                onSurface = surface.surfaceType;
                lastSurfaceValue = surface.transform.position.z;
            }
        }

        if (numOfHits == 0)
            onSurface = Surface.SurfaceTypes.Road;

        lastSurfacePosition = transform.position;
    }
	
	/**/
	/*
    SurfaceHandler::GetCurrentSurface() SurfaceHandler::GetCurrentSurface()

    NAME
        SurfaceHandler::GetCurrentSurface - Retrieves the current surface type.

    SYNOPSIS
        public Surface.SurfaceTypes GetCurrentSurface();

    DESCRIPTION
        This method returns the type of surface the car is currently on. It is used to adjust various 
        aspects of the car's behavior, such as handling and sound effects, depending on the surface type.

    RETURNS
        The current surface type the car is on.
    */
	/**/
    public Surface.SurfaceTypes GetCurrentSurface()
    {
        return onSurface;
    }
}

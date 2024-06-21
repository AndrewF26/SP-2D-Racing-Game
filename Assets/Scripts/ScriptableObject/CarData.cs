using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Car Data", menuName = "Car Data", order = 51)]

/**/
/*
    CarData is a ScriptableObject class used to store data about cars in the game. It includes information 
    such as a unique identifier for each car, a sprite for the UI representation, a prefab for the car object, 
    and the cost of the car. 
*/
/**/
public class CarData : ScriptableObject
{
    [SerializeField]
    private int carUniqueID = 0;

    [SerializeField]
    private Sprite carUISprite;

    [SerializeField]
    private GameObject carPrefab;

    [SerializeField]
    private int cost;

    /**/
    /*
    CarData::CarUniqueID CarData::CarUniqueID

    NAME
        CarData::CarUniqueID - Getter for the car's unique identifier.

    SYNOPSIS
        int CarUniqueID

    DESCRIPTION
        Provides read-only access to the car's unique identifier, which is used to distinguish
        different cars within the game.

    RETURNS
        Int carUniqueID: The unique identifier of the car.
    */
    /**/
    public int CarUniqueID
    {
        get { return carUniqueID; }
    }

    /**/
    /*
    CarData::CarUISprite CarData::CarUISprite

    NAME
        CarData::CarUISprite - Getter for the car's UI sprite.

    SYNOPSIS
        Sprite CarUISprite

    DESCRIPTION
        Provides read-only access to the Sprite object representing the car in the game's user interface.
        This sprite is utilized for displaying the car the menu.

    RETURNS
        Sprite carUISprite: The sprite used for the car's UI representation.
    */
    /**/
    public Sprite CarUISprite
    {
        get { return carUISprite; }
    }

    /**/
    /*
    CarData::CarPrefab CarData::CarPrefab

    NAME
        CarData::CarPrefab - Getter for the car's prefab.

    SYNOPSIS
        GameObject CarPrefab

    DESCRIPTION
        Provides read-only access to the GameObject prefab of the car, which is instantiated in the game
        to create a playable version of the car.

    RETURNS
        GameObject carPrefab: The prefab representing the car in the game.
    */
    /**/
    public GameObject CarPrefab
    {
        get { return carPrefab; }
    }

    /**/
    /*
    CarData::Cost CarData::Cost

    NAME
        CarData::Cost - Getter for the cost of the car.

    SYNOPSIS
        int Cost

    DESCRIPTION
        Provides read-only access to the cost of the car, used in in-game transactions like purchasing cars.

    RETURNS
        Int cost: The cost of the car.
    */
    /**/
    public int Cost 
    {
        get { return cost; }
    }
}

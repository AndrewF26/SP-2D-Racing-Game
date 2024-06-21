using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class represents different types of surfaces that can be encountered in the game, such as road, grass, sand, etc. 
    Each surface type has a different effect on the cars' speed. 
*/
/**/
public class Surface : MonoBehaviour
{
    public enum SurfaceTypes { Road, Grass, Sand, Mud };

    [Header("Surface")]
    public SurfaceTypes surfaceType;

}

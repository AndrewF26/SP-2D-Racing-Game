using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class is a simple data container that holds parameters for car jumping mechanics in the game. 
    It defines the scale of jump height and push, which can be adjusted to customize the jumping behavior of cars. 
*/
/**/
public class CarJumpData : MonoBehaviour
{
    [Header("Jump Settings")]

    public float jumpHeightScale = 1.0f;
    public float jumpPushScale = 1.0f;
}

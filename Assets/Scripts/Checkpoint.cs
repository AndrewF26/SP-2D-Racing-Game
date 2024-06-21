using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    The Checkpoint class represents a checkpoint or a finish line in the game environment. It holds essential 
    information about each checkpoint, such as whether it is a finish line and its sequential number in the race course. 
*/
/**/
public class Checkpoint : MonoBehaviour
{
    public bool isFinishLine = false;
    public int checkpointNum = 1;
}

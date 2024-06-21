using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CarAIHandler;

/**/
/*
    This class is a key component for guiding AI-controlled cars along the track. Each waypoint represents 
	a position on the track that AI cars aim to reach, effectively forming a path for them to follow. 
    Waypoints can influence AI behavior by specifying parameters like the maximum speed AI cars should aim for 
    when approaching a waypoint, and the minimum distance at which the waypoint is considered reached. 
*/
/**/
public class Waypoint : MonoBehaviour
{
    [Header("Waypoints")]
    public float maxSpeed = 0;

    public float minDistance = 5;

    public Waypoint[] nextWaypoint;
}

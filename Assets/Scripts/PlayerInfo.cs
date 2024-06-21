using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    The PlayerInfo class is designed to store and manage information about both the human player and AI. 
    It holds data such as player number, name, the unique ID of the selected car, AI status, and performance metrics like last race 
    position, top speed, race completion time, and score. This class ensures that each player's data is encapsulated and managed 
    efficiently, allowing for easy access and modification throughout the game.
*/
/**/
public class PlayerInfo
{
    public int playerNum = 0;
    public string name = "";
    public int carUniqueID = 0;
    public bool isAI = false;
    public int lastRacePosition = 0;
    public float topSpeed = 0f;
    public float raceCompletionTime = 0f;
    public int score = 0;

    /**/
    /*
    PlayerInfo::PlayerInfo(int playerNum, string name, int carUniqueID, bool isAI) PlayerInfo::PlayerInfo(int playerNum, string name, int carUniqueID, bool isAI)

    NAME
        PlayerInfo::PlayerInfo - Constructor for creating a PlayerInfo object.

    SYNOPSIS
        PlayerInfo::PlayerInfo(int playerNum, string name, int carUniqueID, bool isAI);
            playerNum     --> The number identifying the player.
            name          --> The name of the player.
            carUniqueID   --> The unique ID of the player's chosen car.
            isAI          --> Indicates whether the player is controlled by AI.

    DESCRIPTION
        This constructor initializes a new instance of the PlayerInfo class. It sets up and manages the player's 
        identification number, name, unique ID for the selected car, and whether the player is an AI.

    RETURNS
        A new instance of PlayerInfo.
    */
    /**/
    public PlayerInfo(int playerNum, string name, int carUniqueID, bool isAI)
    {
        this.playerNum = playerNum;
        this.name = name;
        this.carUniqueID = carUniqueID;
        this.isAI = isAI;
    }
}

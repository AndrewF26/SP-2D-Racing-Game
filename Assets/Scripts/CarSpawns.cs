using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**/
/*
    This class is responsible for initializing and spawning player cars at the start of a race. It dynamically 
    places cars at designated spawn points according to the players' choices and game settings. The class handles 
    the instantiation of car prefabs, aligning them with players' preferences and assigning control mechanisms based on 
    whether the player is an AI or a human. 
*/
/**/
public class CarSpawns : MonoBehaviour
{
    /**/
    /*
    CarSpawns::Start() CarSpawns::Start()

    NAME
        CarSpawns::Start - Initializes and spawns player cars at the beginning of the game.

    SYNOPSIS
        void CarSpawns::Start();

    DESCRIPTION
        This function locates all available spawn points and assigns cars to players based on their selected preferences. 
        It instantiates car prefabs at the spawn points, adjusting their settings for AI or player control based on the GameManager's
        player information.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        CarData[] allCarData = Resources.LoadAll<CarData>("CarData/");

        List<PlayerInfo> playerInfoList = new List<PlayerInfo>(GameManager.instance.GetPlayerList());

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;

            if (playerInfoList.Count == 0)
            {
                return;
            }

            PlayerInfo playerInfo = playerInfoList[0];

            int selectedCarID = playerInfo.carUniqueID;

            foreach (CarData carData in allCarData)
            {
                if (carData.CarUniqueID == selectedCarID)
                {
                    GameObject car = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);

                    car.name = playerInfo.name;

                    if (playerInfo.isAI)
                    {
                        car.GetComponent<CarInputHandler>().enabled = false;
                        car.tag = "AI";
                    }
                    else 
                    {
                        car.GetComponent<CarAIHandler>().enabled = false;
                        car.GetComponent<AStarPath>().enabled = false;
                        car.tag = "Player";
                    }

                    break;
                }
            }

            playerInfoList.Remove(playerInfo);
        }
    }
}

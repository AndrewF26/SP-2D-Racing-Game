using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class contains initialization logic that runs when the game loads, prior to the first scene being loaded. 
    Its primary function is to instantiate a set of predefined GameObjects that are required to be present from the very 
    beginning of the game.
*/
/**/
public class Startup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    /*
    Startup::InstantiatePrefabs() Startup::InstantiatePrefabs()

    NAME
        Startup::InstantiatePrefabs - Instantiates prefabs when the game loads.

    SYNOPSIS
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InstantiatePrefabs();

    DESCRIPTION
        This static method loads and instantiates a set of predefined GameObjects from the "InstantiateOnLoad" resources folder. 

    RETURNS
        Nothing.
    */
    public static void InstantiatePrefabs()
    {
        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject pref in prefabsToInstantiate)
        {
            GameObject.Instantiate(pref);
        }
    }
}

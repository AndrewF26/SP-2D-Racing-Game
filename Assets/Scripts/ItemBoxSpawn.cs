using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class handles the spawning and respawning of item boxes in the game. It controls the placement and timing for
    the appearance of item boxes, which provide players with race items. This class uses a prefab for the item box, 
    and can initiate a respawn after a set delay, allowing for consistent and timed distribution of item boxes 
    throughout the race. 
*/
/**/
public class ItemBoxSpawn : MonoBehaviour
{
    public GameObject itemBoxPrefab; 
    public float respawnTime = 5f; 

    /**/
    /*
    ItemBoxSpawn::Start() ItemBoxSpawn::Start()

    NAME
        ItemBoxSpawn::Start - Initializes the item box spawn.

    SYNOPSIS
        void ItemBoxSpawn::Start();

    DESCRIPTION
        This method calls the SpawnItemBox method to initially spawn an item box at the location of this GameObject. 

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        SpawnItemBox();
    }

    /**/
    /*
    ItemBoxSpawn::SpawnItemBox() ItemBoxSpawn::SpawnItemBox()

    NAME
        ItemBoxSpawn::SpawnItemBox - Spawns an item box.

    SYNOPSIS
        void ItemBoxSpawn::SpawnItemBox();

    DESCRIPTION
        This method instantiates an item box at the spawn location. It is used both for initial item box
        spawning and for respawning item boxes after they have been destroyed.

    RETURNS
        Nothing.
    */
    /**/
    void SpawnItemBox()
    {
        Instantiate(itemBoxPrefab, transform.position, transform.rotation, transform);
    }

    /**/
    /*
    ItemBoxSpawn::RespawnItemBox() ItemBoxSpawn::RespawnItemBox()

    NAME
        ItemBoxSpawn::RespawnItemBox - Initiates the respawn of an item box.

    SYNOPSIS
        void ItemBoxSpawn::RespawnItemBox();

    DESCRIPTION
        This method starts the RespawnCoroutine to respawn an item box after a defined delay. 

    RETURNS
        Nothing.
    */
    /**/
    public void RespawnItemBox()
    {
        StartCoroutine(RespawnCoroutine());
    }

    /**/
    /*
    ItemBoxSpawn::RespawnCoroutine() ItemBoxSpawn::RespawnCoroutine()

    NAME
        ItemBoxSpawn::RespawnCoroutine - Coroutine for respawning an item box.

    SYNOPSIS
        IEnumerator ItemBoxSpawn::RespawnCoroutine();

    DESCRIPTION
        This coroutine waits for a specified amount of time defined by respawnTime, then calls
        SpawnItemBox to respawn an item box. 

    RETURNS
        IEnumerator that temporarily halts execution for the duration of respawnTime.
    */
    /**/
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnItemBox();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class represents item boxes in the game, which players can interact with to receive race items like 
    speed boosts or obstacles. This class manages the item box's behavior, including its interactions with cars, the 
    random selection of items, triggering animations, and handling the item box's destruction and respawn. When a car 
    collides with an item box, the box grants a random item to the car and then initiates a destruction animation. 
    The class also coordinates with the ItemBoxSpawn to manage the respawn of item boxes after they are destroyed.
*/
/**/
public class ItemBox : MonoBehaviour
{
    private Animator animator;
    private bool isDestroyed = false;
    public float destructionDelay = 2f;

    /**/
    /*
    ItemBox::Awake() ItemBox::Awake()

    NAME
        ItemBox::Awake - Initializes the ItemBox component.

    SYNOPSIS
        void ItemBox::Awake();

    DESCRIPTION
        This method initializes the Animator component which is used to handle animations for the item box.

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /**/
    /*
    ItemBox::OnTriggerEnter2D(Collider2D collider) ItemBox::OnTriggerEnter2D(Collider2D collider)

    NAME
        ItemBox::OnTriggerEnter2D - Handles the interaction when an object enters its trigger.

    SYNOPSIS
        void ItemBox::OnTriggerEnter2D(Collider2D collider);
            collider   --> The collider of the object that entered the trigger.

    DESCRIPTION
        This method is triggered when an object enters the item box's trigger collider. If the item box
        is not already destroyed, it checks if the collider belongs to a CarController. If so, it assigns
        a random race item to the car, triggers the destruction animation, and notifies the ItemBoxSpawn
        for a respawn if necessary.

    RETURNS
        Nothing.
    */
    /**/
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isDestroyed)
        {
            CarController carController = collider.GetComponentInParent<CarController>();
            if (carController != null)
            {
                RaceItem item = GetRandomItem();
                carController.AssignItem(item);
                TriggerDestructionAnimation();

                ItemBoxSpawn spawner = GetComponentInParent<ItemBoxSpawn>();
                if (spawner != null)
                {
                    spawner.RespawnItemBox();
                }
            }
        }
    }

    /**/
    /*
    ItemBox::TriggerDestructionAnimation() ItemBox::TriggerDestructionAnimation()

    NAME
        ItemBox::TriggerDestructionAnimation - Triggers the destruction animation of the item box.

    SYNOPSIS
        void ItemBox::TriggerDestructionAnimation();

    DESCRIPTION
        This method is responsible for triggering the destruction animation of the item box. It marks
        the item box as destroyed to prevent further interactions and starts a coroutine to destroy the
        item box game object after a delay, allowing the animation to complete.

    RETURNS
        Nothing.
    */
    /**/
    void TriggerDestructionAnimation()
    {
        isDestroyed = true;
        animator.SetTrigger("Destroy"); 
        StartCoroutine(DestroyAfterDelay());
    }

    /**/
    /*
    ItemBox::DestroyAfterDelay() ItemBox::DestroyAfterDelay()

    NAME
        ItemBox::DestroyAfterDelay - Coroutine to destroy the item box after a delay.

    SYNOPSIS
        IEnumerator ItemBox::DestroyAfterDelay();

    DESCRIPTION
        This coroutine waits for a specified delay, then destroys the item box game object. This delay
        allows the destruction animation to complete before the object is removed from the scene.

    RETURNS
        IEnumerator that allows the function to pause when waiting for the duration of destructionDelay to elapse and resumes afterwards.
    */
    /**/
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destructionDelay);
        Destroy(gameObject);
    }
	
	/**/
	/*
    ItemBox::GetRandomItem() ItemBox::GetRandomItem()

    NAME
        ItemBox::GetRandomItem - Selects a random race item.

    SYNOPSIS
        RaceItem ItemBox::GetRandomItem();

    DESCRIPTION
        This method randomly selects and returns a race item from a predefined set of items. 

    RETURNS
        The randomly selected race item.
    */
	/**/
    RaceItem GetRandomItem()
    {
        int randomIndex = Random.Range(0, 2);
        switch (randomIndex)
        {
            case 0:
                return new SpeedBoost();
            case 1:
                return new MudPuddle();
            default:
                return new SpeedBoost();
        }
    }
}

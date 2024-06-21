using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    This class is responsible for managing the user interface elements related to displaying car information in the game. 
    It controls UI elements like car images and price tags, and manages animations for car selection scenarios. 
*/
/**/
public class CarUIHandler : MonoBehaviour
{
    [Header("Car Details")]
    public Image carImage;
    public TMP_Text carPriceText;

    Animator animator = null;

    /**/
    /*
    CarUIHandler::Awake() CarUIHandler::Awake()

    NAME
        CarUIHandler::Awake - Initializes the CarUIHandler component.

    SYNOPSIS
        void CarUIHandler::Awake();

    DESCRIPTION
        This method initializes the CarUIHandler by finding and storing the Animator component within the child objects. 
        The Animator is used for handling UI animations related to car selection.

    RETURNS
        Nothing.
    */
    /**/
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    /**/
    /*
    CarUIHandler::SetupCar(CarData carData, bool isCarPurchased) CarUIHandler::SetupCar(CarData carData, bool isCarPurchased)

    NAME
        CarUIHandler::SetupCar - Configures the UI elements for a specific car.

    SYNOPSIS
        public void SetupCar(CarData carData, bool isCarPurchased);
            carData          --> The data object containing details about the car.
            isCarPurchased   --> Flag indicating whether the car is already purchased.

    DESCRIPTION
        This method is used to configure the UI elements, such as car image and price, based on the provided car data. 
        It updates the UI to reflect whether the car is already purchased or still available for purchase.

    RETURNS
        Nothing.
    */
    /**/
    public void SetupCar(CarData carData, bool isCarPurchased)
    {
        carImage.sprite = carData.CarUISprite;

        if (!isCarPurchased)
        {
            carPriceText.text = carData.Cost.ToString() + " Points";
            carPriceText.gameObject.SetActive(true);
        }
        else
        {
            carPriceText.gameObject.SetActive(false); // Hide price text for purchased cars
        }
    }

    /**/
    /*
    CarUIHandler::StartCarEnterAnim(bool isEnterOnRight) CarUIHandler::StartCarEnterAnim(bool isEnterOnRight)

    NAME
        CarUIHandler::StartCarEnterAnim - Starts the car entry animation.

    SYNOPSIS
        public void StartCarEnterAnim(bool isEnterOnRight);
            isEnterOnRight   --> Flag indicating the direction of the entry animation.

    DESCRIPTION
        This method triggers the car's entry animation into the UI, based on the specified direction. 

    RETURNS
        Nothing.
    */
    /**/
    public void StartCarEnterAnim(bool isEnterOnRight)
    {
        if (isEnterOnRight)
        {
            animator.Play("Car UI Enter L");
        }
        else
        {
            animator.Play("Car UI Enter R");
        }
    }

    /**/
    /*
    CarUIHandler::StartCarExitAnim(bool isExitOnRight) CarUIHandler::StartCarExitAnim(bool isExitOnRight)

    NAME
        CarUIHandler::StartCarExitAnim - Starts the car exit animation.

    SYNOPSIS
        public void StartCarExitAnim(bool isExitOnRight);
            isExitOnRight    --> Flag indicating the direction of the exit animation.

    DESCRIPTION
        This method triggers the car's exit animation from the UI, based on the specified direction. 

    RETURNS
        Nothing.
    */
    /**/
    public void StartCarExitAnim(bool isExitOnRight)
    {
        if (isExitOnRight)
        {
            animator.Play("Car UI Exit L");
        }
        else
        {
            animator.Play("Car UI Exit R");
        }
    }

    /**/
    /*
    CarUIHandler::OnCarExitAnimComplete() CarUIHandler::OnCarExitAnimComplete()

    NAME
        CarUIHandler::OnCarExitAnimComplete - Handles the completion of the car exit animation.

    SYNOPSIS
        public void OnCarExitAnimComplete();

    DESCRIPTION
        This method is called when the car's exit animation is complete. 

    RETURNS
        Nothing.
    */
    /**/
    public void OnCarExitAnimComplete()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**/
/*
    This class manages the car selection interface in the game. This class handles UI interactions for car selection, 
    including spawning car sprites, updating UI elements based on car purchase status, and navigating between different cars.
    It also manages the buying of cars using points and transitions the player to the level selection menu after car selection.
*/
/**/
public class SelectCarMenu : MonoBehaviour
{
    [Header("Car Prefab")]
    public GameObject carPrefab;

    [Header("Spawn On")]
    public Transform spawnOnTransform;

    bool isChangingCar = false;

    int selectedCarIndex = 0;

    public TMP_Text totalPointsText;

    public Button selectButton;
    public Button buyButton;

    CarData[] allCarData;

    CarUIHandler carUIHandler = null;

    /**/
    /*
    SelectCarMenu::Start() SelectCarMenu::Start()

    NAME
        SelectCarMenu::Start - Initializes the car selection menu.

    SYNOPSIS
        void SelectCarMenu::Start();

    DESCRIPTION
        This method loads all car data, marks free cars as purchased, updates the display of total points, 
        adds a listener to the buy button, and starts the coroutine to spawn the first car.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        allCarData = Resources.LoadAll<CarData>("CarData/");

        foreach (var carData in allCarData)
        {
            if (carData.Cost == 0 && !GameManager.instance.IsCarPurchased(carData.CarUniqueID))
            {
                GameManager.instance.MarkCarAsPurchased(carData.CarUniqueID);
            }
        }

        UpdateTotalPointsDisplay();

        buyButton.onClick.AddListener(OnBuyButtonClicked);

        StartCoroutine(SpawnCarCO(true));
    }

    /**/
    /*
    SelectCarMenu::Update() SelectCarMenu::Update()

    NAME
        SelectCarMenu::Update - Handles real-time input during the car selection process.

    SYNOPSIS
        void SelectCarMenu::Update();

    DESCRIPTION
        This method listens for user input to navigate through and select cars.

    RETURNS
        Nothing.
    */
    /**/
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            OnPreviousCar();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            OnNextCar();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            OnSelectCar();
        }
    }

    private Dictionary<string, string> carNameMappings = new Dictionary<string, string>()
    {
        { "Car", "Red" },
        { "CarBlue Variant", "Blue" },
        { "CarGreen Variant", "Green" },
        { "CarYellow Variant", "Yellow" },
        { "CarOrange Variant", "Orange" },
        { "CarPurple Variant", "Purple" },
        { "CarGray Variant", "Gray" },
        { "CarBlack Variant", "Black" }
    };

    /**/
    /*
    SelectCarMenu::UpdateCarUI() SelectCarMenu::UpdateCarUI()

    NAME
        SelectCarMenu::UpdateCarUI - Updates the UI based on the car's purchase status.

    SYNOPSIS
        void SelectCarMenu::UpdateCarUI();

    DESCRIPTION
        This method updates the car selection UI elements based on whether the currently selected car
        has been purchased. 

    RETURNS
        Nothing.
    */
    /**/
    private void UpdateCarUI()
    {
        CarData selectedCarData = allCarData[selectedCarIndex];
        bool isCarPurchased = GameManager.instance.IsCarPurchased(selectedCarData.CarUniqueID);

        // Show the buy button only if the car has not been purchased
        buyButton.gameObject.SetActive(!isCarPurchased);

        // Show the select button only if the car has been purchased
        selectButton.gameObject.SetActive(isCarPurchased);
    }

    /**/
    /*
	SelectCarMenu::OnPreviousCar() SelectCarMenu::OnPreviousCar()
	
	NAME
		SelectCarMenu::OnPreviousCar - Handles the action of selecting the previous car in the car selection menu.

	SYNOPSIS
		void SelectCarMenu::OnPreviousCar();
	
	DESCRIPTION
		This method is triggered when the player wants to view the previous car in the car selection menu.
		It decrements the selectedCarIndex, ensuring it wraps around if it goes below zero. The method
		then calls SpawnCarCO coroutine to handle the car changing animation and updates the car UI accordingly.

	RETURNS
		Nothing.
	*/
    /**/
    public void OnPreviousCar()
    {
        if (isChangingCar)
        {
            return;
        }

        selectedCarIndex--;

        if (selectedCarIndex < 0)
        {
            selectedCarIndex = allCarData.Length - 1;
        }

        StartCoroutine(SpawnCarCO(true));

        UpdateCarUI();
    }

    /**/
    /*
	SelectCarMenu::OnNextCar() SelectCarMenu::OnNextCar()
	
	NAME
		SelectCarMenu::OnNextCar - Navigates to the next car in the selection menu.

	SYNOPSIS
		void SelectCarMenu::OnNextCar();
	
	DESCRIPTION
		This method is called to navigate to the next car in the car selection menu. It increments
		the selectedCarIndex and loops back to zero if it exceeds the total number of cars.
		It also invokes a coroutine for the car changing animation and updates the UI accordingly.
	
	RETURNS
		Nothing.
	*/
    /**/
    public void OnNextCar()
    {
        if (isChangingCar)
        {
            return;
        }

        selectedCarIndex++;

        if (selectedCarIndex > allCarData.Length - 1)
        {
            selectedCarIndex = 0;
        }

        StartCoroutine(SpawnCarCO(false));

        UpdateCarUI();
    }

    /**/
    /*
	SelectCarMenu::OnSelectCar() SelectCarMenu::OnSelectCar()
	
	NAME
		SelectCarMenu::OnSelectCar - Finalizes the car selection process and loads the level selection menu.

	SYNOPSIS
		void SelectCarMenu::OnSelectCar();

	DESCRIPTION
		This method finalizes the car selection by the player and prepares the game for the next stage.
		It clears the existing player list in the GameManager, adds the selected player and AI players with
		their respective cars, and transitions to the level selection scene. 
	
	RETURNS
		Nothing.
	*/
    /**/
    public void OnSelectCar()
    {
        GameManager.instance.ClearPlayerList();

        GameManager.instance.AddPlayerToList(1, "Player1", allCarData[selectedCarIndex].CarUniqueID, false);

        List<CarData> uniqueCars = new List<CarData>(allCarData);

        // Remove the car that player has selected
        uniqueCars.Remove(allCarData[selectedCarIndex]);

        string[] names = { "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Gray", "Black" };
        List<string> uniqueNames = names.ToList<string>();

        foreach (CarData aiCarData in uniqueCars)
        {
            string carPrefabName = aiCarData.CarPrefab.name;

            // Use the carNameMappings dictionary to get a specific name for each AI car
            if (carNameMappings.TryGetValue(carPrefabName, out string aiDriverName))
            {
                GameManager.instance.AddPlayerToList(aiCarData.CarUniqueID, aiDriverName, aiCarData.CarUniqueID, true);
            }
            else
            {
                // Default name if no mapping is found
                GameManager.instance.AddPlayerToList(aiCarData.CarUniqueID, "DefaultAIName", aiCarData.CarUniqueID, true);
            }
        }

        SceneManager.LoadScene("Level-Menu");
    }

    /**/
    /*
	SelectCarMenu::UpdateTotalPointsDisplay() SelectCarMenu::UpdateTotalPointsDisplay()
	
	NAME
		SelectCarMenu::UpdateTotalPointsDisplay - Updates the display of total points in the UI.
	
	SYNOPSIS
		void SelectCarMenu::UpdateTotalPointsDisplay();
	
	DESCRIPTION
		This method updates the total points text display in the car selection menu to show the current point balance.
	
	RETURNS
		Nothing.
	*/
    /**/
    private void UpdateTotalPointsDisplay()
    {
        if (totalPointsText != null)
        {
            totalPointsText.text = "Total Points: " + GameManager.instance.totalPoints.ToString();
        }
    }

    /**/
    /*
    SelectCarMenu::OnBuyButtonClicked() SelectCarMenu::OnBuyButtonClicked()

    NAME
        SelectCarMenu::OnBuyButtonClicked - Handles the 'Buy Car' button click event.

    SYNOPSIS
        void SelectCarMenu::OnBuyButtonClicked();

    DESCRIPTION
        This method is invoked when the 'Buy Car' button is clicked. It checks if the player has enough points to
        purchase the selected car. If sufficient points are available, the car's cost is deducted from the player's
        total points, and the car is marked as purchased.

    RETURNS
        Nothing.
    */
    /**/
    private void OnBuyButtonClicked()
    {
        CarData selectedCarData = allCarData[selectedCarIndex];
        int carCost = selectedCarData.Cost;

        // Check if the player has enough points
        if (GameManager.instance.totalPoints >= carCost)
        {
            GameManager.instance.totalPoints -= carCost;

            GameManager.instance.MarkCarAsPurchased(selectedCarData.CarUniqueID);

            // Update the UI to reflect the new points total and purchase status
            UpdateTotalPointsDisplay();
            UpdateCarPurchaseUI();
        }
        else
        {
            Debug.Log("Not enough points to buy this car");
        }
        UpdateCarUI();
    }

    /**/
    /*
    SelectCarMenu::UpdateCarPurchaseUI() SelectCarMenu::UpdateCarPurchaseUI()

    NAME
        SelectCarMenu::UpdateCarPurchaseUI - Updates the purchase UI for the selected car.

    SYNOPSIS
        void SelectCarMenu::UpdateCarPurchaseUI();

    DESCRIPTION
        This method updates the UI elements related to the car purchase. 

    RETURNS
        Nothing.
    */
    /**/
    private void UpdateCarPurchaseUI()
    {
        CarData selectedCarData = allCarData[selectedCarIndex];
        bool canAffordCar = GameManager.instance.totalPoints >= selectedCarData.Cost;
        bool isCarPurchased = GameManager.instance.IsCarPurchased(selectedCarData.CarUniqueID);

        buyButton.gameObject.SetActive(canAffordCar && !isCarPurchased);
    }
	
	/**/
	/*
    SelectCarMenu::CanAffordSelectedCar() SelectCarMenu::CanAffordSelectedCar()

    NAME
        SelectCarMenu::CanAffordSelectedCar - Checks if the player can afford the selected car.

    SYNOPSIS
        bool SelectCarMenu::CanAffordSelectedCar();

    DESCRIPTION
        This method returns a boolean value indicating whether the player has enough points to purchase the currently
        selected car based on its cost.

    RETURNS
        True if the player can afford the car, False otherwise.
    */
	/**/
    private bool CanAffordSelectedCar()
    {
        int carCost = allCarData[selectedCarIndex].Cost;
        return GameManager.instance.totalPoints >= carCost;
    }

    /**/
    /*
    SelectCarMenu::CheckIfCarIsPurchased(int carID) SelectCarMenu::CheckIfCarIsPurchased(int carID)

    NAME
        SelectCarMenu::CheckIfCarIsPurchased - Verifies if a car is already purchased.

    SYNOPSIS
        bool SelectCarMenu::CheckIfCarIsPurchased(int carID);
            carID   --> The unique ID of the car to check.

    DESCRIPTION
        This method checks if a car with the specified unique ID has already been purchased by the player.

    RETURNS
        True if the car is purchased, False otherwise.
    */
    /**/
    private bool CheckIfCarIsPurchased(int carID)
    {
        return GameManager.instance.IsCarPurchased(carID);
    }


    /**/
    /*
    SelectCarMenu::SpawnCarCO() SelectCarMenu::SpawnCarCO()

    NAME
        SelectCarMenu::SpawnCarCO - Coroutine for spawning and displaying cars in the car selection menu.

    SYNOPSIS
        IEnumerator SelectCarMenu::SpawnCarCO(bool isCarEnterRight);
            isCarEnterRight   --> Indicates if the car should enter from the right side.

    DESCRIPTION
        This coroutine manages the car spawning process in the car selection menu. It instantiates the car prefab,
        assigns a name based on the car data, and manages the car's entrance animation to allow for a smooth transition
        between car selections.

    RETURNS
        IEnumerator that temporarily halts execution to properly time the spawn animation.
    */
    /**/
    IEnumerator SpawnCarCO(bool isCarEnterRight)
    {
        isChangingCar = true;

        if (carUIHandler != null ) 
        {
            carUIHandler.StartCarExitAnim(!isCarEnterRight);
        }

        GameObject instantiatedCar = Instantiate(carPrefab, spawnOnTransform);

        string carPrefabName = allCarData[selectedCarIndex].CarPrefab.name;
        if (carNameMappings.TryGetValue(carPrefabName, out string fixedName))
        {
            instantiatedCar.name = fixedName;
        }
        else
        {
            instantiatedCar.name = "DefaultName";
        }

        carUIHandler = instantiatedCar.GetComponent<CarUIHandler>();
        bool isPurchased = CheckIfCarIsPurchased(allCarData[selectedCarIndex].CarUniqueID);
        carUIHandler.SetupCar(allCarData[selectedCarIndex], isPurchased);
        carUIHandler.StartCarEnterAnim(isCarEnterRight);

        UpdateCarUI();

        yield return new WaitForSeconds(0.8f);

        isChangingCar = false;
    }

    /**/
    /*
    SelectCarMenu::OnDestroy() SelectCarMenu::OnDestroy()

    NAME
        SelectCarMenu::OnDestroy - Handles the cleanup when the object is destroyed.

    SYNOPSIS
        void SelectCarMenu::OnDestroy();

    DESCRIPTION
        This method is called when the SelectCarMenu object is being destroyed.

    RETURNS
        Nothing.
    */
    /**/
    private void OnDestroy()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClicked);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**/
/*
    The PositionUIHandler class is responsible for managing the display of race positions for each 
    car in the game. It handles the creation and updating of UI elements that represent each car's 
    position in the race. This class uses either a vertical or horizontal layout to display the position information, 
    based on the configuration. 
*/
/**/
public class PositionUIHandler : MonoBehaviour
{
    public GameObject positionItemPrefab;

    PositionItemInfo[] positionItemInfo;

    Canvas canvas;

    bool isInitialized = false;

    public bool useVerticalLayout = true;

    /**/
    /*
    PositionUIHandler::Awake() PositionUIHandler::Awake()

    NAME
        PositionUIHandler::Awake - Initializes the Position UI Handler.

    SYNOPSIS
        void PositionUIHandler::Awake();

    DESCRIPTION
        This method initializes the canvas and determines the layout type (vertical or horizontal).

    RETURNS
        Nothing.
    */
    /**/
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = !useVerticalLayout;

        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }

    /**/
    /*
    PositionUIHandler::Start() PositionUIHandler::Start()

    NAME
        PositionUIHandler::Start - Prepares position item UI elements.

    SYNOPSIS
        void PositionUIHandler::Start();

    DESCRIPTION
        This method creates position item UI elements for each car in the race and initializes their display. 
        The method decides between a vertical or horizontal layout based on the 'useVerticalLayout' flag.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        Transform layoutGroupTransform;
        if (useVerticalLayout)
        {
            VerticalLayoutGroup positionVLG = GetComponentInChildren<VerticalLayoutGroup>();
            layoutGroupTransform = positionVLG.transform;
        }
        else
        {
            HorizontalLayoutGroup positionHLG = GetComponentInChildren<HorizontalLayoutGroup>();
            layoutGroupTransform = positionHLG.transform;
        }

        LapCounter[] lapCounterArray = FindObjectsOfType<LapCounter>();

        positionItemInfo = new PositionItemInfo[lapCounterArray.Length];

        for (int i = 0; i < lapCounterArray.Length; i++)
        {
            GameObject positionInfoGameObject = Instantiate(positionItemPrefab, layoutGroupTransform);

            positionItemInfo[i] = positionInfoGameObject.GetComponent<PositionItemInfo>();

            positionItemInfo[i].SetPositionText($"{i + 1}.");
        }

        Canvas.ForceUpdateCanvases();

        isInitialized = true;
    }

    /**/
    /*
    PositionUIHandler::UpdateList(List<LapCounter> lapCounters) PositionUIHandler::UpdateList(List<LapCounter> lapCounters)

    NAME
        PositionUIHandler::UpdateList - Updates the position UI list.

    SYNOPSIS
        public void PositionUIHandler::UpdateList(List<LapCounter> lapCounters);
            lapCounters    --> List of LapCounter objects representing cars in the race.

    DESCRIPTION
        This method updates the list of position UI elements to reflect the current positions of each car in the race. 

    RETURNS
        Nothing.
    */
    /**/
    public void UpdateList(List<LapCounter> lapCounters)
    {
        if (!isInitialized)
        {
            return;
        }

        for (int i = 0; i < lapCounters.Count; i++)
        {
            positionItemInfo[i].SetCarName(lapCounters[i].gameObject.name);
            Debug.Log($"Updating car {i}: {lapCounters[i].gameObject.name}");
        }
    }

    /**/
    /*
    PositionUIHandler::OnGameStateChanged(GameManager gameManager) PositionUIHandler::OnGameStateChanged(GameManager gameManager)

    NAME
        PositionUIHandler::OnGameStateChanged - Responds to changes in the game state.

    SYNOPSIS
        void PositionUIHandler::OnGameStateChanged(GameManager gameManager);

    DESCRIPTION
        This method is invoked when there is a change in the game state. It is responsible for 
        enabling or disabling the canvas based on the game state and the layout type.

    RETURNS
        Nothing.
    */
    /**/
    void OnGameStateChanged(GameManager gameManager)
    {
        if (GameManager.instance.GetGameState() == GameStates.raceOver)
        {
            if (!useVerticalLayout)
            {
                canvas.enabled = false;
            }
            else
            {
                canvas.enabled = true;
            }
        }
    }

    /**/
    /*
    PositionUIHandler::OnDestroy() PositionUIHandler::OnDestroy()

    NAME
        PositionUIHandler::OnDestroy - Cleans up before the object is destroyed.

    SYNOPSIS
        void PositionUIHandler::OnDestroy();

    DESCRIPTION
        This method is called when the script object is being destroyed.

    RETURNS
        Nothing.
    */
    /**/
    void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }
}

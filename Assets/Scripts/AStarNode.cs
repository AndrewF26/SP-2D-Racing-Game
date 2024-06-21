using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class represents a node used in the A* pathfinding algorithm in a grid-based environment. 
    Each node contains information about its position in the grid, its neighbors, and various costs 
    associated with the pathfinding process. The costs include the distance from the start, 
    the estimated distance to the goal, and the total cost. The class also provides 
    methods for calculating these costs based on the AI's current position and destination, 
    as well as a method to reset the node's properties for new pathfinding calculations. 
    Nodes can be marked as obstacles, affecting their traversal in the pathfinding algorithm. 
*/
/**/
public class AStarNode
{
    public Vector2Int gridPosition;

    public List<AStarNode> neighbours = new List<AStarNode>();

    public bool isObstacle = false;

    public int gCostDistanceFromStart = 0;

    public int hCostDistanceFromGoal = 0;

    public int fCostTotal = 0;

    public int pickedOrder = 0;

    bool isCostCalculated = false;

    /**/
    /*
    AStarNode::AStarNode(Vector2Int gridPosition_) AStarNode::AStarNode(Vector2Int gridPosition_)

    NAME
        AStarNode - Constructor for the AStarNode class.

    SYNOPSIS
        AStarNode(Vector2Int gridPosition_);
            gridPosition_    --> The position of the node in the grid.

    DESCRIPTION
        This constructor initializes an AStarNode instance. It sets the grid position of the node,
        which is used in the A* pathfinding algorithm to track the node's location on the grid.

    RETURNS
        An instance of AStarNode.
    */
    /**/
    public AStarNode(Vector2Int gridPosition_)
    {
        gridPosition = gridPosition_;
    }
	
	/**/
	/*
    AStarNode::CalculateCostsForNode() AStarNode::CalculateCostsForNode()

    NAME
        AStarNode::CalculateCostsForNode - calculates costs for this node.

    SYNOPSIS
        void CalculateCostsForNode(Vector2Int aiPosition, Vector2Int aiDestination);
            aiPosition       --> The current position of the AI.
            aiDestination    --> The destination position of the AI.

    DESCRIPTION
        This method calculates the costs (gCost, hCost, and fCost) for the node.
        If costs are already calculated, the method returns early.

    RETURNS
        Nothing.
    */
	/**/
    public void CalculateCostsForNode(Vector2Int aiPosition, Vector2Int aiDestination)
    {
        if (isCostCalculated)
            return;

        // Calculate the distance from the AI's current position to this node.
        gCostDistanceFromStart = Mathf.Abs(gridPosition.x - aiPosition.x) + Mathf.Abs(gridPosition.y - aiPosition.y);

        // Calculate the estimated distance from this node to the AI's destination.
        hCostDistanceFromGoal = Mathf.Abs(gridPosition.x - aiDestination.x) + Mathf.Abs(gridPosition.y - aiDestination.y);

        fCostTotal = gCostDistanceFromStart + hCostDistanceFromGoal;

        isCostCalculated = true;
    }
	
	/**/
	/*
    AStarNode::Reset() AStarNode::Reset()

    NAME
        AStarNode::Reset - resets the node's cost and order properties.

    SYNOPSIS
        void Reset();

    DESCRIPTION
        This method resets the node's properties, including costs and picked order.
        It is used to prepare the node for a new pathfinding calculation.

    RETURNS
        Nothing.
    */
	/**/
    public void Reset()
    {
        isCostCalculated = false;
        pickedOrder = 0;
        gCostDistanceFromStart = 0;
        hCostDistanceFromGoal = 0;
        fCostTotal = 0;
    }
}

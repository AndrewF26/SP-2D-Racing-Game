using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;

/**/
/*
    This class is responsible for implementing the A* pathfinding algorithm in a grid-based environment. 
    It manages the creation and initialization of the grid of AStarNodes, calculates paths for AI navigation, 
    and provides methods for converting between world and grid coordinates. Key features include grid 
    initialization, obstacle detection, and pathfinding.
*/
/**/
public class AStarPath : MonoBehaviour
{
    int gridSizeX = 60;
    int gridSizeY = 30;

    float cellSize = 1;

    AStarNode[,] aStarNodes;

    AStarNode startNode;

    List<AStarNode> nodesToCheck = new List<AStarNode>();
    List<AStarNode> nodesChecked = new List<AStarNode>();

    List<Vector2> aiPath = new List<Vector2>();

    /**/
    /*
    AStarPath::Start() AStarPath::Start()

    NAME
        AStarPath::Start - Initialization method called before the first frame update.

    SYNOPSIS
        void Start();

    DESCRIPTION
        This method is responsible for initializing the A* pathfinding algorithm. It calls the CreateGrid method 
        to initialize and populate the grid with AStarNodes, and subsequently calls the FindPath method with 
        a predefined destination to demonstrate pathfinding.

    RETURNS
        Nothing.
    */
    /**/
    void Start()
    {
        CreateGrid();

        FindPath(new Vector2(32, 17));
    }

    /**/
    /*
    AStarPath::CreateGrid() AStarPath::CreateGrid()

    NAME
        AStarPath::CreateGrid - initializes and populates the grid with AStarNodes.

    SYNOPSIS
        void CreateGrid();

    DESCRIPTION
        This method creates a grid of AStarNode instances, each representing a cell in the pathfinding grid.
        It initializes each node, assigns its position, and determines if it's an obstacle based on colliders in the scene.

    RETURNS
        Nothing.
    */
    /**/
    void CreateGrid()
    {
        aStarNodes = new AStarNode[gridSizeX, gridSizeY];

        // Create the grid of nodes and check for obstacles
        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                aStarNodes[x, y] = new AStarNode(new Vector2Int(x, y));

                Vector3 worldPosition = ConvertGridToWorldPosition(aStarNodes[x, y]);

                Collider2D hitCollider2D = Physics2D.OverlapCircle(worldPosition, cellSize / 2.0f);

                // Mark objects as obstacles unless thay have certain tags
                if (hitCollider2D != null)
                {
                    if (hitCollider2D.CompareTag("Checkpoint") ||
                        hitCollider2D.transform.root.CompareTag("Jump") ||
                        hitCollider2D.transform.root.CompareTag("ItemBox") ||
                        hitCollider2D.transform.root.CompareTag("AI") ||
                        hitCollider2D.transform.root.CompareTag("Player"))
                    {
                        continue; 
                    }

                    aStarNodes[x, y].isObstacle = true;
                }

            }

        // Loop through the grid and populate neighboring cells
        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                // Check neighbouring cell to north
                if (y - 1 >= 0)
                {
                    if (!aStarNodes[x, y - 1].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x, y - 1]);
                }

                // Check neighbouring cell to south
                if (y + 1 <= gridSizeY - 1)
                {
                    if (!aStarNodes[x, y + 1].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x, y + 1]);
                }

                // Check neighbouring cell to east
                if (x - 1 >= 0)
                {
                    if (!aStarNodes[x - 1, y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x - 1, y]);
                }

                // Check neighbouring cell to west
                if (x + 1 <= gridSizeX - 1)
                {
                    if (!aStarNodes[x + 1, y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x + 1, y]);
                }
            }
    }

    /**/
    /*
    AStarPath::FindPath(Vector2 destination) AStarPath::FindPath(Vector2 destination)

    NAME
        AStarPath::FindPath - calculates a path from the current position to the specified destination.

    SYNOPSIS
        List<Vector2> FindPath(Vector2 destination);
                destination     --> The target position for pathfinding.

    DESCRIPTION
        This method initiates the pathfinding process from the object's current position to the specified destination.
        It uses the A* algorithm to find the shortest path, considering obstacles and grid boundaries.
        The method returns null if no path is found.

    RETURNS
        List<Vector2> aiPath: A list of Vector2 points representing the calculated path.
    */
    /**/
    public List<Vector2> FindPath(Vector2 destination)
    {
        if (aStarNodes == null)
            return null;

        Reset();

        // Convert the destination from world to grid position
        Vector2Int destinationGridPoint = ConvertWorldToGridPoint(destination);
        Vector2Int currentPositionGridPoint = ConvertWorldToGridPoint(transform.position);

        // Calculate the costs for the first node by starting the algorithm
        startNode = GetNodeFromPoint(currentPositionGridPoint);

        AStarNode currentNode = startNode;

        bool isDoneFindingPath = false;
        int pickedOrder = 1;

        // Loop until a path is found
        while (!isDoneFindingPath)
        {
            nodesToCheck.Remove(currentNode);

            currentNode.pickedOrder = pickedOrder;

            pickedOrder++;

            nodesChecked.Add(currentNode);

            if (currentNode.gridPosition == destinationGridPoint)
            {
                isDoneFindingPath = true;
                break;
            }

            CalculateCostsForNodeAndNeighbours(currentNode, currentPositionGridPoint, destinationGridPoint);

            foreach (AStarNode neighbourNode in currentNode.neighbours)
            {
                if (nodesChecked.Contains(neighbourNode))
                    continue;

                if (nodesToCheck.Contains(neighbourNode))
                    continue;

                nodesToCheck.Add(neighbourNode);
            }

            nodesToCheck = nodesToCheck.OrderBy(x => x.fCostTotal).ThenBy(x => x.hCostDistanceFromGoal).ToList();

            if (nodesToCheck.Count == 0)
            {
                Debug.LogWarning($"No solutions left to check for {transform.name}");
                return null;
            }
            else
            {
                currentNode = nodesToCheck[0];
            }
        }

        aiPath = CreatePathForAI(currentPositionGridPoint);

        return aiPath;
    }
	
	/**/
	/*
    AStarPath::ConvertWorldToGridPoint(Vector2 position) AStarPath::ConvertWorldToGridPoint(Vector2 position)

    NAME
        AStarPath::ConvertWorldToGridPoint - converts a world position to a grid coordinate.

    SYNOPSIS
        Vector2Int ConvertWorldToGridPoint(Vector2 position);
            position        --> The world position to be converted.

    DESCRIPTION
        This method converts a position in the world space (e.g., Unity's Vector2 coordinates) to a corresponding
        point in the grid coordinate system used for pathfinding. It is essential for translating game objects' positions
        into the grid-based context of the A* algorithm.

    RETURNS
        Vector2Int gridPoint: The grid coordinate corresponding to the given world position.
    */
	/**/
    Vector2Int ConvertWorldToGridPoint(Vector2 position)
    {
        Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(position.x / cellSize + gridSizeX / 2.0f), Mathf.RoundToInt(position.y / cellSize + gridSizeY / 2.0f));

        return gridPoint;
    }

	/**/
	/*
    AStarPath::ConvertGridToWorldPosition(AStarNode aStarNode) AStarPath::ConvertGridToWorldPosition(AStarNode aStarNode)

    NAME
        AStarPath::ConvertGridToWorldPosition - converts a grid coordinate to a world position.

    SYNOPSIS
        Vector3 ConvertGridToWorldPosition(AStarNode aStarNode);
            aStarNode       --> The AStarNode whose grid position is to be converted.

    DESCRIPTION
        This method converts a grid coordinate from an AStarNode back to a world space position.
        It is useful for mapping the pathfinding results to actual positions in the game world.

    RETURNS
        The world space position corresponding to the AStarNode's grid position.
    */
	/**/
    Vector3 ConvertGridToWorldPosition(AStarNode aStarNode)
    {
        return new Vector3(aStarNode.gridPosition.x * cellSize - (gridSizeX * cellSize) / 2.0f, aStarNode.gridPosition.y * cellSize - (gridSizeY * cellSize) / 2.0f, 0);
    }
	
	/**/
	/*
    AStarPath::GetNodeFromPoint(Vector2Int gridPoint) AStarPath::GetNodeFromPoint(Vector2Int gridPoint)

    NAME
        AStarPath::GetNodeFromPoint - retrieves the AStarNode at a specific grid coordinate.

    SYNOPSIS
        AStarNode GetNodeFromPoint(Vector2Int gridPoint);
            gridPoint       --> The grid coordinate for which to retrieve the node.

    DESCRIPTION
        This method returns the AStarNode located at a specified grid coordinate. It is used to access
        nodes in the grid based on their positions. The method handles edge cases where the gridPoint
        may be out of the grid's bounds.

    RETURNS
        The node at the specified grid coordinate, or null if out of bounds.
    */
	/**/
    AStarNode GetNodeFromPoint(Vector2Int gridPoint)
    {
        if (gridPoint.x < 0)
            return null;

        if (gridPoint.x > gridSizeX - 1)
            return null;

        if (gridPoint.y < 0)
            return null;

        if (gridPoint.y > gridSizeY - 1)
            return null;

        return aStarNodes[gridPoint.x, gridPoint.y];
    }

    /**/
    /*
    AStarPath::CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector2Int aiPosition, Vector2Int aiDestination)

    NAME
        AStarPath::CalculateCostsForNodeAndNeighbours - calculates costs for a node and its neighbors.

    SYNOPSIS
        void CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector2Int aiPosition, Vector2Int aiDestination);
            aStarNode       --> The node for which to calculate costs.
            aiPosition      --> The AI's current position in grid coordinates.
            aiDestination   --> The AI's destination in grid coordinates.

    DESCRIPTION
        This method calculates the pathfinding costs for a given node and its accessible neighbors.
        It updates the nodes' cost properties in order for the A* algorithm to determine the most efficient path.

    RETURNS
        Nothing.
    */
    /**/
    void CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector2Int aiPosition, Vector2Int aiDestination)
    {
        aStarNode.CalculateCostsForNode(aiPosition, aiDestination);

        foreach (AStarNode neighbourNode in aStarNode.neighbours)
        {
            neighbourNode.CalculateCostsForNode(aiPosition, aiDestination);
        }
    }
	
	/**/
	/*
    AStarPath::CreatePathForAI(Vector2Int currentPositionGridPoint) AStarPath::CreatePathForAI(Vector2Int currentPositionGridPoint)

    NAME
        AStarPath::CreatePathForAI - creates the final path for AI navigation.

    SYNOPSIS
        List<Vector2> CreatePathForAI(Vector2Int currentPositionGridPoint);
            currentPositionGridPoint --> The current position of the AI in grid coordinates.

    DESCRIPTION
        After the pathfinding process is complete, this method is used to backtrack from the destination to the start,
        creating a list of waypoints that represent the path for the AI to follow. The method ensures that the path is
        created efficiently and handles potential issues in path creation.

    RETURNS
        List<Vector2> resultAIPath: A list of waypoints representing the AI's path.
    */
	/**/
    List<Vector2> CreatePathForAI(Vector2Int currentPositionGridPoint)
    {
        List<Vector2> resultAIPath = new List<Vector2>();
        List<AStarNode> aiPath = new List<AStarNode>();

        nodesChecked.Reverse();

        bool isPathCreated = false;

        AStarNode currentNode = nodesChecked[0];

        aiPath.Add(currentNode);

        int attempts = 0;

        // Loop until a path is created
        while (!isPathCreated)
        {
            currentNode.neighbours = currentNode.neighbours.OrderBy(x => x.pickedOrder).ToList();

            foreach (AStarNode aStarNode in currentNode.neighbours)
            {
                if (!aiPath.Contains(aStarNode) && nodesChecked.Contains(aStarNode))
                {
                    aiPath.Add(aStarNode);
                    currentNode = aStarNode;

                    break;
                }
            }

            if (currentNode == startNode)
                isPathCreated = true;

            if (attempts > 1000)
            {
                Debug.LogWarning("Unable to create a path for AI after too many attempts");
                break;
            }

            attempts++;
        }

        foreach (AStarNode aStarNode in aiPath)
        {
            resultAIPath.Add(ConvertGridToWorldPosition(aStarNode));
        }

        resultAIPath.Reverse();

        return resultAIPath;
    }

    /**/
    /*
    AStarPath::Reset() AStarPath::Reset()

    NAME
        AStarPath::Reset - resets the pathfinding data for a new calculation.

    SYNOPSIS
        void Reset();

    DESCRIPTION
        This method clears the lists of nodes to check and checked nodes, and resets each node in the grid
        in order to prepare for a new path calculation.

    RETURNS
        Nothing.
    */
    /**/
    private void Reset()
    {
        nodesToCheck.Clear();
        nodesChecked.Clear();
        aiPath.Clear();

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
                aStarNodes[x, y].Reset();
    }
}

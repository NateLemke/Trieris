using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: This class stores information for a corordiante node on a board. Nodes may contain multiple ships and up to one Port.
///             Ports have an X Y coordiante on the game board, which is NOT the same as their realworld unity scene position
/// </summary>
public class Node{

    // determines the spacing between ships when there are multiple ships on a node
    public const float shipSpacingX = 0.34f;
    public const float shipSpacingY = 0.3f;

    // used for debugging visuals
    public const float GIZMO_SIZE = 0.1f;
    public static Color GIZMO_COLOR = Color.black;

    // game board coordinates
    public int X { get; }
    public int Y { get; }

    //private Color color;

    // adjacent nodes in clockwise direction
    public Node[] Adjacents { get { return adjacents; } }
    private Node[] adjacents = new Node[8];

    public Port Port { get; set; }
    public List<Ship> Ships { get { return ships; } }
    private List<Ship> ships = new List<Ship>();    
    private GameObject thisNode;

    // if true, this node is a "land node"
    public bool island { get; set; }

    /// <summary>
    /// Constructor. Sets the game board X Y coordinates for this node.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Node(int x,int y) {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Sets the reference to this node's sprite renderer gameobject
    /// </summary>
    /// <param name="input"></param>
    public void setGameObject(GameObject input)
    {
        thisNode = input;
    }

    /// <summary>
    /// Gets the reference to this node's sprite renderer gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject getGameObject()
    {
        return thisNode;
    }    


    /// <summary>
    /// Returns the node's game board coordinate
    /// </summary>
    /// <returns></returns>
    public Vector2 getBoardPosition() {
        return new Vector2(X,Y);
    }

    /// <summary>
    /// Gets this node's world position in the scene's X Y space
    /// </summary>
    /// <returns></returns>
    public Vector2 getRealPos() {
        return new Vector2(Y,Board.ROW_OF_NODES - 1 - X);
    }

    /// <summary>
    /// Returns the adjacent node from this node in the given direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Node getAdjacentNode(int direction) {
        return adjacents[direction];
    }
 
    public void setAdjacentNode(int direction,Node node) {
        adjacents[direction] = node;
    }

    public int getNumberOfShips() {
        return ships.Count;
    }

    /// <summary>
    /// calcuates the render position for a ship on a node
    /// this is needed as there could be possibly multiple ships on the same node
    /// this calcuation returns positions so the ships are even spaced around the node without overlapping
    /// </summary>
    /// <param name="s">the ship whose position we're looking for</param>
    /// <param name="n">the node the ship is currently on</param>
    /// <param name="xSpace">the spacing between ships on the x-axis</param>
    /// <param name="ySpace">the spacing between shpis on the y-axis</param>
    /// <returns>Vector2 position that the ship should be set to</returns>
    public Vector2 shipNodePos(Ship s,float xSpace = shipSpacingX,float ySpace = shipSpacingY) {
        //Node n = s.getNode();
        //List<Ship> ships = n.getShips();

        if (ships.Count == 0) {
            Debug.LogError("No ships in node!");
        }

        if (ships.Count == 1) {
            return getRealPos();
        }

        float sqr = Mathf.Sqrt(ships.Count);
        float rounded = Mathf.Ceil(sqr);


        int i = 0;
        for (; i < ships.Count; i++) {
            if (ships[i] == s) {
                break;
            }
        }

        int x = (i) % (int)rounded;
        int y = i / (int)rounded;

        float offset = (rounded - 1) * xSpace / 2f;
        Vector2 pos = new Vector2(x * xSpace - offset,-y * ySpace);

        return pos + getRealPos();
    }
}

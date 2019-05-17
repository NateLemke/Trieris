using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: This class stores information for a corordiante node on a board. Nodes may contain multiple ships and up to one Port.
///             Ports have an X Y coordiante on the game board, which is NOT the same as their realworld unity scene position
/// </summary>
public class Node{

    // used for debugging visuals
    public const float GIZMO_SIZE = 0.1f;
    public static Color GIZMO_COLOR = Color.black;

    // game board coordinates
    private int x;
    private int y;

    //private Color color;

    // adjacent nodes in clockwise direction
    private Node[] adjacents = new Node[8];
    private Port port;
    private List<Ship> ships = new List<Ship>();
    private bool island;
    private GameObject thisNode;

    //public Node() {
    //    setColor(Color.black);
    //}

    /// <summary>
    /// Constructor. Sets the game board X Y coordinates for this node.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Node(int x,int y) {
        this.x = x;
        this.y = y;
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

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    /// <summary>
    /// Returns the node's game board coordinate
    /// </summary>
    /// <returns></returns>
    public Vector2 getPosition() {
        return new Vector2(x,y);
    }

    /// <summary>
    /// Gets this node's world position in the scene's X Y space
    /// </summary>
    /// <returns></returns>
    public Vector2 getRealPos() {
        return new Vector2(y,Board.ROW_OF_NODES - 1 - x);
    }

    //public Color getColor() {
    //    return color;
    //}

    public Node getAdjacentNode(int direction) {
        return adjacents[direction];
    }

    public Node[] getAdjacentNodes() {
        return adjacents;
    }

    public Port getPort() {
        return port;
    }

    public List<Ship> getShips() {
        return ships;
    }

    public bool isIsland() {
        return island;
    }

    public void setPosition(int x,int y) {
        this.x = x;
        this.y = y;
    }

    //public void setColor(Color color) {
    //    this.color = color;
    //}

    public void setAdjacentNode(int direction,Node node) {
        adjacents[direction] = node;
    }

    public void setPort(Port port) {
        this.port = port;
    }

    public void setShips(List<Ship> ships) {
        this.ships = ships;
    }

    public void setIsland(bool isIsland) {
        //if (isIsland) {
        //    setColor(Color.magenta);
        //}
        island = isIsland;
    }

    public int getNumberOfShips() {
        return ships.Count;
    }


    //public void activateNotification()
    //{
    //    thisNode.transform.Find("TargetNeededNotification").gameObject.SetActive(true);
    //}

    //public void expand()
    //{

    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{

    // new properties
    public const float GIZMO_SIZE = 0.1f;
    public static Color GIZMO_COLOR = Color.black;
    //public Vector3 realPos;

    private int x;
    private int y;

    private Color color;

    private Node[] adjacents = new Node[8];
    private Port port;
    private List<Ship> ships = new List<Ship>();
    private bool island;
    private GameObject thisNode;
    

    public Node() {
        setColor(Color.black);
    }

    public void setGameObject(GameObject input)
    {
        thisNode = input;
    }

    public GameObject getGameObject()
    {
        return thisNode;
    }

    public Node(int x,int y) {
        this.x = x;
        this.y = y;
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    // new function
    public Vector2 getPosition() {
        return new Vector2(x,y);
    }

    // new function
    public Vector2 getRealPos() {
        return new Vector2(y,Board.ROW_OF_NODES - 1 - x);
    }

    public Color getColor() {
        return color;
    }

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

    public void setColor(Color color) {
        this.color = color;
    }

    public void setAdjacentNode(int direction,Node node) {
        adjacents[direction] = node;
    }

    public void setPort(Port port) {
        this.port = port;
        setColor(port.getColor());
    }

    public void setShips(List<Ship> ships) {
        this.ships = ships;
    }

    public void setIsland(bool isIsland) {
        if (isIsland) {
            setColor(Color.magenta);
        }
        island = isIsland;
    }

    public int getNumberOfShips() {
        return ships.Count;
    }

    public void activateNotification()
    {
        thisNode.transform.Find("TargetNeededNotification").gameObject.SetActive(true);
    }

    public void expand()
    {

    }
}

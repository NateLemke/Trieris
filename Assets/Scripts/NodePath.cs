using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: This class is used to generate node paths for the AI
/// </summary>
public class NodePath : IComparable<NodePath> {

    private Node currentNode;
    private List<int> actionsList;
    private int shipDirection;

    /// <summary>
    /// Consturctor, requires references to the current node, the list of actions, and the ship direction
    /// </summary>
    /// <param name="currentNode">the current node in the path</param>
    /// <param name="actionsList">the action list of the AI ship finding the path</param>
    /// <param name="shipDirection">the current direction of the ship</param>
    public NodePath(Node currentNode,List<int> actionsList,int shipDirection) {
        this.currentNode = currentNode;
        this.actionsList = actionsList;
        this.shipDirection = shipDirection;
    }

    public Node getNode() {
        return currentNode;
    }

    public List<int> getActionsList() {
        return actionsList;
    }

    public int getShipDirection() {
        return shipDirection;
    }

    public void setNode(Node newNode) {
        currentNode = newNode;
    }

    public void setActionsList(List<int> newActionsList) {
        actionsList = newActionsList;
    }

    public void setShipDirection(int newShipDirection) {
        shipDirection = newShipDirection;
    }

    /// <summary>
    /// Compares one node path to another. Node paths with longer action lists are asummed to be greater.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(NodePath other) {
        if (this.getActionsList().Count < other.getActionsList().Count) {
            return -1;
        }
        if (this.getActionsList().Count > other.getActionsList().Count) {
            return 1;
        }
        return 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePath : IComparable<NodePath> {

    private Node currentNode;
    private List<int> actionsList;
    private int shipDirection;

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

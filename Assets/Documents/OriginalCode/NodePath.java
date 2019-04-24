package trieris;

import java.util.List;

public class NodePath {
    private Node currentNode;
    private List<Integer> actionsList;
    private int shipDirection;
    
    public NodePath(Node currentNode, List<Integer> actionsList, int shipDirection) {
        this.currentNode = currentNode;
        this.actionsList = actionsList;
        this.shipDirection = shipDirection;
    }
    
    public Node getNode() {
        return currentNode;
    }
    
    public List<Integer> getActionsList() {
        return actionsList;
    }
    
    public int getShipDirection() {
        return shipDirection;
    }
    
    public void setNode(Node newNode) {
        currentNode = newNode;
    }
    
    public void setActionsList(List<Integer> newActionsList) {
        actionsList = newActionsList;
    }
    
    public void setShipDirection(int newShipDirection) {
        shipDirection = newShipDirection;
    }
}

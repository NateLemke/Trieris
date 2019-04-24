package trieris;
import java.awt.Color;
import java.util.ArrayList;
import java.util.List;

public class Node {
    
    private int x;
    private int y;
    
    private Color color;
    
    private Node[] adjacents = new Node[8];
    private Port port;
    private List<Ship> ships = new ArrayList<>();
    private boolean island;
    
    public Node() {
        setColor(Color.BLACK);
    }
    
    public Node(int x, int y) {
        this.x = x;
        this.y = y;
        setColor(Color.BLACK);
    }
    
    public int getX() {
        return x;
    }
    
    public int getY() {
        return y;
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
    
    public boolean isIsland() {
        return island;
    }
    
    public void setPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
    
    public void setColor(Color color) {
        this.color = color;
    }
    
    public void setAdjacentNode(int direction, Node node) {
        adjacents[direction] = node;
    }
    
    public void setPort(Port port) {
        this.port = port;
        setColor(port.getColor());
    }
    
    public void setShips(List<Ship> ships) {
        this.ships = ships;
    }
    
    public void setIsland(boolean isIsland) {
        if(isIsland) {
            setColor(Color.MAGENTA);
        }
        island = isIsland;
    }

    public int getNumberOfShips() {
        return ships.size();
    }
}
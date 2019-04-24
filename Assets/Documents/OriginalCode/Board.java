package trieris;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Scanner;

public class Board {

    public static final int ROW_OF_NODES = 16;
    public static final int COLUMN_OF_NODES = 21;
    private List<Port> ports = new ArrayList<>();
    private List<Ship> ships = new ArrayList<>();
    private List<Node> portNodes = new ArrayList<>();
    
    private Node[][] node;

    public Board() {
        node = new Node[ROW_OF_NODES][COLUMN_OF_NODES];
        initialize();
        setIsland();
        setPorts();
        setAllShips();
        setAdjacentNodes();
    }
    
    public List<Ship> getAllShips() {
        return ships;
    }
    
    public List<Port> getAllPorts(){
    	return ports;
    }
    
    public List<Node> getAllPortNodes(){
    	return portNodes;
    }
    
    public ArrayList<Node> getAllNodes() {

        ArrayList<Node> nodes = new ArrayList<Node>();
        for (int row = 0; row < ROW_OF_NODES; row++) {
            for (int col = 0; col < COLUMN_OF_NODES; col++) {
                Node toAdd = getNodeAt(row, col);
                if (toAdd != null) {
                    nodes.add(getNodeAt(row, col));
                }
            }
        }
        return nodes;
    }

    public Node getNodeAt(int row, int col) {
        return node[row][col];
    }

    private void initialize() {
        for (int row = 0; row < ROW_OF_NODES; row++) {
            for (int col = 0; col < COLUMN_OF_NODES; col++) {
                node[row][col] = new Node(row, col);
            }
        }
    }

    private void setIsland() {
        InputStreamReader fReader;
        fReader = new InputStreamReader(getClass().getResourceAsStream("island.txt"));
        Scanner scanner = new Scanner(fReader);
        while (scanner.hasNext()) {
            String line = scanner.nextLine();
            String[] coordinates = line.split(",");
            int x = Integer.parseInt(coordinates[0]);
            int y = Integer.parseInt(coordinates[1]);
            getNodeAt(x, y).setIsland(true);
        }
        scanner.close();
    }
    
    private void setPorts() {
        for(TeamColor teamColor : TeamColor.values()) {
            setPort(teamColor.portFile, teamColor);
        }
    }

    private void setPort(String file, TeamColor tColor) {
        InputStreamReader fReader;
        fReader = new InputStreamReader(getClass().getResourceAsStream(file));
        Scanner scanner = new Scanner(fReader);
        while(scanner.hasNext()) {
            Port port = null;
            String line = scanner.nextLine();
            String[] coordinates = line.split(",");
            int x = Integer.parseInt(coordinates[0]);
            int y = Integer.parseInt(coordinates[1]);
            int z = Integer.parseInt(coordinates[2]);
            if(z == 0) {
                port = new Port(false, tColor);
                getNodeAt(x, y).setPort(port);
            }
            else if(z == 1) {
                port = new Port(true, tColor);
                getNodeAt(x, y).setPort(port);
            }
            ports.add(port);
            portNodes.add(getNodeAt(x, y));
        }
        scanner.close();
    }
    
    private void setAllShips() {
        HashMap<TeamColor, Integer> teamShipCount = new HashMap<>();
        for(int i = 0; i < 6; i++) {
            teamShipCount.put(TeamColor.values()[i], 0);
        }
              
        for(Node node : getAllNodes()) {
            if(node.getPort() != null) {
                TeamColor key = node.getPort().getTeamColor();
                teamShipCount.put(key, teamShipCount.get(key) + 1);
                int shipID = teamShipCount.get(key);
                ships.add(new Ship(key, shipID, node));
            }
        }
    }
    
    private void setAdjacentNodes() {
        InputStreamReader fReader;
        fReader = new InputStreamReader(getClass().getResourceAsStream("adjacentNodes.txt"));
        Scanner scanner = new Scanner(fReader);
        while(scanner.hasNext()) {
            String line = scanner.nextLine();
            String[] delim = line.split(":");
            String[] coordinates = delim[0].split(",");
            String[] adjacentNodes = delim[1].split(",");
            int x = Integer.parseInt(coordinates[0]);
            int y = Integer.parseInt(coordinates[1]);
            Node node = getNodeAt(x, y);
            Direction[] directionEnum = Direction.values();
            for(int i = 0; i < adjacentNodes.length; i++) {
                if(Integer.parseInt(adjacentNodes[i]) == 1) {
                    int xDiff = directionEnum[i].dx;
                    int yDiff = directionEnum[i].dy;
                    Node adjacentNode = getNodeAt(node.getX() + xDiff, node.getY() + yDiff);
                    node.setAdjacentNode(i, adjacentNode);
                }
            }
        }
        scanner.close();
    }
}
package trieris;

import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.PriorityQueue;
import java.util.Random;

public class TrierisAI {
    private List<Ship> aiShips = new ArrayList<>();
    private Comparator<NodePath> comparator = new NodePathComparator();
    private PriorityQueue<NodePath> queue = new PriorityQueue<NodePath>(8, comparator);
    private final List<Integer> FORWARD = Collections.singletonList(1);
    private final List<Integer> STARBOARD_45 = Arrays.asList(3, 1);
    private final List<Integer> STARBOARD_90 = Arrays.asList(3, 3, 1);
    private final List<Integer> STARBOARD_135 = Arrays.asList(3, 3, 3, 1);
    private final List<Integer> STARBOARD_180 = Arrays.asList(3, 3, 3, 3, 1);
    private final List<Integer> PORT_45 = Arrays.asList(2, 1);
    private final List<Integer> PORT_90 = Arrays.asList(2, 2, 1);
    private final List<Integer> PORT_135 = Arrays.asList(2, 2, 2, 1);
    private List<Integer> shipActions = new ArrayList<>();
    private ArrayList<Node> visitedNodes = new ArrayList<>();
    private ArrayList<Node> destinationPorts = new ArrayList<>();

    TrierisAI(List<Ship> aiShips) {
        this.aiShips = aiShips;
        for (Ship ship : aiShips) {
            ship.setAI(this);
        }
    }

    public void setNextTurn() throws InvalidActionIndexException, CannotReverseException, InvalidActionException {
        destinationPorts = new ArrayList<Node>();
        for (Ship ship : aiShips) {
            if (ship.getNode() != null) {
//                System.out.println(ship.getTeamColor() + "" + ship.getID() + "(" + ship.getNode().getX() + ","
//                        + ship.getNode().getY() + ")");
                queue = new PriorityQueue<NodePath>(8, comparator);
                shipActions = new ArrayList<Integer>();
                visitedNodes = new ArrayList<Node>();
                List<Integer> catapultMove = new ArrayList<Integer>();
                NodePath shipPath = new NodePath(ship.getNode(), shipActions, ship.getFront());
                shipPath = shortestPathToPort(shipPath);
                shipActions = shipPath.getActionsList();
                
//                for (int i = 0; i < shipActions.size(); i++) {
//                    System.out.println(shipActions.get(i));
//                }
                if (shipActions.size() < 4) {
                    for (int i = shipActions.size(); i < 4; i++) {
                        shipActions.add(4);
                    }
                }

                catapultMove = catapultInstructions(ship);
//                System.out.println("catapultMoveSize: " + catapultMove.size());
                int phase = -1;
                int catapultDirection = -1;
                if (catapultMove.size() == 2) {
                    phase = catapultMove.get(0);
                    catapultDirection = catapultMove.get(1);
                }
                
                for (int i = 0; i < ship.getLife(); i++) {
                    if (phase == i) {
                        System.out.println(ship.getTeamColor() + " " + ship.getID());
                        System.out.println("firing catapult in phase: " + phase + " in direction: " + catapultDirection);
                        ship.setAction(phase, shipActions.get(i), catapultDirection);
                    } else {
                        ship.setAction(i, shipActions.get(i), -1);
                    }
                }
            }
        }
    }

    public NodePath shortestPathToPort(NodePath shipPath) {
        visitedNodes.add(shipPath.getNode());
        queue.add(shipPath);

        while (queue.size() != 0) {
            shipPath = queue.poll();
            if (shipPath.getNode().getPort() != null
                    && aiShips.get(0).getTeamColor() != shipPath.getNode().getPort().getTeamColor()
                    && !destinationPorts.contains(shipPath.getNode())) {
                System.out.println("found port");
                System.out.println(shipPath.getNode().getX() + ", " + shipPath.getNode().getY());
                destinationPorts.add(shipPath.getNode());
                break;
            }
            Node[] adjacentNodes = shipPath.getNode().getAdjacentNodes();
            List<Integer> temp;
            for (int i = 0; i < 8; i++) {
                if (adjacentNodes[(shipPath.getShipDirection() + i) % 8] != null) {
                    temp = new ArrayList<Integer>();
                    temp.addAll(shipPath.getActionsList());
                    switch (i) {
                    case 0:
                        temp.addAll(FORWARD);
                        break;
                    case 1:
                        temp.addAll(STARBOARD_45);
                        break;
                    case 2:
                        temp.addAll(STARBOARD_90);
                        break;
                    case 3:
                        temp.addAll(STARBOARD_135);
                        break;
                    case 4:
                        temp.addAll(STARBOARD_180);
                        break;
                    case 5:
                        temp.addAll(PORT_135);
                        break;
                    case 6:
                        temp.addAll(PORT_90);
                        break;
                    case 7:
                        temp.addAll(PORT_45);
                        break;
                    }
                    if (!visitedNodes.contains(adjacentNodes[(shipPath.getShipDirection() + i) % 8])) {
                        visitedNodes.add(adjacentNodes[(shipPath.getShipDirection() + i) % 8]);
                        queue.add(new NodePath(adjacentNodes[(shipPath.getShipDirection() + i) % 8], temp,
                                (shipPath.getShipDirection() + i) % 8));
                    }
                }
            }
        }
        return shipPath;
    }

    public boolean decidePortCapture() {
        return true;
    }

    public Ship selectShip(List<Ship> enemyShips) {
        Random rand = new Random();
        int shipIndex = rand.nextInt(enemyShips.size());
        return enemyShips.get(shipIndex);
    }

    public int setNewShipDirection(Ship ship) {
        queue = new PriorityQueue<NodePath>(8, comparator);
        visitedNodes = new ArrayList<Node>();
        int shipDirection = ship.getFront() + 8;
        List<Integer> temp = new ArrayList<Integer>();
        NodePath newShipPath = new NodePath(ship.getNode(), temp, ship.getFront());
        temp.addAll(shortestPathToPort(newShipPath).getActionsList());
        int index = 0;
        while (temp.get(index) != 1) {
            if (temp.get(index) == 2) {
                shipDirection--;
            }
            if (temp.get(index) == 3) {
                shipDirection++;
            }
            index++;
        }
        return shipDirection % 8;
    }

    public List<Ship> getShips() {
        return aiShips;
    }

    public List<Integer> catapultInstructions(Ship ship) {
        Node shipNode = ship.getNode();
        ArrayList<Integer> catapultDirections = new ArrayList<Integer>();
        int shipDirection = ship.getFront() + 8;
        if (shipActions.get(0) == 1) {
            shipNode = shipNode.getAdjacentNode(shipDirection % 8);
        } else if (shipActions.get(0) == 3) {
            shipDirection++;
        } else if (shipActions.get(0) == 2) {
            shipDirection--;
        }
        shipDirection %= 8;
        Random rand = new Random();
        if (shipNode.getShips().size() > 0) {
            for (int i = 0; i < shipNode.getShips().size(); i++) {
                if (shipNode.getShips().get(i).getTeamColor() != ship.getTeamColor()) {
                    int randNum = rand.nextInt(100);
                    if (randNum % 2 == 0) {
                        catapultDirections.add(0);
                        catapultDirections.add(8);
                        return catapultDirections;
                    } else {
                        catapultDirections.add(0);
                        catapultDirections.add((shipNode.getShips().get(i).getFront() + 8 - shipDirection) % 8);
                        return catapultDirections;
                    }
                }
            }
        }
        shipDirection += 8;
        if (shipActions.get(1) == 1) {
            shipNode = shipNode.getAdjacentNode(shipDirection % 8);
        } else if (shipActions.get(1) == 3) {
            shipDirection++;
        } else if (shipActions.get(1) == 2) {
            shipDirection--;
        }
        shipDirection %= 8;
        Node[] adjacentNodes = shipNode.getAdjacentNodes();
        for (int i = 0; i < 8; i++) {
            if (adjacentNodes[(shipDirection + i) % 8] != null && adjacentNodes[(shipDirection + i) % 8].getShips().size() > 0) {
                for (int j = 0; j < shipNode.getShips().size(); j++) {
                    if (shipNode.getShips().get(j).getTeamColor() != ship.getTeamColor()) {
                        int randNum = rand.nextInt(80);
                        while (shipNode.getAdjacentNode(randNum % 8) == null) {
                            randNum = rand.nextInt(80);
                        }
                        catapultDirections.add(1);
                        catapultDirections.add(((randNum % 8) + 8 - shipDirection) % 8);
                        return catapultDirections;
                    }
                }
            }
        }
        return catapultDirections;
    }
}
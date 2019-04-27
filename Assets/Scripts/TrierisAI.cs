using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TrierisAI {

    // AI manage teams, so might as well give them access to a team's list of ships
    //private List<Ship> aiShips = new List<Ship>();
    Team team;


    // comparator is a java thing, for comparing!
    // uses NodePathComparator class
    // we can implement IComparable or something to use this? 
    //private Comparator<NodePath> comparator = new NodePathComparator();

    // Priority queue is, unsurprisingly, a java priority queue based on a priority heap
    //private PriorityQueue<NodePath> queue = new PriorityQueue<NodePath>(8,comparator);

    private List<NodePath> queue = new List<NodePath>();
    private readonly int[] FORWARD = { 1 };
    private readonly int[] STARBOARD_45 = { 3,1 };
    private readonly int[] STARBOARD_90 = { 3,3,1 };
    private readonly int[] STARBOARD_135 = { 3,3,3,1 };
    private readonly int[] STARBOARD_180 = { 3,3,3,3,1 };
    private readonly int[] PORT_45 = { 2,1 };
    private readonly int[] PORT_90 = { 2,2,1 };
    private readonly int[] PORT_135 = { 2,2,2,1 };
    private List<int> shipActions = new List<int>();
    private List<Node> visitedNodes = new List<Node>();
    private List<Node> destinationPorts = new List<Node>();

    //TrierisAI(List<Ship> aiShips) {
    //    this.aiShips = aiShips;
    //    foreach (Ship ship in aiShips) {
    //        ship.setAI(this);
    //    }
    //}

    public TrierisAI(Team t) {
        if(t == null) {
            Debug.Log("Assigning an AI a NULL team");
        }
        team = t;
        if(team.ships.Count == 0) {
            Debug.Log("Assigning an AI a team with no ships");
        }
        foreach (Ship ship in team.ships) {
            ship.setAI(this);
        }
    }

    // throws InvalidActionIndexException, CannotReverseException, InvalidActionException
    public void setNextTurn()  {
        destinationPorts = new List<Node>();
        foreach (Ship ship in team.ships) {
            if (ship.getNode() != null) {
                // System.out.println(ship.getTeamColor() + "" + ship.getID() + "(" + ship.getNode().getX() + ","
                //         + ship.getNode().getY() + ")");

                // QUEUE STUFF
                //queue = new PriorityQueue<NodePath>(8, comparator);
                queue = new List<NodePath>();

                shipActions = new List<int>();
                visitedNodes = new List<Node>();
                List<int> catapultMove = new List<int>();
                NodePath shipPath = new NodePath(ship.getNode(),shipActions,ship.getFront());
                shipPath = shortestPathToPort(shipPath);
                shipActions = shipPath.getActionsList();
                
                // for (int i = 0; i < shipActions.size(); i++) {
                //     System.out.println(shipActions.get(i));
                // }
                if (shipActions.Count < 4) {
                    for (int i = shipActions.Count; i< 4; i++) {
                        shipActions.Add(4);
                    }
                }

                catapultMove = catapultInstructions(ship);
                // System.out.println("catapultMoveSize: " + catapultMove.size());
                int phase = -1;
                int catapultDirection = -1;
                if (catapultMove.Count == 2) {
                    phase = catapultMove[0];
                    catapultDirection = catapultMove[1];
                    if(catapultDirection != -1 && phase != -1) {
                        //Debug.Log("Setting catapult for phase: " + phase + " dir: "+catapultDirection);
                    }
                }
                
                for (int i = 0; i<ship.getLife(); i++) {
                    if (phase == i) {
                        DebugControl.log("AI",ship.team + " " + ship.getID());
                        DebugControl.log("AI", "firing catapult in phase: " + phase + " in direction: " + catapultDirection);
                        ship.setAction(phase, shipActions[i], catapultDirection);
                    } else {
                        ship.setAction(i, shipActions[i], -1);
                    }
                }
            }
        }
    }

    public NodePath shortestPathToPort(NodePath shipPath) {
        visitedNodes.Add(shipPath.getNode());
        queue.Add(shipPath);

        // so queue was originally a priority queue in java, which means it has sorted insertion, right?
        // so Im prety sure we need to sort out list every time we insert something new
        queue.Sort();

        while (queue.Count != 0) {
            //shipPath = queue.poll();
            shipPath = queue[0];
            queue.RemoveAt(0);
            if (shipPath.getNode().getPort() != null
                    && team.ships[0].team != shipPath.getNode().getPort().getTeam()
                    && !destinationPorts.Contains(shipPath.getNode())) {
                DebugControl.log("AI","found port");
                DebugControl.log("AI",shipPath.getNode().getX() + ", " + shipPath.getNode().getY());

                //Debug.Log("AI found port");
                //Debug.Log("AI" + shipPath.getNode().getX() + ", " + shipPath.getNode().getY());
                destinationPorts.Add(shipPath.getNode());
                break;
            }
            Node[] adjacentNodes = shipPath.getNode().getAdjacentNodes();
            List<int> temp;
            for (int i = 0; i < 8; i++) {
                if (adjacentNodes[(shipPath.getShipDirection() + i) % 8] != null) {
                    temp = new List<int>();
                    temp.AddRange(shipPath.getActionsList());
                    switch (i) {
                        case 0:
                        temp.AddRange(FORWARD);
                        break;
                        case 1:
                        temp.AddRange(STARBOARD_45);
                        break;
                        case 2:
                        temp.AddRange(STARBOARD_90);
                        break;
                        case 3:
                        temp.AddRange(STARBOARD_135);
                        break;
                        case 4:
                        temp.AddRange(STARBOARD_180);
                        break;
                        case 5:
                        temp.AddRange(PORT_135);
                        break;
                        case 6:
                        temp.AddRange(PORT_90);
                        break;
                        case 7:
                        temp.AddRange(PORT_45);
                        break;
                    }
                    if (!visitedNodes.Contains(adjacentNodes[(shipPath.getShipDirection() + i) % 8])) {
                        visitedNodes.Add(adjacentNodes[(shipPath.getShipDirection() + i) % 8]);
                        queue.Add(new NodePath(adjacentNodes[(shipPath.getShipDirection() + i) % 8],temp,
                                (shipPath.getShipDirection() + i) % 8));

                        // so queue was originally a priority queue in java, which means it has sorted insertion, right?
                        // so Im prety sure we need to sort out list every time we insert something new
                        queue.Sort();
                    }
                }
            }
        }
        return shipPath;
    }

    public bool decidePortCapture() {
        return true;
    }

    public Ship selectShip(List<Ship> enemyShips) {
           System.Random rand = new System.Random();
    
        int shipIndex = rand.Next(enemyShips.Count);
        return enemyShips[shipIndex];
    }

    public int setNewShipDirection(Ship ship) {
        //Debug.Log("setting new AI direction for "+ship);
        ship.needRedirect = false;
        //queue = new PriorityQueue<NodePath>(8,comparator);
        queue = new List<NodePath>();
        visitedNodes = new List<Node>();
        int shipDirection = ship.getFront() + 8;
        List<int> temp = new List<int>();
        NodePath newShipPath = new NodePath(ship.getNode(),temp,ship.getFront());
        temp.AddRange(shortestPathToPort(newShipPath).getActionsList());
        int index = 0;
        while (temp[index] != 1) {
            if (temp[index] == 2) {
                shipDirection--;
            }
            if (temp[index] == 3) {
                shipDirection++;
            }
            index++;
        }
        return shipDirection % 8;
    }

    public List<Ship> getShips() {
        return team.ships;
    }

    public List<int> catapultInstructions(Ship ship) {
        Node shipNode = ship.getNode();
        List<int> catapultDirections = new List<int>();
        int shipDirection = ship.getFront() + 8;
        if (shipActions[0] == 1) {
            shipNode = shipNode.getAdjacentNode(shipDirection % 8);
        } else if (shipActions[0] == 3) {
            shipDirection++;
        } else if (shipActions[0] == 2) {
            shipDirection--;
        }
        shipDirection %= 8;
            System.Random rand = new System.Random();
        if (shipNode.getShips().Count > 0) {
            for (int i = 0; i < shipNode.getShips().Count; i++) {
                if (shipNode.getShips()[i].team != ship.team) {
                    int randNum = rand.Next(100);
                    if (randNum % 2 == 0) {
                        catapultDirections.Add(0);
                        catapultDirections.Add(8);
                        return catapultDirections;
                    } else {
                        catapultDirections.Add(0);
                        catapultDirections.Add((shipNode.getShips()[i].getFront() + 8 - shipDirection) % 8);
                        return catapultDirections;
                    }
                }
            }
        }
        shipDirection += 8;
        if (shipActions[1] == 1) {
            shipNode = shipNode.getAdjacentNode(shipDirection % 8);
        } else if (shipActions[1] == 3) {
            shipDirection++;
        } else if (shipActions[1] == 2) {
            shipDirection--;
        }
        shipDirection %= 8;
        Node[] adjacentNodes = shipNode.getAdjacentNodes();
        for (int i = 0; i < 8; i++) {
            if (adjacentNodes[(shipDirection + i) % 8] != null && adjacentNodes[(shipDirection + i) % 8].getShips().Count > 0) {
                for (int j = 0; j < shipNode.getShips().Count; j++) {
                    if (shipNode.getShips()[j].team != ship.team) {
                        int randNum = rand.Next(80);
                        while (shipNode.getAdjacentNode(randNum % 8) == null) {
                            randNum = rand.Next(80);
                        }
                        catapultDirections.Add(1);
                        catapultDirections.Add(((randNum % 8) + 8 - shipDirection) % 8);
                        return catapultDirections;
                    }
                }
            }
        }
        return catapultDirections;
    }

    public Team GetTeam() {
        return team;
    }
}

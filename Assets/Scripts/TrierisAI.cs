﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Gives commands to all AI ships.
/// has functions to find the nearest port, set attacks, and set directions.
/// </summary>
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

    /// <summary>
    /// Basic constructor for the AI.
    /// Sets the team to given one. 
    /// Doesn't work if given team is null or has no ships.
    /// </summary>
    /// <param name="t">The team to set this AI to.</param>
    public TrierisAI(Team t) {
        if(t == null) {
            Debug.Log("Assigning an AI a NULL team");
        }
        team = t;
        team.aiTeam = true;
        if(team.ships.Count == 0) {
            Debug.Log("Assigning an AI a team with no ships");
        }
        foreach (Ship ship in team.ships) {
            ship.Ai = this;
        }
    }

    /// <summary>
    /// Sets the next turn of actions and attack for the AI ship.
    /// Throws InvalidActionIndexException, CannotReverseException, InvalidActionException
    /// </summary>
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
                        DebugControl.log("AI",ship.team + " " + ship.Id);
                        DebugControl.log("AI", "firing catapult in phase: " + phase + " in direction: " + catapultDirection);
                        ship.setAction(phase, shipActions[i], catapultDirection);
                    } else {
                        ship.setAction(i, shipActions[i], -1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Finds the shortest path to a port.
    /// </summary>
    /// <param name="shipPath">The current path.</param>
    /// <returns>The shortest path to a port.</returns>
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
            if (shipPath.getNode().Port != null
                    && team.ships[0].team != shipPath.getNode().Port.Team
                    && !destinationPorts.Contains(shipPath.getNode())) {
                DebugControl.log("AI","found port");
                DebugControl.log("AI",shipPath.getNode().X + ", " + shipPath.getNode().Y);

                //Debug.Log("AI found port");
                //Debug.Log("AI" + shipPath.getNode().getX() + ", " + shipPath.getNode().getY());
                destinationPorts.Add(shipPath.getNode());
                break;
            }
            Node[] adjacentNodes = shipPath.getNode().Adjacents;
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

    /// <summary>
    /// Returns wether the ship decides to catpure a port (currently always captures.)
    /// </summary>
    /// <returns>true</returns>
    public bool decidePortCapture() {
        return true;
    }

    /// <summary>
    /// Randomly selects an enemy ship to attack or ram.
    /// </summary>
    /// <param name="enemyShips">a list of possible enemy targets.</param>
    /// <returns>the randomly selected target.</returns>
    public Ship selectShip(List<Ship> enemyShips) {
        System.Random rand = new System.Random();
    
        int shipIndex = rand.Next(enemyShips.Count);

        try {
            return enemyShips[shipIndex];
        } catch (ArgumentOutOfRangeException e) {
            Debug.LogError("random.next is exclusive max so Im not sure how this happened?");
        }
        return null;
    }

    /// <summary>
    /// Sets a new direction for the ship. 
    /// </summary>
    /// <param name="ship">The AI ship.</param>
    /// <returns>The new direction.</returns>
    public int setNewShipDirection(Ship ship) {
        //Debug.Log("setting new AI direction for "+ship);
        ship.NeedRedirect = false;
        //queue = new PriorityQueue<NodePath>(8,comparator);
        queue = new List<NodePath>();
        visitedNodes = new List<Node>();
        int shipDirection = ship.getFront() + 8;
        List<int> temp = new List<int>();
        NodePath newShipPath = new NodePath(ship.getNode(),temp,ship.getFront());
        temp.AddRange(shortestPathToPort(newShipPath).getActionsList());
        int index = 0;

        try {
            while (temp[index] != 1) {
                if (temp[index] == 2) {
                    shipDirection--;
                }
                if (temp[index] == 3) {
                    shipDirection++;
                }
                index++;
            }
        } catch (ArgumentOutOfRangeException e) {
            Debug.LogError("ArgumentOutOfRangeException for AI ship "+ship);
        }

        
        return shipDirection % 8;
    }

    /// <summary>
    /// Returns a list of all ships on this team.
    /// </summary>
    /// <returns>a list of all ships on this team.</returns>
    public List<Ship> getShips() {
        return team.ships;
    }

    /// <summary>
    /// Gives attack commands to the given ship if there are valid targets.
    /// </summary>
    /// <param name="ship">The AI ship</param>
    /// <returns>The Catapult Directions.</returns>
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
        if (shipNode.Ships.Count > 0) {
            for (int i = 0; i < shipNode.Ships.Count; i++) {
                if (shipNode.Ships[i].team != ship.team) {
                    int randNum = rand.Next(100);
                    if (randNum % 2 == 0) {
                        catapultDirections.Add(0);
                        catapultDirections.Add(8);
                        return catapultDirections;
                    } else {
                        catapultDirections.Add(0);
                        catapultDirections.Add((shipNode.Ships[i].getFront() + 8 - shipDirection) % 8);
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
        Node[] adjacentNodes = shipNode.Adjacents;
        for (int i = 0; i < 8; i++) {
            if (adjacentNodes[(shipDirection + i) % 8] != null && adjacentNodes[(shipDirection + i) % 8].Ships.Count > 0) {
                for (int j = 0; j < shipNode.Ships.Count; j++) {
                    if (shipNode.Ships[j].team != ship.team) {
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

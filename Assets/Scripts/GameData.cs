using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public float[,] shipPositions;

    public int[] shipDirection;
    public int[,] shipActions;

    public int[] shipCatapultPhase;
    public int[] shipCatapultDirection;

    public int[] shipHealth;
    public bool[] shipSunk;    

    public int[] portOwners;

    public GameData() {
        Initialize();
    }

    public void Initialize() {
        shipPositions = new float[30,2];

        shipDirection = new int[30];
        shipActions = new int[30,4];

        shipCatapultPhase = new int[30];
        shipCatapultDirection = new int[30];

        shipHealth = new int[30];
        shipSunk = new bool[30];
        
        portOwners = new int[30];
    }

    public void GrabData() {
        foreach(Team t in GameManager.main.teams) {
            int teamDataID = (int)t.TeamFaction * 5;
            for(int i = 0; i < 5; i++) {
                int shipDataID = teamDataID + i;
                Ship ship = t.ships[i];

                shipSunk[shipDataID] = ship.isSunk;

                if (!ship.isSunk) {
                    continue;
                }

                shipPositions[shipDataID,0] = ship.getNode().X;
                shipPositions[shipDataID,0] = ship.getNode().Y;

                shipDirection[shipDataID] = ship.Direction;
                shipActions[shipDataID,0] = ship.actions[0].actionType;
                shipActions[shipDataID,1] = ship.actions[1].actionType;
                shipActions[shipDataID,2] = ship.actions[2].actionType;
                shipActions[shipDataID,3] = ship.actions[3].actionType;

                shipCatapultPhase[shipDataID] = ship.CatapultPhaseIndex;
                shipCatapultDirection[shipDataID] = ship.CatapultDirection;

                shipHealth[shipDataID] = ship.life;
                
            }
        }

        foreach(Port p in GameManager.main.Board.ports) {
            portOwners[p.id] = (int)p.Team.TeamFaction;
        }
    }
}

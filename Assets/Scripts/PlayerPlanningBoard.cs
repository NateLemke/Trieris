using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanningBoard {
    private List<Ship> playerShips;

    public PlayerPlanningBoard(List<Ship> playerShips) {
        this.playerShips = playerShips;
    }

    public void setShipAction(int ship,int phaseIndex,int phaseType,int catapultDirection) {        // throws cannotreverseexception, invalidactionexception, invalidactionindexexception
        //playerShips.get(ship).setAction(phaseIndex,phaseType,catapultDirection);
    }

}


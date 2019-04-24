using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TrierisUIInterface {

    Ship promptPlayerShips(string message,List<Ship> ships);

    int promptPlayerDirection(string message);

    bool promptPlayerCapture(string message);

    TeamColor promptPlayerTeam();

    void updateMapDisplay();

    void setShipAction(int shipIndex,int actionIndex,int actionType,int catapultDirection);

          //  throws CannotReverseException, InvalidActionException, InvalidActionIndexException;

    void executeTurn();

    void ready();

    void victory(TeamColor color);
}

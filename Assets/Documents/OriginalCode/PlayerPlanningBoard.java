package trieris;

import java.util.List;

public class PlayerPlanningBoard {
    private List<Ship> playerShips;
    
    public PlayerPlanningBoard(List<Ship> playerShips) {
        this.playerShips = playerShips;
    }
    
    public void setShipAction(int ship, int phaseIndex, int phaseType, int catapultDirection) 
            throws CannotReverseException, InvalidActionException, InvalidActionIndexException {
        playerShips.get(ship).setAction(phaseIndex, phaseType, catapultDirection);
    }

}

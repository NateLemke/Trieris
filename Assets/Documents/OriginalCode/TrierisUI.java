package trieris;

import java.util.List;

public interface TrierisUI {
    
	Ship promptPlayerShips(String message, List<Ship> ships);
	
	int promptPlayerDirection(String message);
	
	boolean promptPlayerCapture(String message);
	
	TeamColor promptPlayerTeam();
	
	void updateMapDisplay();
	
	void setShipAction(int shipIndex, int actionIndex, int actionType, int catapultDirection) 
	        throws CannotReverseException, InvalidActionException, InvalidActionIndexException;
	
	void executeTurn();
	
	void ready();

	void victory(TeamColor color);
}

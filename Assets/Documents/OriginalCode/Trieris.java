package trieris;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;


public class Trieris {
    
    private Board board;
    private Controller controller;
    private PlayerPlanningBoard planBoard;
    private List<Ship> ships;
    private TrierisUI trierisUI;
    public List<Ship> playerShips = new ArrayList<>();
    List<Ship> allAIShips = new ArrayList<>();
    private List<TrierisAI> aiList;
    List<Ship> aiShips = new ArrayList<>();
    private boolean gameOver = false;
    
    public Trieris() {
        board = new Board();
        ships = board.getAllShips();
    }
    
    public void setUI(TrierisUI ui) {
        this.trierisUI = ui;
    }
    
    public List<Ship> getPlayerShips() {
        return playerShips;
    }
    
    public void start() throws NoTrierisInterfaceException {
        // ensure UI is set before starting
        if (trierisUI == null) {
            throw new NoTrierisInterfaceException();
        }
        
        // set player ships and board
        TeamColor playerColor = trierisUI.promptPlayerTeam();
        playerShips = ships.stream().filter(ship -> ship.getTeamColor() == playerColor).
                collect(Collectors.toList());
        planBoard = new PlayerPlanningBoard(playerShips);
        
        // set ai ships
        aiList = new ArrayList<TrierisAI>();
        List<TeamColor> aiColors = new LinkedList<TeamColor>(Arrays.asList(TeamColor.values()));
        aiColors.remove(playerColor);
        for (TeamColor aicolor : aiColors) {
            aiShips = ships.stream().filter(ship -> ship.getTeamColor() == aicolor).
                    collect(Collectors.toList());
            aiList.add(new TrierisAI(aiShips));   
            for(Ship ship : aiShips) {
                allAIShips.add(ship);
            }
        }

        // set controller
        controller = new Controller(this, ships, playerShips, allAIShips);
        
        // set ships initial face
        promptInitialFace();
        for (TrierisAI ai : aiList) {
            for (Ship ship : ai.getShips()) {
                int direction = ai.setNewShipDirection(ship);
                ship.setFront(direction);
            }
        }
        trierisUI.updateMapDisplay();
    }
    
    public TrierisUI getUI() {
        return trierisUI;
    }
    

    public Board getBoard() {
        return board;
    }
    
    public void setShipAction(int shipIndex, int actionIndex, int actionType, int catapultDirection)
            throws CannotReverseException, InvalidActionException, InvalidActionIndexException {
        planBoard.setShipAction(shipIndex, actionIndex, actionType, catapultDirection);
    }

    public boolean executeTurn() {
        if(!gameOver && controller.executeTurn()) {
            trierisUI.updateMapDisplay();
            return true;
        }
        return false;
    }
    
    public void setAIActions() {
        if (!gameOver) {
            for (TrierisAI ai : aiList) {
                try {
                    ai.setNextTurn();
                } catch (InvalidActionIndexException | CannotReverseException | InvalidActionException e) {
                    e.printStackTrace();
                }
            }
        }
    }
    
    public Ship promptPlayerShips(String message, List<Ship> shipChoices) {
        if(shipChoices.size() == 1) {
            return shipChoices.get(0);
        }
        Ship result = trierisUI.promptPlayerShips(message, shipChoices);
        return result == null ? shipChoices.get(0) : result;
    }
    
    public int promptPlayerDirection(String message) {
        return trierisUI.promptPlayerDirection(message);
    }
    
    public boolean promptPlayerCapture(String message) {
        return trierisUI.promptPlayerCapture(message);
    }
    
    public void checkVictory() {
        Map<TeamColor, Integer> distribution = new HashMap<TeamColor, Integer>();
        for (TeamColor color : TeamColor.values()) {
            distribution.put(color, 0);
        }
        for (Node portNode : board.getAllPortNodes()) {
            if (portNode.getPort() != null) {
                TeamColor color = portNode.getPort().getTeamColor();
                int currentValue = distribution.get(color);
                distribution.put(color, currentValue + 1);
                if (currentValue + 1 >= 12) {
                    victory(color);
                }
            }
        }
        
    }
    
    private void victory(TeamColor color) {
        trierisUI.victory(color);
        this.gameOver = true;
    }
    
    public void promptInitialFace() {
        for (Ship ship : playerShips) {
            int direction = trierisUI.promptPlayerDirection("Choose ship " + ship.getID() + "'s initial direction.");
            ship.setFront(direction);
        }
        /*
        for (TrierisAI ai : aiList) {
            for (Ship ship : ai.getShips()) {
                int direction = ai.setNewShipDirection(ship);
                ship.setFront(direction);
            }
        }
        */
    }
}

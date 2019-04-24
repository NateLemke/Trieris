package trieris;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

public class Controller {

    public static final int PHASES = 1;

    private Trieris trieris;
    private List<Ship> ships = new ArrayList<>();
    private List<Ship> playerShips = new ArrayList<>();
    private List<Ship> aiShips = new ArrayList<>();
    private int index = 0;

    public Controller(Trieris trieris, List<Ship> ships, List<Ship> playerShips, List<Ship> aiShips) {
        this.trieris = trieris;
        this.ships = ships;
        this.playerShips = playerShips;
        this.aiShips = aiShips;
    }

    public boolean executeTurn() {
        for (Ship ship : ships) {
            if (!(ship.ready()))
                ship.populateDefaultActions();
        }
        for (int i = 0; i < PHASES; i++) {
            executePhase(index);
            if (index == 3) {
                resetShips();
                trieris.checkVictory();
                index = 0;
            } else {
                index++;
            }
        }
        return true;
    }

    private void executePhase(int turn) {
        for (Ship ship : ships) {
            if (ship.getCanAct()) {
                try {
                    ship.doAction(turn);
                } catch (ShipCrashedException e) {
                    int newDirection = 0;
                    if(ship.getAI() != null) {
                        newDirection = ship.getAI().setNewShipDirection(ship);
                    } else {
                        newDirection = trieris.promptPlayerDirection(e.getMessage());
                    }
                    ship.setFront(newDirection);
                }
            }
        }

        handleCollisions();
        handleCapture();
        updateShips();
        handleCatapults();
        sinkShips();
    }

    private void handleCapture() {
        for (Ship ship : ships) {
            Port port = ship.getNode().getPort();
            if (port != null && port.getTeamColor() != ship.getTeamColor()) {
                List<Ship> enemyShips = ship.getNode().getShips().stream()
                        .filter(target -> target.getTeamColor() != ship.getTeamColor()).collect(Collectors.toList());
                if (enemyShips.size() == 0) {
                  
                    if (playerShips.contains(ship)) {
                        if (trieris.promptPlayerCapture("Ship " + ship.getID() + " may capture the port.")) {
                            ship.capturePort();
                            int direction = trieris.promptPlayerDirection("Set a direction for ship " + ship.getID() + " to face.");
                            ship.setFront(direction);
                        }
                    } else {
                        if (ship.getAI().decidePortCapture()) {
                            ship.capturePort();
                            int direction = ship.getAI().setNewShipDirection(ship);
                            ship.setFront(direction);
                        }
                    }
                }
            }
        }
    }

    // First checks if the ship in question has moved, the checks if node ship
    // is at has more than 1 ship,
    // then compares current ship to others to see if they collide and calls
    // collision handler from the
    // Event Handler class, where I will either pass in the node of the
    // collision or the ship list at that node
    private void handleCollisions() {
        for (Ship ship : ships) {
            if (ship.getMoved()) {
                if (ship.getNode().getNumberOfShips() > 1) {
                    List<Ship> enemyShips = ship.getNode().getShips().stream()
                            .filter(target -> target.getTeamColor() != ship.getTeamColor())
                            .collect(Collectors.toList());
                    List<Ship> potentialCollisions = didCollide(enemyShips, ship);

                    if (potentialCollisions.size() != 0) {
                        Ship chosenShip = null;
                        if (playerShips.contains(ship)) {
                           chosenShip = trieris.promptPlayerShips("Ship " + ship.getID() + "can collide with multiple ships.", potentialCollisions);
                        } else {
                           chosenShip = ship.getAI().selectShip(potentialCollisions);
                        }
                        ship.ram(chosenShip);
                    }
                }
            }
        }
    }

    private List<Ship> didCollide(List<Ship> enemyShips, Ship ship) {
        List<Ship> potentialCollisions = new ArrayList<>();
        for (int i = 0; i < enemyShips.size(); i++) {
            if (!(ship.getFront() == enemyShips.get(i).getFront() && (enemyShips.get(i).getMoved()))) { 
                potentialCollisions.add(enemyShips.get(i));
            }
        }
        return potentialCollisions;
    }

    private void updateShips() {
        for (Ship ship : ships) {
            ship.updateFrontAfterCollision();
        }
    }

    // Detects catapults by checking each ships hasFired()
    private void handleCatapults() {
        for (Ship ship : ships) {
            Node node = ship.getCatapultNode();
            if (node != null && node.getShips().size() > 0) {

                List<Ship> potentialTargets = node.getShips().stream()
                        .filter(target -> target.getTeamColor() != ship.getTeamColor()).collect(Collectors.toList());
                Ship chosenShip = null;
                if(playerShips.contains(ship)) {
                    if (potentialTargets.size() > 0) {
                        chosenShip = trieris.promptPlayerShips("Ship " + ship.getID() + " may attack multiple ships.", potentialTargets); 
                    }
                } else {
                    if (potentialTargets.size() > 0) {
                        chosenShip = ship.getAI().selectShip(potentialTargets);
                    }
                }

                ship.catapult(chosenShip);
            }
        }
    }

    // 'Sinks' ships by removing them from the lists of ships
    private void sinkShips() {
        ArrayList<Ship> sunkShips = new ArrayList<>();
        for (Ship ship : ships) {
            if (ship.getLife() <= 0) {
                sunkShips.add(ship);
            }
        }
        for (Ship ship : sunkShips) {
            ship.getNode().getShips().remove(ship);
            ship.sink();
            ships.remove(ship);
            /*
            if (aiShips.contains(ship)) {
                aiShips.remove(ship);
            }
            if (playerShips.contains(ship)) {
                playerShips.remove(ship);
            }
            */
        }
    }

    private void resetShips() {
        for (Ship ship : ships) {
            ship.reset();
        }
    }
}
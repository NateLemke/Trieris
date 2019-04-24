using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {

    public static int PHASES = 4;

    private GameManager gameManager;
    //private List<Ship> ships = new List<Ship>();
    private List<Ship> playerShips = new List<Ship>();
    private List<Ship> aiShips = new List<Ship>();
    //private int index = 0;

    // NEW STUFF
    public ushort phaseIndex = 0;

    enum Phases { planning, phaseOne, phaseTwo, phaseThree };
    bool readyNext = false;

    Image[] actionImages = new Image[4];

    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        actionImages[0] = GameObject.Find("ActionImage1").GetComponent<Image>();
        actionImages[1] = GameObject.Find("ActionImage2").GetComponent<Image>();
        actionImages[2] = GameObject.Find("ActionImage3").GetComponent<Image>();
        actionImages[3] = GameObject.Find("ActionImage4").GetComponent<Image>();
    }

    //public GameLogic(GameManager trieris,List<Ship> ships,List<Ship> playerShips,List<Ship> aiShips) {
    //    this.gameManager = trieris;
    //    this.ships = ships;
    //    this.playerShips = playerShips;
    //    this.aiShips = aiShips;
    //}

        [System.Obsolete]
    public bool executeTurn() {
        Debug.Log("BEGIN OLD OLD OLD TURN");
        //for (Ship ship : ships) {
        //    if (!(ship.ready()))
        //        ship.populateDefaultActions();
        //}
        foreach (Ship ship in gameManager.getShips()) {
            if (!(ship.ready()))
                ship.populateDefaultActions();
        }
        
        for (int i = 0; i < PHASES; i++) {
            executePhase(phaseIndex);
            if (phaseIndex == 3) {
                // I commented out ships resetting before, why? :thinking:
                resetShips();
                gameManager.checkVictory();
                phaseIndex = 0;
            } else {
                phaseIndex++;
            }
        }
        return true;
    }

    public void newExecuteTurn() {
        DebugControl.log("turn","BEGIN TURN");
        gameManager.setAIActions();
        gameManager.processingTurn = true;
        foreach (Ship ship in gameManager.getPlayerShips()) {
            ship.canActAfterCollision = true;
            if (!(ship.ready())) {
                ship.populateDefaultActions();
                Debug.Log("ship not ready");
            }                
        }
        
        phaseIndex = 0;
        executePhase(phaseIndex);

    }

    //[System.Obsolete]
    public bool executePhase() {
        if(phaseIndex >= 3) {
            gameManager.checkVictory();

            foreach (Ship s in gameManager.getPlayerShips())
            {
                s.currentActionIndex = 0;
                s.actions = new Ship.Action[4];
            }

            Image image;
            for(int i = 0; i < 4; i++)
            {
                image = actionImages[i].GetComponent<Image>();
                var tempCol = image.color;
                image.sprite = null;
                tempCol.a = 0;
                image.color = tempCol;
            }

            return false;
        } else {
            phaseIndex++;
            executePhase(phaseIndex);
            return true;
        }
    }

    private void executePhase(int phase) {
        DebugControl.log("turn","--PHASE "+phase);
        handleCapture();
        foreach (Ship ship in gameManager.getShips()) {
            if (ship.getCanAct()) {
                try {
                    ship.doAction(phase);
                } catch (ShipCrashedException e) {
                    int newDirection = 0;
                    if (ship.team != gameManager.playerTeam) {
                        newDirection = ship.getAI().setNewShipDirection(ship);
                    } else {
                        //newDirection = gameManager.promptPlayerDirection(e.getMessage());
                    }
                    ship.setFront(newDirection);
                }
            } else {
                //Debug.Log("ship " + ship.getID() + " cannot act");
            }
        }

        handleCollisions();
        updateShips();
        handleCatapults();
        sinkShips();
    }

    private void handleCapture() {
        foreach (Ship ship in gameManager.getShips()) {
            Port port = ship.getNode().getPort();
            if (port != null && port.getTeam() != ship.team) {
                //List<Ship> enemyShips = new List<Ship>();
                //List<Ship> portShips = ship.getNode().getShips();
                int enemyShipNo = 0;
                int playerShipNo = 0;
                foreach(Ship s in port.node.getShips())
                {
                    if (s.team == gameManager.playerTeam)
                        enemyShipNo++;
                    else
                        playerShipNo++;
                }
                //List<Ship> enemyShips = ship.getNode().getShips().stream()
                //        .filter(target->target.getTeamColor() != ship.getTeamColor()).collect(Collectors.toList());
                //if (enemyShips.Count == 0) {}
                if (ship.team == gameManager.playerTeam && enemyShipNo == 0)
                {
                    /*Prompt Capture
                    if (gameManager.promptPlayerCapture("Ship " + ship.getID() + " may capture the port.")) {
                        ship.capturePort();
                        int direction = gameManager.promptPlayerDirection("Set a direction for ship " + ship.getID() + " to face.");
                        ship.setFront(direction);
                    }
                    */
                    ship.capturePort();
                    int direction = gameManager.promptPlayerDirection("Set a direction for ship " + ship.getID() + " to face.");
                    ship.setFront(direction);
                }
                else if(playerShipNo == 0)
                {
                    // commenting out 

                    Debug.Log("Handle AI capture");

                    if (ship.getAI().decidePortCapture())
                    {
                        ship.capturePort();
                        int direction = ship.getAI().setNewShipDirection(ship);
                        ship.setFront(direction);
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
        foreach (Ship ship in gameManager.getShips()) {
            if (ship.getMoved()) {
                DebugControl.log("position","ship " + ship.getID() + " at " +ship.getNode().ToString());
                if (ship.getNode().getNumberOfShips() > 1) {
                    //List<Ship> enemyShips = ship.getNode().getShips().stream()
                    //        .filter(target->target.getTeamColor() != ship.getTeamColor())
                    //        .collect(Collectors.toList());
                    List<Ship> enemyShips = new List<Ship>();
                    foreach(Ship nodeShip in ship.getNode().getShips()) {
                        if(nodeShip.team != ship.team) {
                            enemyShips.Add(nodeShip);
                        }
                    }
                    List<Ship> potentialCollisions = didCollide(enemyShips,ship);

                    if (potentialCollisions.Count != 0) {
                        Ship chosenShip = null;
                        //Debug.Log("ram happening");

                        if (playerShips.Contains(ship)) {
                            //temp collision decision
                            chosenShip = potentialCollisions[0];

                            //chosenShip = gameManager.promptPlayerShips("Ship " + ship.getID() + "can collide with multiple ships.",potentialCollisions);
                        } else {
                            chosenShip = ship.getAI().selectShip(potentialCollisions);
                        }
                        ship.ram(chosenShip);
                    }
                }
            }
        }
    }

    private List<Ship> didCollide(List<Ship> enemyShips,Ship ship) {
        List<Ship> potentialCollisions = new List<Ship>();
        for (int i = 0; i < enemyShips.Count; i++) {
            if (!(ship.getFront() == enemyShips[i].getFront() && (enemyShips[i].getMoved()))) {
                potentialCollisions.Add(enemyShips[i]);
            }
        }
        return potentialCollisions;
    }

    private void updateShips() {
        foreach (Ship ship in gameManager.getShips()) {
            ship.updateFrontAfterCollision();
        }
    }

    // Detects catapults by checking each ships hasFired()
    private void handleCatapults() {
        foreach (Ship ship in gameManager.getShips()) {
            Node node = ship.getCatapultNode();
            if (node != null && node.getShips().Count > 0) {

                //List<Ship> potentialTargets = node.getShips().stream()
                //        .filter(target->target.getTeamColor() != ship.getTeamColor()).collect(Collectors.toList());

                List<Ship> potentialTargets = new List<Ship>();

                foreach(Ship s in node.getShips()) {
                    if(s.team != ship.team) {
                        potentialTargets.Add(s);
                    }
                }


                Ship chosenShip = null;
                if (playerShips.Contains(ship)) {
                    if (potentialTargets.Count > 0) {
                        // need to get player input later
                        //chosenShip = gameManager.promptPlayerShips("Ship " + ship.getID() + " may attack multiple ships.",potentialTargets);

                        chosenShip = potentialTargets[0];
                    }
                } else {
                    if (potentialTargets.Count > 0) {
                        chosenShip = ship.getAI().selectShip(potentialTargets);
                    }
                }
                Debug.Log("Game logic firing catapult");
                ship.catapult(chosenShip);
            }
        }
    }

    // 'Sinks' ships by removing them from the lists of ships
    private void sinkShips() {
        List<Ship> sunkShips = new List<Ship>();
        foreach (Ship ship in gameManager.getShips()) {
            if (ship.getLife() <= 0) {
                sunkShips.Add(ship);
            }
        }
        foreach (Ship ship in sunkShips) {
            ship.getNode().getShips().Remove(ship);
            ship.sink();
            gameManager.getShips().Remove(ship);
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
        foreach (Ship ship in gameManager.getShips()) {
            ship.reset();
        }
    }
}

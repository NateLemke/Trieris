using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {

    public static int PHASES = 4;

    private GameManager gameManager;
    private List<Ship> playerShips = new List<Ship>();
    private List<Ship> aiShips = new List<Ship>();

    public int phaseIndex { get; set; }
    
    public int TurnIndex { get { return turnIndex; } set { } }
    private int turnIndex = 1;

    enum Phases { planning, phaseOne, phaseTwo, phaseThree };
   
    Image[] actionImages = new Image[4];

    private void Awake() {
        phaseIndex = 4;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        actionImages[0] = GameObject.Find("ActionImage1").GetComponent<Image>();
        actionImages[1] = GameObject.Find("ActionImage2").GetComponent<Image>();
        actionImages[2] = GameObject.Find("ActionImage3").GetComponent<Image>();
        actionImages[3] = GameObject.Find("ActionImage4").GetComponent<Image>();
    }

    public void newExecuteTurn() {
        Debug.Log("Begin turn");
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
        if(AnimationManager.actionAnimations.Count != 0) {
            ;
        }
        
        phaseIndex = 0;
        executePhase(phaseIndex);
    }

    public bool executeNextPhase() {

        DebugControl.log("turn","--PHASE " + phaseIndex);
        
        if (phaseIndex >= 3) {
            UIControl.main.devPhaseTrack(4);
            phaseIndex = 4;
            gameManager.checkVictory();
            turnIndex++;
            AnimationManager.updateText();
            foreach (Ship s in gameManager.getPlayerShips()) {
                s.currentActionIndex = 0;
                s.catapultIndex = -1;
                s.actions = new Ship.Action[4];
                s.populateDefaultActions();
            }

            Image image;
            for (int i = 0; i < 4; i++) {
                image = actionImages[i].GetComponent<Image>();
                var tempCol = image.color;
                image.sprite = null;
                tempCol.a = 0;
                image.color = tempCol;
            }

            return false;
        } else {
            phaseIndex++;
            AnimationManager.updateText();
            executePhase(phaseIndex);
            return true;
        }
    }

    private void executePhase(int phase) {
        UIControl.main.devPhaseTrack(phaseIndex);
        DebugControl.log("turn","--PHASE "+phase);
        //UIControl.postNotice("Phase " + (phaseIndex + 1),4f);
        foreach (Ship ship in gameManager.getShips()) {
            if (ship.getCanAct()) {
                try {
                    ship.doAction(phase);
                } catch (ShipCrashedException e) {
                    
                    if (ship.team != gameManager.playerTeam) {
                        int newDirection = 0;
                        newDirection = ship.getAI().setNewShipDirection(ship);
                        ship.setFront(newDirection);
                    }                   
                }
            } else {
                //Debug.Log("ship " + ship + " cannot act");
            }
        }

        handleCollisions();
        handleCatapults();
        
        StartCoroutine( AnimationManager.playAnimations());        
    }

    public void postAnimation() {
        handleCapture();
    }

    private void handleCapture() {
        foreach (Ship ship in gameManager.getShips()) {
            Port port = ship.getNode().getPort();
            int enemyShipNo = 0;
            
            if (port != null && port.getTeam() != ship.team) {
                foreach (Ship s in port.node.getShips())
                {
                    if (s.team != ship.team)
                        enemyShipNo++;
                }
                if (ship.team == gameManager.playerTeam && enemyShipNo == 0)
                {
                    ship.needCaptureChoice = true;
                    port.getGameObject().GetComponent<PortPrompt>().activateNotification(port, ship);
                    //port.transform.gameObject.GetComponent<PortPrompt>().activateNotification();
                    Debug.Log(ship + " needs port capture choice");
                }
                else if(enemyShipNo == 0)
                {
                    // AI port capture
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

                    List<Ship> enemyShips = new List<Ship>();
                    foreach(Ship nodeShip in ship.getNode().getShips()) {
                        if(nodeShip.team != ship.team) {
                            enemyShips.Add(nodeShip);
                        }
                    }
                    List<Ship> potentialCollisions = didCollide(enemyShips,ship);

                    if (potentialCollisions.Count != 0) {
                        Ship chosenShip = null;

                        if (ship.team == gameManager.playerTeam) {

                            ship.needRammingChoice = true;

                            chosenShip = potentialCollisions[0];

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

                List<Ship> potentialTargets = new List<Ship>();

                foreach(Ship s in node.getShips()) {
                    if(s.team != ship.team) {
                        potentialTargets.Add(s);
                    }
                }

                Ship chosenShip = null;
                if(potentialTargets.Count == 0) {
                    continue;
                }
                else if(potentialTargets.Count == 1) {
                    chosenShip = potentialTargets[0];
                } else if (playerShips.Contains(ship)) {
                    if (potentialTargets.Count > 0) {

                        // need player ship catapult choice
                        ship.needCatapultChoice = true;

                        chosenShip = potentialTargets[0];
                    }
                } else {
                    chosenShip = ship.getAI().selectShip(potentialTargets);
                }
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
            ship.sink();
        }
    }

    private void resetShips() {
        foreach (Ship ship in gameManager.getShips()) {
            ship.reset();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Purpose: This class is responsible for calculating the result of actions for each phase
/// </summary>
public class GameLogic : MonoBehaviour {

    public static int PHASES = 4;

    private GameManager gameManager;

    // the number of the current phase, 0-3
    public static int phaseIndex { get; set; }
    
    // the number of the current turn
    public int TurnIndex { get { return turnIndex; } set { } }
    private int turnIndex = 1;

    //enum Phases { planning, phaseOne, phaseTwo, phaseThree };
   
    Image[] actionImages = new Image[4];

    private void Awake() {
        phaseIndex = 4;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        actionImages[0] = GameObject.Find("ActionImage1").GetComponent<Image>();
        actionImages[1] = GameObject.Find("ActionImage2").GetComponent<Image>();
        actionImages[2] = GameObject.Find("ActionImage3").GetComponent<Image>();
        actionImages[3] = GameObject.Find("ActionImage4").GetComponent<Image>();
    }

    /// <summary>
    /// Starts a new turn
    /// </summary>
    public void executeTurn() {
        PhaseManager.EnablePhaseUI();
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

    /// <summary>
    /// Executes the next phase, ends the turn if the phase index is at 3
    /// </summary>
    /// <returns></returns>
    public bool executeNextPhase() {

        DebugControl.log("turn","--PHASE " + phaseIndex);
        
        // if the phase index is 3, then the fourth phase has been completed
        if (phaseIndex >= 3) {
            
            determineGameState();

            gameManager.uiControl.enableControls();

            GameManager.main.uiControl.GoText.text = "START TURN";
            PhaseManager.DisablePhaseUI();
            phaseIndex = 4;
            resetShips();
            turnIndex++;
            foreach(Ship ship in gameManager.getAllShips())
            {
                ship.CanFire = true;
            }
            foreach (Ship s in gameManager.getPlayerShips()) {
                s.currentActionIndex = 0;
                s.catapultIndex = -1;
                s.catapultDirection = -1;
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

            gameManager.uiControl.setSelection(gameManager.getPlayerShips()[0].Id);
            return false;
        } else {
            phaseIndex++;
            PhaseManager.updateText();
            executePhase(phaseIndex);
            return true;
        }
    }
    
    /// <summary>
    /// Executes a phase of a specific index
    /// also runs the phase manager
    /// </summary>
    /// <param name="phase"></param>
    private void executePhase(int phase) {
        DebugControl.log("turn","--PHASE "+phase);
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship.getCanAct()) {
                
                // checks for head on ramming where ships are in adjacent nodes
                if (!checkAdjHRam(ship,phase)) {
                    ship.doAction(phase);
                }
                    
                //ship.doAction(phase);
                if (ship.needRedirect && ship.team != gameManager.playerTeam)
                {
                    int newDirection = 0;
                    newDirection = ship.Ai.setNewShipDirection(ship);
                    ship.hold();
                    ship.setFront(newDirection);
                    ship.setSpriteRotation();

                }

            } 
        }



        handleCollisions();
        handleCatapults();
        handleCapture();

        StartCoroutine( PhaseManager.playPhaseAnimations());        
    }

    /// <summary>
    /// Check for node adjacent head-on rams
    /// </summary>
    /// <param name="ship"></param>
    /// <param name="phase"></param>
    public bool checkAdjHRam(Ship ship, int phase)
    {
        if (ship.actions[phase].GetType().Name == "ForwardAction")
        {
            Node nextNode = ship.getNode().getAdjacentNode(ship.getFront());
            if (nextNode != null)
            {
                foreach (Ship s in nextNode.Ships)
                {
                    if (s.team != ship.team && Mathf.Abs(ship.getFront() - s.getFront()) == 4 && s.actions[phase].GetType().Name == "ForwardAction")
                    {
                        ship.ram(s);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Final check for port capture and ship sinking. Also determines if a win or lose state is reached.
    /// </summary>
    public void postAnimation() {
        
        //sinkShips();
        executeNextPhase();
    }

    /// <summary>
    /// Checks to see if any victory/defeat conditions have been met
    /// </summary>
    private void determineGameState()
    {
        if (GameManager.main.playerTeam.ships.Count == 0)
        {
            gameOver("Defeat");
        }
        else if (GameManager.main.getAllShips().Count == GameManager.main.playerTeam.ships.Count)
        {
            gameOver("Victory");
        }
        else
        {
            foreach(Team t in GameManager.main.teams)
            {
                int portCount = 0;
                bool hasCapital = false;
                foreach (Port port in GameManager.main.Board.getAllPorts())
                {
                    if (port.Team == t)
                    {
                        portCount++;

                        if (port.IsCapital && port.OriginalTeam == t)
                        {
                            hasCapital = true;
                        }
                    }
                }
                if(portCount >= 12)
                {
                    if(t == GameManager.main.playerTeam)
                        gameOver("Victory");
                    else
                        gameOver("Defeat");
                }

            }
        }

    }

    /// <summary>
    /// Ends the game and brings up the game over screen
    /// </summary>
    /// <param name="gamestate">the specific game over state (victory/defeat)</param>
    private void gameOver(string gamestate)
    {
        GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
        gameOverObj.transform.Find("Screen").gameObject.SetActive(true);
        gameOverObj.GetComponent<GameOver>().Initialize(gamestate);
        GameManager.main.gameOver = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Checks each ship if it can capture the node its on.
    /// AI ships use AI to choose.
    /// Adds a pending capture choice for the players ships
    /// </summary>
    private void handleCapture() {
        foreach (Ship ship in gameManager.getAllShips()) {
            Port port = ship.getNode().Port;
            int enemyShipNo = 0;
            
            if (port != null && port.Team != ship.team) {
                foreach (Ship s in port.node.Ships)
                {
                    if (s.team != ship.team)
                        enemyShipNo++;
                }
                if (ship.team == gameManager.playerTeam && enemyShipNo == 0)
                {
                    ship.needCaptureChoice = true;
                    
                    Debug.Log(ship + " needs port capture choice");
                }
                else if(enemyShipNo == 0)
                {
                    // AI port capture
                    if (ship.Ai.decidePortCapture())
                    {
                        ship.capturePort();
                        

                        //ship.canActAfterCollision = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks each ship for instances of ramming collisions
    /// Adds ramming resolutions to the phase manager
    /// Adds ship target resolution if the player has more than one possible target
    /// </summary>
    private void handleCollisions() {
        
        foreach (Ship ship in gameManager.getAllShips()) {

            if (ship.getMoved()) {
                DebugControl.log("position","ship " + ship.Id + " at " +ship.getNode().ToString());
                if (ship.getNode().getNumberOfShips() > 1) {

                    List<Ship> enemyShips = new List<Ship>();
                    foreach(Ship nodeShip in ship.getNode().Ships) {
                        if(nodeShip.team != ship.team) {
                            enemyShips.Add(nodeShip);
                        }
                    }
                    List<Ship> potentialCollisions = didCollide(enemyShips,ship);

                    if (potentialCollisions.Count != 0) {
                        Ship chosenShip = null;
                        if (ship.team == gameManager.playerTeam) {

                            //Debug.Log("potential collision: " + enemyShips[0].team.ToString() + enemyShips[0].getNumeralID());
                            
                            //ship.getNode().activateNotification();
                            //chosenShip = potentialCollisions[0];
                            
                            if(potentialCollisions.Count > 1) {
                                ship.needRammingChoice = true;
                                PhaseManager.rammingTargetResolutions.Add(new ShipTargetResolution(ship,potentialCollisions));
                                PhaseManager.involvedInRamming.Add(ship);
                            } else {
                                chosenShip = potentialCollisions[0];
                            }
                        } else {
                            chosenShip = ship.Ai.selectShip(potentialCollisions);
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

    /// <summary>
    /// Calls updateFrontAfterCollision for each ship
    /// </summary>
    private void updateShips() {
        foreach (Ship ship in gameManager.getAllShips()) {
            ship.updateFrontAfterCollision();
            Debug.Log(ship.name + " " + ship.getCanAct());
        }
    }

    /// <summary>
    /// Checks each ship for a catapult shot
    /// Adds animations to phase manager if either a hit or a miss
    /// Adds pending target resolution if the player's ship has more than one target
    /// </summary>
    private void handleCatapults() {
        foreach (Ship ship in gameManager.getAllShips()) {
            Node node = ship.getCatapultNode();
            if (node != null) {

                List<Ship> potentialTargets = new List<Ship>();

                foreach(Ship potentialTarget in node.Ships) {
                    if(potentialTarget.team != ship.team) {
                        potentialTargets.Add(potentialTarget);
                    }
                }

                Ship chosenShip = null;
                if(potentialTargets.Count == 0) {
                    PhaseManager.addMissedShot(ship,node);
                    continue;
                }
                else if(potentialTargets.Count == 1) {
                    chosenShip = potentialTargets[0];
                } else if (GameManager.main.getPlayerShips().Contains(ship)) {
                    if (potentialTargets.Count > 0) {

                        // need player ship catapult choice
                        ship.needCatapultChoice = true;

                        PhaseManager.catapultTargetResolutions.Add(new ShipTargetResolution(ship,potentialTargets));

                        //chosenShip = potentialTargets[0];
                    }
                } else {
                    chosenShip = ship.Ai.selectShip(potentialTargets);
                }
                ship.catapult(chosenShip);
            }
        }
    }

    /// <summary>
    /// Old method from previous project to sink ships with 0 hp
    /// </summary>
    [System.Obsolete("Should use a subphase in phase manager to sink ships instead",true)]
    private void sinkShips() {
        List<Ship> sunkShips = new List<Ship>();
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship.getLife() <= 0) {
                sunkShips.Add(ship);
                Debug.Log("Sunk ship added");
            }
        }
        foreach (Ship ship in sunkShips) {
            ship.sink();
            Debug.Log("Sinking ship");
        }
    }

    /// <summary>
    /// calls reset for each ship
    /// used at the end of a turn
    /// resets variables like movedForward, canAct
    /// </summary>
    private void resetShips() {
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship == null)
                continue;
            ship.reset();
        }
    }
}

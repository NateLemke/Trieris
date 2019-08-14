using Photon.Pun;
using Photon.Realtime;
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
        foreach (Ship ship in gameManager.getHumanShips()) {
            ship.canActAfterCollision = true;
            if (!(ship.ready())) {
                ship.populateDefaultActions();
                Debug.Log("ship not ready");
            }                
        }        
        phaseIndex = 0;
        executePhase(phaseIndex);
    }
    
    [PunRPC]
    public void setAllTeamsUnready()
    {
        Debug.Log("Ready Reset");
        foreach (Team t in gameManager.teams)
        {
            t.Ready = false;
        }
    }

    /// <summary>
    /// Executes the next phase, ends the turn if the phase index is at 3
    /// </summary>
    /// <returns></returns>
    public bool executeNextPhase() {

        DebugControl.log("turn","--PHASE " + phaseIndex);
        
        // if the phase index is 3, then the fourth phase has been completed
        if (phaseIndex >= 3) {
            PhotonView.Get(this).RPC("setAllTeamsUnready", RpcTarget.All);
            endTurn();

            return false;
        } else {
            phaseIndex++;
            PhaseManager.updateText(GameLogic.phaseIndex);
            executePhase(phaseIndex);
            return true;
        }
    }

    [PunRPC]
    public void endTurn() {
        determineGameState();

        gameManager.uiControl.enableControls();

        
        PhaseManager.DisablePhaseUI();
        phaseIndex = 4;
        resetShips();
        turnIndex++;
        foreach (Ship ship in gameManager.getAllShips()) {
            ship.CanFire = true;
        }

        foreach(Team t in gameManager.getHumanTeams()) {

            t.Ready = false;

            foreach (Ship s in t.ships) {
                s.currentActionIndex = 0;
                s.catapultIndex = -1;
                s.catapultDirection = -1;
                s.actions = new Ship.Action[4];
                s.populateDefaultActions();
            }
        }

        

        Image image;
        for (int i = 0; i < 4; i++) {
            image = actionImages[i].GetComponent<Image>();
            var tempCol = image.color;
            image.sprite = null;
            tempCol.a = 0;
            image.color = tempCol;
        }

        if(GameManager.playerTeam.ships.Count != 0) {
            gameManager.uiControl.setSelection(gameManager.getHumanShips()[0].Id);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView.Get(this).RPC("endTurn", RpcTarget.Others);
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


                //if (ship.needRedirect && ship.team != GameManager.playerTeam)
                if (ship.NeedRedirect && ship.team.TeamType == Team.Type.ai)
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

        executeNextPhase();
    }

    /// <summary>
    /// Checks to see if any victory/defeat conditions have been met
    /// </summary>
    private void determineGameState()
    {   

        foreach (Team t in GameManager.main.teams) {

            if (t.eliminated) {
                continue;
            }

            // check if the only the teams's ships remain
            if (GameManager.main.getAllShips().Count == t.ships.Count) {
                gameOverElimination(t);
                return;
            }

            // check if the player has captured 12 ports or not
            if (t.portsCaptured() >= 12) {
                if (t == GameManager.playerTeam)
                    gameOverCapture(t);
                return;
            }            
        }

        foreach(Team t in gameManager.teams) {
            // check if the player has lost all their ships
            if (t.ships.Count == 0) {
                eliminatePlayer(t);
            }
        }
    }

    private void eliminatePlayer(Team t) {
        t.eliminated = true;
        if(t == GameManager.playerTeam) {
            GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
            gameOverObj.transform.Find("EliminationScreen").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Ends the game and brings up the game over screen
    /// </summary>
    /// <param name="gamestate">the specific game over state (victory/defeat)</param>
    private void gameOverElimination(Team t)
    {
        GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
        gameOverObj.transform.Find("GameoverScreen").gameObject.SetActive(true);
        gameOverObj.GetComponent<GameOver>().gameOverElimination(t);
        GameManager.main.gameOver = true;
        Time.timeScale = 0;
    }

    private void gameOverCapture(Team t) {
        GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
        gameOverObj.transform.Find("GameoverScreen").gameObject.SetActive(true);
        gameOverObj.GetComponent<GameOver>().gameOverCapture(t);
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
                if (!ship.belongsToAI() && enemyShipNo == 0)
                {
                    ship.NeedCaptureChoice = true;
                    
                    //Debug.Log(ship + " needs port capture choice");
                }
                else if(enemyShipNo == 0)
                {
                    // AI port capture
                    if (ship.Ai.decidePortCapture())
                    {
                        ship.capturePort();
                        
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
                        if (!ship.belongsToAI()) {
                            
                            if(potentialCollisions.Count > 1) {
                                Debug.Log("Collisions: ");
                                foreach(Ship s in potentialCollisions){
                                    Debug.Log(s.team.TeamFaction + " " + s.Id);
                                }
                                Debug.Log("Collison List End");
                                ship.NeedRammingChoice = true;
                                PhaseManager.rammingTargetResolutions.Add(new ShipTargetResolution(ship,potentialCollisions));
                                PhaseManager.involvedInRamming.Add(ship);
                            } else {
                                chosenShip = potentialCollisions[0];
                            }
                        } else {
                            chosenShip = ship.Ai.selectShip(potentialCollisions);
                        }
                        Debug.LogFormat("Ship from team {0} rammed ship from team {1}",ship.team.TeamFaction,chosenShip.team.TeamFaction);
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
                } else if (!ship.belongsToAI()) {
                    if (potentialTargets.Count > 0) {

                        // need player ship catapult choice
                        ship.NeedCatapultChoice = true;

                        PhaseManager.catapultTargetResolutions.Add(new ShipTargetResolution(ship,potentialTargets));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// was called Controller 
public class GameLogic : MonoBehaviour {

    public static int PHASES = 4;

    private GameManager gameManager;
    //private List<Ship> playerShips = new List<Ship>();
    //private List<Ship> aiShips = new List<Ship>();

    public static int phaseIndex { get; set; }
    
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
        //Debug.Log("Begin turn");
        //DebugControl.log("turn","BEGIN TURN");
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

    public bool executeNextPhase() {

        DebugControl.log("turn","--PHASE " + phaseIndex);
        
        if (phaseIndex >= 3) {

            gameManager.uiControl.enableControls();

            GameManager.main.uiControl.GoText.text = "START TURN";
            //UIControl.main.devPhaseTrack(4);
            PhaseManager.DisablePhaseUI();
            phaseIndex = 4;
            resetShips();
            //gameManager.checkVictory();
            turnIndex++;
            //PhaseManager.updateText();
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

    public int executed = 0;

    private void executePhase(int phase) {
        //UIControl.main.devPhaseTrack(phaseIndex);
        DebugControl.log("turn","--PHASE "+phase);
        //UIControl.postNotice("Phase " + (phaseIndex + 1),4f);
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship.getCanAct()) {
                if (!checkAdjHRam(ship,phase)) {
                    ship.doAction(phase);
                    executed++;
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

                /*
                try {
                    ship.doAction(phase);
                } catch (ShipCrashedException e) {
                    if (ship.team != gameManager.playerTeam) {
                        int newDirection = 0;
                        newDirection = ship.getAI().setNewShipDirection(ship);
                        ship.setFront(newDirection);
                        ship.setSpriteRotation();
                    }                   
                }
                */
            } else {
                //Debug.Log("ship " + ship + " cannot act");
            }
        }

        ;

        executed = 0;

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
                foreach (Ship s in nextNode.getShips())
                {
                    if (Mathf.Abs(ship.getFront() - s.getFront()) == 4 && s.actions[phase].GetType().Name == "ForwardAction")
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
        
        sinkShips();
        determineGameState();
        executeNextPhase();
    }

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
                foreach (Port port in GameManager.main.getBoard().getAllPorts())
                {
                    if (port.Team == t)
                    {
                        portCount++;
                        Debug.Log(port.node.getX() + ", " + port.node.getY());
                        Debug.Log(" This is " + port.OriginalTeam.getTeamType().ToString());

                        if (port.IsCapital && port.OriginalTeam == t)
                        {
                            Debug.Log(t.getTeamType().ToString() + " has their capital");
                            hasCapital = true;
                        }
                    }
                }
                if (!hasCapital)
                {
                    foreach (Ship s in t.ships)
                        s.life = 0;
                    break;
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

    private void gameOver(string gamestate)
    {
        GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
        gameOverObj.transform.Find("Screen").gameObject.SetActive(true);
        gameOverObj.GetComponent<GameOver>().Initialize(gamestate);
        GameManager.main.gameOver = true;
        Time.timeScale = 0;
    }

    private void handleCapture() {
        foreach (Ship ship in gameManager.getAllShips()) {
            Port port = ship.getNode().getPort();
            int enemyShipNo = 0;
            
            if (port != null && port.Team != ship.team) {
                foreach (Ship s in port.node.getShips())
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

    // First checks if the ship in question has moved, the checks if node ship
    // is at has more than 1 ship,
    // then compares current ship to others to see if they collide and calls
    // collision handler from the
    // Event Handler class, where I will either pass in the node of the
    // collision or the ship list at that node
    private void handleCollisions() {
        
        foreach (Ship ship in gameManager.getAllShips()) {

            if (ship.getMoved()) {
                DebugControl.log("position","ship " + ship.Id + " at " +ship.getNode().ToString());
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

    private void updateShips() {
        foreach (Ship ship in gameManager.getAllShips()) {
            ship.updateFrontAfterCollision();
            Debug.Log(ship.name + " " + ship.getCanAct());
        }
    }

    // Detects catapults by checking each ships hasFired()
    private void handleCatapults() {
        foreach (Ship ship in gameManager.getAllShips()) {
            Node node = ship.getCatapultNode();
            if (node != null) {

                List<Ship> potentialTargets = new List<Ship>();

                foreach(Ship potentialTarget in node.getShips()) {
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

    // 'Sinks' ships by removing them from the lists of ships
    private void sinkShips() {
        List<Ship> sunkShips = new List<Ship>();
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship.getLife() <= 0) {
                sunkShips.Add(ship);
                Debug.Log("Sunk ship added");
            }
        }
        foreach (Ship ship in sunkShips) {
            gameManager.uiControl.setDead((int) ship.team.getTeamType(), ship.Id);
            ship.sink();
            Debug.Log("Sinking ship");
        }
    }

    private void resetShips() {
        foreach (Ship ship in gameManager.getAllShips()) {
            if (ship == null)
                continue;
            ship.reset();
        }
    }
}

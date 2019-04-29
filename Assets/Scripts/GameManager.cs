using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private Board board;
    public GameLogic gameLogic;
    private PlayerPlanningBoard planBoard;
    private List<Ship> ships = new List<Ship>();
    private TrierisUIInterface trierisUI;
    //public List<Ship> playerShips;
    //List<Ship> allAIShips = new List<Ship>();
    private List<TrierisAI> aiList;
    //List<Ship> aiShips = new List<Ship>();
    private bool gameOver = false;

    // for moving and flipping the board
    private int boardOffsetX;
    private int boardOffsetY;
    private bool fipBoard;

    // other new variables
    //public bool playingAnimation { get; set; }
    public bool processingTurn { get; set; }
    public bool animationPlaying = false;
    public bool needRedirect = false;
    public bool needCaptureChoice = false;
    public bool shipCrashed;
    public static GameManager main;
    public List<Team> teams = new List<Team>();
    LineRenderer lineRenderer;
    public Team playerTeam;

    //Locks camera movement and zoom
    public bool cameraLock;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        main = this;
        board = new Board();

        // draw nodes and lines on board
        board.CreateGridVisuals();

        // spawn teams, ships and ports
        foreach (Team.Type teamType in (Team.Type[])Enum.GetValues(typeof(Team.Type))) {
            teams.Add(new Team(teamType));
        }

        // set player's team to 0 by default
        playerTeam = teams[0];
    }

    // SHOULDNT HAVE TO USE THIS CONSTRUCTOR
    // having constructors for a monobehavior object seems to mess things up
    //public GameManager() {
    //    board = new Board();
    //    ships = board.getAllShips();
    //}

    private void Start() {        

        // set player ships and board
        //TeamColor playerColor = trierisUI.promptPlayerTeam();
        
        //planBoard = new PlayerPlanningBoard(playerShips);
                        
        // set ai ships
        // we make an AI for each team, and simply skip the AI's control for the player's team
        // this will allow us to change the player's team duing the game which will be useful for debugging and testing
        aiList = new List<TrierisAI>();
        for (int i = 0; i < teams.Count; i++) {
            aiList.Add(new TrierisAI(teams[i]));
        }

        foreach (TrierisAI ai in aiList) {
            foreach (Ship ship in ai.getShips()) {
                int direction = ai.setNewShipDirection(ship);
                ship.setFront(direction);
                ship.needRedirect = false;
                //Debug.Log("hello!");
                ship.setSpriteRotation();
            }
        }


        // set GameLogic
        gameLogic = GetComponent<GameLogic>();

        //Locks camera for Team Select
        cameraLock = true;
    }

    private void Update() {

        foreach (Ship ship in ships) {
            if (needRedirect = ship.needRedirect) {
                //Debug.Log(ship+ "needs redirect")
                break;
            }
        }

        foreach (Ship ship in ships) {
            if(needCaptureChoice = ship.needCaptureChoice) {
                break;
            }
        }

        if (processingTurn) {
            animationPlaying = AnimationManager.playingAnimation;
            //needRedirect = false;
            
            if (!animationPlaying && !needRedirect && !needCaptureChoice) {
                processingTurn = gameLogic.executePhase();
            }
        }
    }


    public void setUI(TrierisUIInterface ui) {
        this.trierisUI = ui;
    }

    public List<Ship> getPlayerShips() {
        int teamIndex = (int)playerTeam.getTeamType();
        return teams[teamIndex].ships;
    }

    public List<Ship> getAllAiShips() {
        List<Ship> r = new List<Ship>();
        for(int i = 0; i < teams.Count; i++) {
            if(i != (int)playerTeam.getTeamType()) {
                r.AddRange(teams[i].ships);
            }
        }
        return r;
    }

    public List<TrierisAI> getAllAi() {
        List<TrierisAI> r = new List<TrierisAI>();
        for (int i = 0; i < teams.Count; i++) {
            if (i != (int)playerTeam.getTeamType()) {
                foreach(Ship s in teams[i].ships) {
                    r.Add(s.getAI());
                }
            }
        }
        return r;
    }

    public TrierisUIInterface getUI() {
        return trierisUI;
    }


    public Board getBoard() {
        return board;
    }

    public void setShipAction(int shipIndex,int actionIndex,int actionType,int catapultDirection) {         // throws CannotReverseException, InvalidActionException, InvalidActionIndexException
        planBoard.setShipAction(shipIndex, actionIndex, actionType, catapultDirection);
    }

    public bool executeTurn() {
        //if (!gameOver && gameLogic.executeTurn()) {
        //    trierisUI.updateMapDisplay();
        //    return true;
        //}
        //return false;
        gameLogic.newExecuteTurn();
        return true;
    }

    public void setAIActions() {
        if (!gameOver) {
            foreach (TrierisAI ai in aiList) {
                if(ai.GetTeam() == playerTeam) {
                    Debug.Log("found a ship");
                    continue;
                }
                ai.setNextTurn();
                //try {
                //    ai.setNextTurn();
                //} catch (InvalidActionIndexException | CannotReverseException | InvalidActionException e) {
                //e.printStackTrace();
            }
        }    
    }

    public Ship promptPlayerShips(string message,List<Ship> shipChoices) {
        if (shipChoices.Count == 1) {
            return shipChoices[0];
        }
        Ship result = trierisUI.promptPlayerShips(message,shipChoices);
        return result == null ? shipChoices[0] : result;
    }

    public int promptPlayerDirection(string message) {
        return trierisUI.promptPlayerDirection(message);
    }

    public bool promptPlayerCapture(string message) {
        //return trierisUI.promptPlayerCapture(message);
        Debug.Log(playerTeam.getTeamType().ToString() + " player has captured a port!");
        return false;
    }

    public void checkVictory() {
        Dictionary<string,int> distribution = new Dictionary<string,int>();
        foreach (Team t in teams) {
            distribution.Add(t.ToString(),0);
        }
        foreach (Node portNode in board.getAllPortNodes()) {
            if (portNode.getPort() != null) {
                Team t = portNode.getPort().getTeam();
                int currentValue = distribution[t.ToString()];
                distribution[t.ToString()] = currentValue + 1;
                if (currentValue + 1 >= 12) {
                    victory(t);
                }
            }
        }
    }

    private void victory(Team t) {
        //trierisUI.victory(color);
        this.gameOver = true;
    }

    public void promptInitialFace() {
        foreach(Ship ship in getPlayerShips()) {
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

    // N E W   S T U F F

    public Ship spawnShip(Node node,Team team) {
        GameObject parent = GameObject.Find("Ships");
        if (parent == null) {
            parent = Instantiate(new GameObject());
            parent.name = "Ships";
        }
        GameObject shipPrefab = Resources.Load("Ship") as GameObject;
        GameObject spawn = Instantiate(shipPrefab,node.getPosition(),Quaternion.identity);
        spawn.transform.parent = parent.transform;
        Ship ship = spawn.GetComponent<Ship>();
        //Debug.Log("can act: "+ship.getCanAct());
        ships.Add(ship);
        ship.intialize(team,node);
        ship.name = team.getTeamType().ToString() + " ship " + ship.getID();

        return ship;
    }

    public List<Ship> getShips() {
        return ships;
    }

    public Team getTeam (Team.Type t) {
        return teams[(int)t];
    }

    public Team getTeam(int i) {
        if(i < 0 || i >= teams.Count) {
            Debug.LogError("getTeam argument out of range");
            return null;
        }
        return teams[i];
    }

    public void setPlayerTeam(Team.Type t) {
        playerTeam = getTeam(t);
    }

    private void OnDrawGizmos() {

        // draw nodes and node connections
        //drawBoardGizmos();
    }

    private void drawBoardGizmos() {
        if (board != null) {
            Gizmos.color = Node.GIZMO_COLOR;
            for (int x = 0; x < Board.ROW_OF_NODES; x++) {
                for (int y = 0; y < Board.COLUMN_OF_NODES; y++) {
                    Node n = board.getNodeAt(x,y);
                    if (!n.isIsland()) {
                        Vector3 pos = new Vector3(n.getY(),Board.ROW_OF_NODES - 1 - n.getX(),0);

                        Gizmos.DrawSphere(pos,Node.GIZMO_SIZE);

                        Node[] adjacents = n.getAdjacentNodes();
                        for (int i = 0; i < adjacents.Length; i++) {
                            if (adjacents[i] != null) {
                                Vector3 adjPos = new Vector3(adjacents[i].getY(),Board.ROW_OF_NODES - 1 - adjacents[i].getX(),0);
                                Gizmos.DrawLine(pos,adjPos);
                            }
                        }
                    }
                }
            }
        }
    }

    public delegate IEnumerator coroutineDel();

    public void startCoroutine(coroutineDel d) {
        d();
    }
}

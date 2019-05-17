using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    private Board board;
    public GameLogic gameLogic;
   // private List<Ship> ships = new List<Ship>();
    public UIControl uiControl;
    private List<TrierisAI> aiList;
    public bool gameOver = false;

    // for moving and flipping the way the board is rendered offscreen
    private int boardOffsetX;
    private int boardOffsetY;
    private bool fipBoard;

    public bool processingTurn { get; set; }
    public bool animationPlaying = false;
    public bool needRedirect = false;
    public bool needCaptureChoice = false;
    public bool needCatapultChoice = false;
    public bool needRammingChoice = false;
    public bool shipCrashed;
    public static GameManager main;
    public List<Team> teams = new List<Team>();
    LineRenderer lineRenderer;
    public Team playerTeam;

    public bool cameraLock;
    
    public static int PortsCaptured { get; set; }

    /// <summary>
    /// Sets the static reference to the main gamemanager
    /// creates the game board
    /// creates game board visuals
    /// </summary>
    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        main = this;
        board = new Board();
        board.CreateGridVisuals();       

        cameraLock = true;
    }

    /// <summary>
    /// Creates teams, ports, and ships
    /// Assigns additional references
    /// </summary>
    private void Start() {
        createTeams();
        createPorts();
        createShips();
        createAIs();
        gameLogic = GetComponent<GameLogic>();
        uiControl = GetComponent<UIControl>();

        //Debug.Log(Random.Range(-0.5f,0.5f));
        //Debug.Log(Random.Range(-0.5f,0.5f));
        //Debug.Log(Random.Range(-0.5f,0.5f));
        //Debug.Log(Random.Range(-0.5f,0.5f));
        //Debug.Log(Random.Range(-0.5f,0.5f));
    }

    /// <summary>
    /// Checks every frame if there are pending redirects or port captures
    /// </summary>
    private void Update() {        

        checkForChoices();
        //checkForExecuteNextPhase();
        PhaseManager.drawFocusMargin();
    }

    /// <summary>
    /// Checks if any ships currently need to be redirected
    /// </summary>
    public void checkForRedirects() {
        foreach (Ship ship in getAllShips()) {
            if (needRedirect = ship.needRedirect) {
                break;

            }
        }
    }

    /// <summary>
    /// Checks if any ships need to make a port capture choice
    /// </summary>
    public void checkForCaptureChoice() {
        foreach (Ship ship in getAllShips()) {
            if (needCaptureChoice = ship.needCaptureChoice) {
                break;
            }
        }
    }

    /// <summary>
    /// Checks for pending redirects or port captures
    /// </summary>
    public void checkForChoices() {
        checkForRedirects();
        checkForCaptureChoice();
    }

    /// <summary>
    /// Checks if the game should execute the next phase
    /// </summary>
    public void checkForExecuteNextPhase() {
        if (processingTurn) {
            if (!needRedirect && !needCaptureChoice) {
                processingTurn = gameLogic.executeNextPhase();
            }
        }
    }

    /// <summary>
    /// returns a reference to every ships that's still alive
    /// </summary>
    /// <returns></returns>
    public List<Ship> getAllShips() {

        List<Ship> r = new List<Ship>(); 

        foreach(Team t in teams) {
            r.AddRange(t.ships);
        }

        return r;
    }

    /// <summary>
    /// returns a reference to the list of ships on the player's team
    /// </summary>
    /// <returns></returns>
    public List<Ship> getPlayerShips() {
        return playerTeam.ships;
    }

    /// <summary>
    /// Returns a list of every ship on an AI team
    /// </summary>
    /// <returns></returns>
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
        return aiList;
    }

    public Board getBoard() {
        return board;
    }

    /// <summary>
    /// Executes the next turn
    /// </summary>
    /// <returns></returns>
    public bool executeTurn() { 
        gameLogic.newExecuteTurn();
        return true;
    }

    /// <summary>
    /// Sets actions for all AI ships
    /// </summary>
    public void setAIActions() {
        if (!gameOver) {
            foreach (TrierisAI ai in getAllAi()) {
                if(ai.GetTeam() == playerTeam) {
                    Debug.Log("found a ship");
                    continue;
                }
                ai.setNextTurn();
            }
        }    
    }

    //public bool promptPlayerCapture(string message) {
    //    //return trierisUI.promptPlayerCapture(message);
    //    Debug.Log(playerTeam.getTeamType().ToString() + " player has captured a port!");
    //    return false;
    //}

    ///// <summary>
    ///// Checks to see if the game has reached a game over condition
    ///// </summary>
    //public void checkVictory() {
    //    Dictionary<string,int> distribution = new Dictionary<string,int>();
    //    foreach (Team t in teams) {
    //        distribution.Add(t.ToString(),0);
    //    }
    //    foreach (Node portNode in board.getAllPortNodes()) {
    //        if (portNode.getPort() != null) {
    //            Team t = portNode.getPort().Team;
    //            int currentValue = distribution[t.ToString()];
    //            distribution[t.ToString()] = currentValue + 1;
    //            if (currentValue + 1 >= 12) {
    //                victory(t);
    //            }
    //        }
    //    }
    //}

    
    //private void victory(Team t) {
    //    this.gameOver = true;
    //}

    /// <summary>
    /// Instantiates a new ship into the game world for the given team on the given node
    /// </summary>
    /// <param name="node">the node to place the ship</param>
    /// <param name="team">the team the ship is assigned to</param>
    /// <returns>A reference to the newly created ship</returns>
    public Ship spawnShip(Node node,Team team) {
        GameObject parent = GameObject.Find("Ships");
        if (parent == null) {
            parent = Instantiate(new GameObject());
            parent.name = "Ships";
        }
        GameObject shipPrefab = Resources.Load("Prefabs/Ship") as GameObject;
        GameObject spawn = Instantiate(shipPrefab,node.getBoardPosition(),Quaternion.identity);
        spawn.transform.parent = parent.transform;
        Ship ship = spawn.GetComponent<Ship>();
        //Debug.Log("can act: "+ship.getCanActa());
        //ships.Add(ship);
        ship.intialize(team,node);
        ship.name = team.getTeamType().ToString() + " ship " + ship.Id;

        return ship;
    }

    /// <summary>
    /// Returns a team based on a team enum type
    /// </summary>
    /// <param name="t">the team type</param>
    /// <returns>A reference to a team</returns>
    public Team getTeam (Team.Type t) {
        return teams[(int)t];
    }

    /// <summary>
    /// Returns a team based on an integer team index
    /// </summary>
    /// <param name="i">the team index</param>
    /// <returns>A referemce to a team</returns>
    public Team getTeam(int i) {
        if(i < 0 || i >= teams.Count) {
            Debug.LogError("getTeam argument out of range");
            return null;
        }
        return teams[i];
    }
    

    private void OnDrawGizmos() {

        // draw nodes and node connections
        //drawBoardGizmos();

        //if(board != null) {
        //    Gizmos.color = Color.red;
        //    foreach (Node n in board.getAllNodes()) {
        //        Handles.Label(n.getRealPos(),n.getPosition().ToString());
        //        for (int i = 0; i < 8; i++) {
        //            if (n.getAdjacentNode(i) != null) {
        //                Vector2 halfway = (n.getAdjacentNode(i).getRealPos() - n.getRealPos()) / 2;
        //                Gizmos.DrawLine(n.getRealPos(),n.getRealPos() + halfway);
        //            }
        //        }
        //    }
        //}

        
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

    /// <summary>
    /// Sets the redirect UI to active for all ships that need a redirect choice made
    /// </summary>
    public void revealRedirects() {
        foreach(Ship s in getPlayerShips()) {
            s.setRedirectUI(true);
        }
    }

    /// <summary>
    /// To be used once at the beginning of the game.
    /// Sets the player's team.
    /// Sets directions for all other team's AI ships
    /// </summary>
    /// <param name="i"></param>
    public void setPlayerTeam(int i) {
        playerTeam = teams[i];        
        foreach (Ship ship in playerTeam.ships) {
            ship.needRedirect = true;
        }
        revealRedirects();
        setAIDirections();
        cameraLock = false;
        GameObject.Find("TeamIcon").GetComponent<Image>().sprite = playerTeam.getPortSprite();

    }

    /// <summary>
    /// Sets the initial direction for each AI ship
    /// </summary>
    void setAIDirections() {
        foreach (TrierisAI ai in getAllAi()) {
            if (ai.GetTeam() == playerTeam) {
                continue;                
            }
            foreach (Ship ship in ai.GetTeam().ships) {

                int direction = ship.Ai.setNewShipDirection(ship);
                ship.setFront(direction);
                ship.needRedirect = false;
                ship.setSpriteRotation();
            }
        }
    }

    /// <summary>
    /// Creates the instances of each team type
    /// </summary>
    public void createTeams() {
        foreach (Team.Type teamType in (Team.Type[])Enum.GetValues(typeof(Team.Type))) {
            teams.Add(new Team(teamType));
        }
    }

    /// <summary>
    /// Creates each 5 ports for each team
    /// </summary>
    public void createPorts() {
        foreach(Team t in teams) {
            t.createPorts();
        }
    }

    /// <summary>
    /// Creates a ship on each port for each team
    /// </summary>
    public void createShips() {
        foreach (Team t in teams) {
            t.createShips();
        }
    }

    /// <summary>
    /// Create an AI for every team
    /// (player team ignores their AI)
    /// </summary>
    void createAIs() {
        aiList = new List<TrierisAI>();
        for (int i = 0; i < teams.Count; i++) {
            aiList.Add(new TrierisAI(teams[i]));
        }
    }    
}

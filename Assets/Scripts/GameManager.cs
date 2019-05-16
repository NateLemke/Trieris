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
    private bool gameOver = false;

    // for moving and flipping the board
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

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        main = this;
        board = new Board();
        board.CreateGridVisuals();       

        cameraLock = true;
    }

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

    private void Update() {        

        checkForChoices();
        checkForExecuteNextPhase();
        PhaseManager.drawFocusMargin();
    }

    public void checkForRedirects() {
        foreach (Ship ship in getAllShips()) {
            if (needRedirect = ship.needRedirect) {
                break;

            }
        }
    }

    public void checkForCaptureChoice() {
        foreach (Ship ship in getAllShips()) {
            if (needCaptureChoice = ship.needCaptureChoice) {
                break;
            }
        }
    }

    public void checkForChoices() {
        checkForRedirects();
        checkForCaptureChoice();
    }

    public void checkForExecuteNextPhase() {
        if (processingTurn) {
            if (!needRedirect && !needCaptureChoice) {
                processingTurn = gameLogic.executeNextPhase();
            }
        }
    }

    public List<Ship> getAllShips() {

        List<Ship> r = new List<Ship>(); 

        foreach(Team t in teams) {
            r.AddRange(t.ships);

        }

        return r;
    }

    public List<Ship> getPlayerShips() {
        return playerTeam.ships;
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
        return aiList;
    }

    public Board getBoard() {
        return board;
    }

    public bool executeTurn() { 
        gameLogic.newExecuteTurn();
        return true;
    }

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
                Team t = portNode.getPort().Team;
                int currentValue = distribution[t.ToString()];
                distribution[t.ToString()] = currentValue + 1;
                if (currentValue + 1 >= 12) {
                    victory(t);
                }
            }
        }
    }

    private void victory(Team t) {
        this.gameOver = true;
    }

    public Ship spawnShip(Node node,Team team) {
        GameObject parent = GameObject.Find("Ships");
        if (parent == null) {
            parent = Instantiate(new GameObject());
            parent.name = "Ships";
        }
        GameObject shipPrefab = Resources.Load("Prefabs/Ship") as GameObject;
        GameObject spawn = Instantiate(shipPrefab,node.getPosition(),Quaternion.identity);
        spawn.transform.parent = parent.transform;
        Ship ship = spawn.GetComponent<Ship>();
        //Debug.Log("can act: "+ship.getCanActa());
        //ships.Add(ship);
        ship.intialize(team,node);
        ship.name = team.getTeamType().ToString() + " ship " + ship.Id;

        return ship;
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

    public void revealRedirects() {
        foreach(Ship s in getPlayerShips()) {
            s.setRedirectUI(true);
        }
    }

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

    public void createTeams() {
        foreach (Team.Type teamType in (Team.Type[])Enum.GetValues(typeof(Team.Type))) {
            teams.Add(new Team(teamType));
        }
    }

    public void createPorts() {
        foreach(Team t in teams) {
            t.createPorts();
        }
    }

    public void createShips() {
        foreach (Team t in teams) {
            t.createShips();
        }
    }

    void createAIs() {
        aiList = new List<TrierisAI>();
        for (int i = 0; i < teams.Count; i++) {
            aiList.Add(new TrierisAI(teams[i]));
        }
    }

    
}

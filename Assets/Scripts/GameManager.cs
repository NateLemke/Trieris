using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public Board Board { get { return board; } }
    private Board board;
    public GameLogic gameLogic;
    // private List<Ship> ships = new List<Ship>();
    public UIControl uiControl;
    private List<TrierisAI> aiList = new List<TrierisAI>();
    public bool gameOver = false;

    // for moving and flipping the way the board is rendered offscreen
    private int boardOffsetX;
    private int boardOffsetY;
    private bool flipBoard;

    public bool processingTurn { get; set; }
    //public bool animationPlaying = false;
    //public bool needRedirect = false;
    //public bool needCaptureChoice = false;
    //public bool needCatapultChoice = false;
    //public bool needRammingChoice = false;
    //public bool shipCrashed;
    public static GameManager main;
    public Team[] teams = new Team[6];
    LineRenderer lineRenderer;
    //public Team playerTeam;

    public bool cameraLock;

    // needs to be changed for multiplayer
    //public static int PortsCaptured { get; set; }

    // new multiplayer data
    public static Team playerTeam;
    public static Team.Faction playerFaction;
    public static Team.Type[] teamTypes = new Team.Type[6];

    /// <summary>
    /// Creates teams, ports, and ships
    /// Assigns additional references
    /// </summary>
    private void Start() {
        GetComponent<UIControl>().SetUpUI();

        //createAIs();
        gameLogic = GetComponent<GameLogic>();
        uiControl = GetComponent<UIControl>();
        Time.timeScale = 1;

        if (PhotonNetwork.IsConnected) {
            GameObject.Find("OverlayCanvas/TeamSelectPanel").gameObject.SetActive(false);
            //playerFaction = (Team.Faction)PhotonNetwork.LocalPlayer.CustomProperties["TeamInt"];
            //teamTypes[(int)PhotonNetwork.LocalPlayer.CustomProperties["TeamInt"]] = (Team.Type)1;
            //uiControl.setTeam((int)PhotonNetwork.LocalPlayer.CustomProperties["TeamInt"]);

            if (PhotonNetwork.IsMasterClient) {
                Debug.Log("Im the master client!");

            } else {
                Debug.Log("Im NOT the master client");
            }

            setupGame((int)PhotonNetwork.LocalPlayer.CustomProperties["TeamInt"]);
        }
    }

    public void setupGame(int playerChoice) {

        //if (!PhotonNetwork.IsConnected) {
        //    for (int i = 0; i < 6; i++) {
        //        if (i == playerChoice) {
        //            teamTypes[i] = Team.Type.player;
        //        } else {
        //            teamTypes[i] = Team.Type.ai;
        //        }
        //    }
        //}

        // TEMPORARY
        for (int i = 0; i < 6; i++) {
            if (i == playerChoice) {
                teamTypes[i] = Team.Type.player;
            } else {
                teamTypes[i] = Team.Type.ai;
            }
        }
               
        createTeams();

        playerFaction = (Team.Faction)playerChoice;
        playerTeam = teams[(int)playerFaction];

        //if (PhotonNetwork.IsConnected)
        //    PhotonView.Get(this).RPC("teamIsHuman",RpcTarget.All,playerChoice);
        if (playerTeam == null) {
            Debug.LogError("Player's team is null");
        }

        createPorts();


        if (playerTeam == null) {
            Debug.LogError("Player's team is null");
        }

        createShips();

        if (!PhotonNetwork.IsConnected || (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)) {
            

            assignAI();

            setAIDirections();

            //if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
            //    FindShips();
            //}

            uiControl.PostTeamSelection();
        }



        //if (PhotonNetwork.IsConnected) {
        //    PhotonView photonView = PhotonView.Get(this);
        //    photonView.RPC("CopyTeams",RpcTarget.All,teams);
        //}

        
        
        //uiControl.setTeam((int)PhotonNetwork.LocalPlayer.CustomProperties["TeamInt"]);
        promptInitialRedirets();
        revealRedirects();


        cameraLock = false;
        GameObject.Find("TeamIcon").GetComponent<Image>().sprite = playerTeam.getPortSprite();
        
    }

    /// <summary>
    /// Checks every frame if there are pending redirects or port captures
    /// </summary>
    private void Update() {

        //checkForChoices();
        //checkForExecuteNextPhase();
        //PhaseManager.drawFocusMargin();
    }

    // new multiplayer functions
    public bool playersReady() {
        foreach (Team t in teams) {
            if (!t.aiTeam && !t.Ready) {
                return false;
            }
        }
        return true;
    }

    public void setPlayerReady(Team t,bool ready) {
        t.Ready = ready;
    }

    public void beginTurn() {
        if (GameLogic.phaseIndex != 4 || !playersReady()) {
            return;
        }

        UIControl.main.disableControls();
        UIControl.main.setShipAttacks();

        gameLogic.executeTurn();
    }


    public bool needRammingChoice() {
        foreach (Team t in teams) {
            if(t == null) {
                continue;
            }

            if (!t.aiTeam && t.needRammingChoice()) {
                return true;
            }
        }
        return false;
    }

    public bool needCatapultChoice() {
        foreach (Team t in teams) {
            if (t == null) {
                continue;
            }

            if (!t.aiTeam && t.needCatapultChoice()) {
                return true;
            }
        }
        return false;
    }

    //[PunRPC]
    //public void teamIsHuman(int i)
    //{
    //    teams[i].setTeamType((Team.Type) 1);
    //}

    public void promptInitialRedirets() {
        foreach(Team t in teams) {
            if (!t.aiTeam) {
                foreach(Ship s in t.ships) {
                    s.needRedirect = true;
                }
            }
        }
    }


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
    /// Checks if any ships currently need to be redirected
    /// </summary>
    public bool needRedirect() {
        foreach (Team t in teams) {
            if (t == null) {
                continue;
            }

            if (t.aiTeam == false && t.needRedirectChoice()) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if any ships need to make a port capture choice
    /// </summary>
    public bool needCaptureChoice() {


        foreach (Team t in teams) {
            if (t == null) {
                continue;
            }

            if (t.aiTeam == false && t.needCaptureChoice()) {
                return true;
            }
        }
        return false;
    }



    /// <summary>
    /// Checks for pending redirects or port captures
    /// </summary>
    //public void checkForChoices() {
    //    checkForRedirects();
    //    checkForCaptureChoice();
    //}

    /// <summary>
    /// Checks if the game should execute the next phase
    /// </summary>
    public void checkForExecuteNextPhase() {
        if (processingTurn) {
            if (!needRedirect() && !needCaptureChoice()) {
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

    public List<Team> getHumanTeams() {
        List<Team> teams = new List<Team>();
        foreach(Team t in this.teams) {
            if(t != null && !t.aiTeam) {
                teams.Add(t);
            }
        }
        return teams;
    }

    public List<Ship> getHumanShips() {
        List<Ship> ships = new List<Ship>();
        foreach(Team t in getHumanTeams()) {
            ships.AddRange(t.ships);
        }
        return ships;
    }

    public List<Ship> getPlayerShips() {
        return playerTeam.ships;
    }

    /// <summary>
    /// Returns a list of every ship on an AI team
    /// </summary>
    /// <returns></returns>
    public List<Ship> getAllAiShips() {
        List<Ship> r = new List<Ship>();
        for(int i = 0; i < teams.Length; i++) {
            if(i != (int)playerTeam.TeamFaction) {
                r.AddRange(teams[i].ships);
            }
        }
        return r;
    }

    public List<TrierisAI> getAllAi() {
        return aiList;
    }

    /// <summary>
    /// Executes the next turn
    /// </summary>
    /// <returns></returns>
    public bool executeTurn() { 
        gameLogic.executeTurn();
        return true;
    }

    /// <summary>
    /// Sets actions for all AI ships
    /// </summary>
    public void setAIActions() {
        if (!gameOver) {
            foreach (TrierisAI ai in getAllAi()) {
                if(ai.GetTeam() == playerTeam) {
                    continue;
                }
                ai.setNextTurn();
            }
        }    
    }

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

        GameObject spawn = Instantiate(shipPrefab,node.getRealPos(),Quaternion.identity);

        Ship ship = spawn.GetComponent<Ship>();

        ship.intialize(team,node);
        ship.name = team.TeamFaction.ToString() + " ship " + ship.Id;

        PhotonView pv = GetComponent<PhotonView>();
        pv.ViewID = (int)(team.TeamFaction+1) * 100 + (ship.Id+1) * 10;

        Debug.Log((int)(team.TeamFaction + 1) * 100 + (ship.Id + 1) * 10);
        Debug.Log(pv.ViewID);

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) {
            pv.TransferOwnership(PhotonNetwork.MasterClient);
            //spawn = PhotonNetwork.Instantiate("Prefabs/Ship",node.getRealPos(),Quaternion.identity);
        } else {
            
        }

        
        

        spawn.transform.parent = parent.transform;


        return ship;
    }

    /// <summary>
    /// Returns a team based on a team enum type
    /// </summary>
    /// <param name="t">the team type</param>
    /// <returns>A reference to a team</returns>
    public Team getTeam (Team.Faction t) {
        return teams[(int)t];
    }

    /// <summary>
    /// Returns a team based on an integer team index
    /// </summary>
    /// <param name="i">the team index</param>
    /// <returns>A referemce to a team</returns>
    public Team getTeam(int i) {
        if(i < 0 || i >= teams.Length) {
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
                    if (!n.island) {
                        Vector3 pos = new Vector3(n.Y,Board.ROW_OF_NODES - 1 - n.X,0);

                        Gizmos.DrawSphere(pos,Node.GIZMO_SIZE);

                        Node[] adjacents = n.Adjacents;
                        for (int i = 0; i < adjacents.Length; i++) {
                            if (adjacents[i] != null) {
                                Vector3 adjPos = new Vector3(adjacents[i].Y,Board.ROW_OF_NODES - 1 - adjacents[i].X,0);
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
        foreach(Ship s in getHumanShips()) {
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
        playerTeam.aiTeam = false;
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
        foreach (Team.Faction faction in (Team.Faction[])Enum.GetValues(typeof(Team.Faction))) {

            int index = (int)faction;

            if (teamTypes[index] == Team.Type.empty) {
                continue;
            }
            Team t = new Team(faction);
            teams[index] = t;
        }
    }

    public void FindShips() {
        foreach(Team t in teams) {
            t.FindShips();
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
    void assignAI() {
        for(int i = 0; i < teamTypes.Length; i++) {
            if(teamTypes[i] == Team.Type.ai) {
                aiList.Add(new TrierisAI(teams[i]));
            }
        }
    }

    public void changeFXVolume()
    {
        gameObject.GetComponents<AudioSource>()[0].volume = GameObject.Find("OverlayCanvas/OptionsMenu/FXVolumeSlider").GetComponent<Slider>().value; ;
    }

    public void changeMusicVolume()
    {
        gameObject.GetComponents<AudioSource>()[1].volume = GameObject.Find("OverlayCanvas/OptionsMenu/MusicVolumeSlider").GetComponent<Slider>().value; ;
    }

    public void restartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void goToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }


}

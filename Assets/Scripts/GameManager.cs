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

    public bool shipsSynced = false;

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
                ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
                ht["InProgress"] = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            } else {
                Debug.Log("Im NOT the master client");
            }

            setupGame((int)PhotonNetwork.LocalPlayer.CustomProperties["TeamNum"]);
        }
    }

    /// <summary>
    /// Checks every frame if there are pending redirects or port captures
    /// </summary>
    private void Update() {

        //checkForChoices();
        //checkForExecuteNextPhase();
        //PhaseManager.drawFocusMargin();


        if (PhotonNetwork.IsMasterClient) {
            CheckPlayersReady();
        }
    }

    public void CheckPlayersReady() {
        if (shipsSynced)
            return;
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player p in players) {
            if (!(bool)p.CustomProperties["LoadedGame"]) {
                return;
            }
        }
        SyncShipPhotonID();
        SetPortTransparency();
        //SetInitialRedirects();
        PhotonView.Get(this).RPC("SetInitialRedirects",RpcTarget.Others);
        SetInitialRedirects();

        PhotonView.Get(this).RPC("RevealRedirects",RpcTarget.Others);
        RevealRedirects();
        //RevealRedirects();
    }

    public void setupGame(int playerChoice) {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " " + PhotonNetwork.LocalPlayer.CustomProperties["TeamNum"]);
        Debug.Log("Setup for " + playerChoice);

        // TEMPORARY
        if (!PhotonNetwork.IsConnected)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i == playerChoice)
                {
                    teamTypes[i] = Team.Type.player;
                }
                else
                {
                    teamTypes[i] = Team.Type.ai;
                }
            }
        }

        createTeams();

        if (PhotonNetwork.IsConnected)
        {
            for (int i = 0; i < teamTypes.Length; i++)
            {
                switch(teamTypes[i]){
                    case (Team.Type) 2:
                        Debug.Log("Team " + teams[i].TeamFaction + " is off");
                        teams[i].setTeamType((Team.Type)2);
                        break;
                    case (Team.Type) 1:
                        Debug.Log("Team " + teams[i].TeamFaction + " is human");
                        teams[i].setTeamType((Team.Type)1);
                        break;
                    case (Team.Type) 0:
                    default:
                        Debug.Log("Team " + teams[i].TeamFaction + " is ai");
                        teams[i].setTeamType((Team.Type)0);
                        break;
                }
            }

            foreach (Team t in teams)
            {
                if (t.TeamType == (Team.Type)1)
                {
                    t.aiTeam = false;
                }
            }
        }

        playerFaction = (Team.Faction)playerChoice;
        playerTeam = teams[(int)playerFaction];

        Debug.Log("My playerFaction is " + playerFaction.ToString());
        Debug.Log("My team is " + playerTeam.TeamFaction.ToString());

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

            uiControl.PostTeamSelection();


        }

        if (!PhotonNetwork.IsConnected) {
            SetPortTransparency();

            SetInitialRedirects();

            RevealRedirects();
        }

        if (PhotonNetwork.IsConnected) {
            ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
            ht["LoadedGame"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
        }

        cameraLock = false;
        GameObject.Find("TeamIcon").GetComponent<Image>().sprite = playerTeam.getPortSprite();
    }

    public void SyncShipPhotonID() {
        int[] ids = new int[getAllShips().Count];
        int i = 0;
        foreach (Ship s in getAllShips()) {
            ids[i] = s.GetComponent<PhotonView>().ViewID;
            i++;
        }
        PhotonView.Get(this).RPC("SetShipPhotonID",RpcTarget.Others,ids);
        shipsSynced = true;
        Debug.Log("Syncing ships");
    }

    [PunRPC]
    public void SetShipPhotonID(int[] ids) {
        int i = 0;
        foreach (Ship s in getAllShips()) {
            s.GetComponent<PhotonView>().ViewID = ids[i];
            i++;
        }
    }



    // new multiplayer functions
    public bool playersReady() {
        Debug.Log("are the teams ready?");
        foreach (Team t in teams) {
            Debug.Log(t.aiTeam);
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

    /// <summary>
    /// Checks if any ships currently need to be redirected
    /// </summary>
    public bool needRedirect() {
        if (playerTeam == null) {
            return false;
        }
        return playerTeam.needRedirectChoice();

        //foreach (Team t in teams) {
        //    if (t == null) {
        //        continue;
        //    }
        //    if (t.aiTeam == false && t.needRedirectChoice()) {
        //        return true;
        //    }
        //}
        //return false;
    }

    /// <summary>
    /// Checks if any ships need to make a port capture choice
    /// </summary>
    public bool needCaptureChoice() {
        if (playerTeam == null) {
            return false;
        }
        return playerTeam.needCaptureChoice();

        //foreach (Team t in teams) {
        //    if (t == null) {
        //        continue;
        //    }

        //    if (t.aiTeam == false && t.needCaptureChoice()) {
        //        return true;
        //    }
        //}
        //return false;
    }

    public bool needRammingChoice() {
        if(playerTeam == null) {
            return false;
        }
        return playerTeam.needRammingChoice();

        //foreach (Team t in teams) {
        //    if (t == null) {
        //        continue;
        //    }

        //    if (!t.aiTeam && t.needRammingChoice()) {
        //        return true;
        //    }
        //}
        //return false;
    }

    public bool needCatapultChoice() {
        if (playerTeam == null) {
            return false;
        }
        return playerTeam.needCatapultChoice();

        //foreach (Team t in teams) {
        //    if (t == null) {
        //        continue;
        //    }

        //    if (!t.aiTeam && t.needCatapultChoice()) {
        //        return true;
        //    }
        //}
        //return false;
    }

    //[PunRPC]
    //public void teamIsHuman(int i)
    //{
    //    teams[i].setTeamType((Team.Type) 1);
    //}

    [PunRPC]
    public void SetInitialRedirects() {
        foreach (Ship s in playerTeam.ships) {
            s.NeedRedirect = true;
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

        foreach (Team t in teams) {
            r.AddRange(t.ships);
        }

        return r;
    }

    public List<Team> getHumanTeams() {
        List<Team> teams = new List<Team>();
        foreach (Team t in this.teams) {
            if (t != null && !t.aiTeam) {
                teams.Add(t);
            }
        }
        return teams;
    }

    public List<Ship> getHumanShips() {
        List<Ship> ships = new List<Ship>();
        foreach (Team t in getHumanTeams()) {
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
        for (int i = 0; i < teams.Length; i++) {
            if (i != (int)playerTeam.TeamFaction) {
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
                if (ai.GetTeam() == playerTeam) {
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
               
        PhotonView pv = spawn.AddComponent<PhotonView>();
        PhotonTransformView ptv = spawn.AddComponent<PhotonTransformView>();
        pv.ObservedComponents = new List<Component>();
        pv.ObservedComponents.Add(ptv);
        pv.OwnershipTransfer = OwnershipOption.Takeover;
        pv.Synchronization = ViewSynchronization.Unreliable;
        //pv.ViewID = (int)(team.TeamFaction+1) * 100 + (ship.Id+1) * 10;

        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.IsMasterClient) {
                if (!PhotonNetwork.AllocateSceneViewID(pv)) {
                    Debug.LogError("Failed to allocated viewID for ship");
                }
            } else {
                pv.TransferOwnership(PhotonNetwork.MasterClient);
            }
        }
        
        //    Debug.Log((int)(team.TeamFaction + 1) * 100 + (ship.Id + 1) * 10);
        //Debug.Log(pv.ViewID);

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) {
            pv.TransferOwnership(PhotonNetwork.MasterClient);
            //spawn = PhotonNetwork.Instantiate("Prefabs/Ship",node.getRealPos(),Quaternion.identity);
        }
               
        spawn.transform.parent = parent.transform;

        ship.intialize(team,node);
        ship.name = team.TeamFaction.ToString() + " ship " + ship.Id;

        return ship;
    }

    /// <summary>
    /// Returns a team based on a team enum type
    /// </summary>
    /// <param name="t">the team type</param>
    /// <returns>A reference to a team</returns>
    public Team getTeam(Team.Faction t) {
        return teams[(int)t];
    }

    /// <summary>
    /// Returns a team based on an integer team index
    /// </summary>
    /// <param name="i">the team index</param>
    /// <returns>A referemce to a team</returns>
    public Team getTeam(int i) {
        if (i < 0 || i >= teams.Length) {
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
    [PunRPC]
    public void RevealRedirects() {
        foreach (Ship s in playerTeam.ships) {
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
            ship.NeedRedirect = true;
        }
        RevealRedirects();
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
                ship.NeedRedirect = false;
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
        foreach (Team t in teams) {
            t.FindShips();
        }
    }

    /// <summary>
    /// Creates each 5 ports for each team
    /// </summary>
    public void createPorts() {
        foreach (Team t in teams) {
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
        for (int i = 0; i < teamTypes.Length; i++) {
            if (teamTypes[i] == Team.Type.ai) {
                aiList.Add(new TrierisAI(teams[i]));
            }
        }
    }

    public Player findOwnerOfShip(Ship inputShip)
    {
        Team curT = inputShip.team;
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if ((int)p.CustomProperties["TeamNum"] == (int)curT.TeamFaction)
                return p;
        }
        return null;
    }

    public void changeFXVolume()
    {
        gameObject.GetComponents<AudioSource>()[0].volume = GameObject.Find("OverlayCanvas/OptionsMenu/FXVolumeSlider").GetComponent<Slider>().value; ;
    }

    public void changeMusicVolume() {
        gameObject.GetComponents<AudioSource>()[1].volume = GameObject.Find("OverlayCanvas/OptionsMenu/MusicVolumeSlider").GetComponent<Slider>().value; ;
    }

    public void restartGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void goToStartMenu() {
        SceneManager.LoadScene("StartMenu");
    }

    public Ship GetShip(int shipID,int team) {
        foreach (Ship s in teams[team].ships) {
            if (s.Id == shipID)
                return s;
        }
        return null;
    }

    public Port GetPort(int portID) {
        foreach(Port p in board.ports) {
            if(p.id == portID) {
                return p;
            }
        }
        Debug.LogError("Cannot find port from GetPort() id: " + portID);
        return null;
    }

    //[PunRPC]
    //public void SyncDamage(int dmg,int shipID,int team) {
    //    GameManager.main.GetShip(shipID,team).life -= dmg;
    //}

    //[PunRPC]
    //public void SetIconAttack(int shipID, int team) {
    //    GameManager.main.GetShip(shipID,team).setIcon(Sprites.main.AttackIcon);
    //}

    //[PunRPC]
    //public void InitRammingAnimation(int shipID, int team) {
    //    GameManager.main.GetShip(shipID,team).GetComponent<Animator>().SetTrigger("Collision");
    //}

    //[PunRPC]
    //public void DisableShipIcon(int shipID,int team) {
    //    GameManager.main.GetShip(shipID,team).disableIcon();
    //}

    [PunRPC]
    public void SpawnFireball(float startX,float startY,float endX,float endY) {
        Vector2 start = new Vector2(startX,startY);

        GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
        CatapultBullet bullet = GameObject.Instantiate(go,start,Quaternion.identity).GetComponent<CatapultBullet>();

        bullet.startPos = start;
        bullet.endPos = new Vector2(endX,endY);
    }

    //[PunRPC]
    //public void LaunchSound() {
    //    if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
    //        PhotonView.Get(this).RPC("LaunchSound",RpcTarget.Others);
    //    }

    //    Sounds.main.playClip(Sounds.main.Launch);
    //}

    //[PunRPC]
    //public void PlaySplash() {
    //    if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
    //        PhotonView.Get(this).RPC("PlaySplash",RpcTarget.Others);
    //    }

    //    Sounds.main.playClip(Sounds.main.Splash);
    //}

    //[PunRPC]
    //public void PlayFireball() {
    //    if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
    //        PhotonView.Get(this).RPC("PlayFireball",RpcTarget.Others);
    //    }

    //    Sounds.main.playClip(Sounds.main.Fireball);
    //}

    //[PunRPC]
    //public void PlayWhistle() {
    //    if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
    //        PhotonView.Get(this).RPC("PlayWhistle",RpcTarget.Others);
    //    }

    //    Sounds.main.playClip(Sounds.main.Whistle,0.4f);
    //}

    //[PunRPC]
    //public void PlayRandomCrunch() {
    //    if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
    //        PhotonView.Get(this).RPC("PlayRandomCrunch",RpcTarget.Others);
    //    }

    //    Sounds.main.playRandomCrunch();
    //}

    [PunRPC]
    public void SetPortTransparency(int portID, float alpha) {
        foreach(Port p in board.ports) {
            if(p.id == portID) {
                p.SetTransparency(alpha);
                return;
            }
        }
        Debug.LogError("No port found with given ID for port transparency RPC call");
    }

    [PunRPC]
    public void focus(float x,float y) {
        StartCoroutine(PhaseManager.Focus(new Vector2(x,y)));
    }
    
    [PunRPC]
    public void RunPortCaptureAnimation(int team1 , int team2 , float x, float y) {

        //Ship ship = GetShip(shipID,team);

        //Debug.LogFormat("Playing animation for team {0} which is {1} team, port number {2}, ship id {3}",(int)ship.team.TeamFaction,ship.team.TeamFaction,ship.getNode().Port.id,ship.Id);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/PortCaptureAnimation");

        PortCaptureAnimationObject animObj;

        GameObject go = GameObject.Instantiate(prefab,new Vector3(x,y),Quaternion.identity);

        animObj = go.GetComponent<PortCaptureAnimationObject>();

        animObj.SetLowerImg(teams[team1].getPortSprite());
        animObj.SetUpperImg(teams[team2].getPortSprite());

    }

    [PunRPC]
    public void SetPortTeam(int portID, int teamId) {

        board.ports[portID].Team = teams[teamId];
    }

    [PunRPC]
    public void CreateRotationArrow(int team,float x,float y,float rotation,bool portTurn) {
        GameObject prefab = Resources.Load<GameObject>("prefabs/RotationArrow");
        GameObject arrow = GameObject.Instantiate(prefab,new Vector3(x,y),Quaternion.Euler(0,0,rotation));
        arrow.GetComponent<AnimArrow>().Initialize(teams[team]);
        if (portTurn) {
            arrow.transform.localScale = new Vector3(-1,1,1);
        }
    }

    [PunRPC]
    public void CreateMovementArrow(int team,float x,float y,float rotation,int momentum = 0,bool reverse = false) {
        GameObject prefab = Resources.Load<GameObject>("prefabs/MovementArrow");
        GameObject arrow = GameObject.Instantiate(prefab,new Vector3(x,y),Quaternion.Euler(0,0,rotation));
        arrow.GetComponent<AnimArrow>().Initialize(teams[team],momentum);
        if (reverse) {
            arrow.transform.localScale = new Vector3(0.158f,-0.158f,0.158f);
        }        
    }

    void SyncPortTransparency() {
        PhotonView.Get(this).RPC("SetPortTransparency",RpcTarget.Others);
        SetPortTransparency();
    }

    [PunRPC]
    public void SetPortTransparency() {
        foreach (Port p in board.ports) {
            p.TransparencyCheck();
        }
    }

    [PunRPC]
    public void DisablePhaseUI() {
        PhaseManager.DisablePhaseUI();
    }

    [PunRPC]
    public void EnablePhaseUI() {
        PhaseManager.EnablePhaseUI();
    }

    [PunRPC]
    public void setSubphaseText(string s) {
        PhaseManager.setSubphaseText(s);
    }

    [PunRPC]
    public void updateText(int phase) {
        PhaseManager.updateText(phase);
    }

    [PunRPC]
    public void subPhaseProgress(int index) {
        PhaseManager.subPhaseProgress(index);
    }

    [PunRPC]
    public void SendTargetInfo(int shipID,int teamID,int[] targetIDs,int[] targetTeamIDs) {

        if(teamID != (int)playerTeam.TeamFaction) {
            Debug.Log("Player's team does NOT own this multi target choice");
            return;
        }

        Debug.Log("Player's team DOES own this multi target choice");

        Ship attacker = GetShip(shipID,teamID);
        List<Ship> targets = new List<Ship>();
        for(int i = 0; i < targetIDs.Length; i++) {
            targets.Add(GetShip(targetIDs[i],targetTeamIDs[i]));
        }
        
        ShipTargetResolution targetChoice = new ShipTargetResolution(attacker,targets);
        StartCoroutine(targetChoice.resolve());        
    }

    [PunRPC]
    public void SyncTargetChoice(int shipID, int teamID) {
        PhaseManager.chosenTarget = GetShip(shipID,teamID);
    }

    public bool HumanNeedsRedirect() {
        foreach(Team t in teams) {
            if (t.needRedirectChoice()) {
                return true;
            }
        }
        return false;
    }

    public bool HumanNeedsCaptureChoice() {
        foreach (Team t in teams) {
            if (t.needCaptureChoice()) {
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    public void ActivatePortPrompt(int shipID, int teamID, int portID) {

        if(teamID != (int)playerTeam.TeamFaction) {
            return;
        }

        Ship ship = GetShip(shipID,teamID);
        Port port = GetPort(portID);

        port.activatePrompt(ship);
    }

    [PunRPC]
    public void PlayerCapturePort(int shipID, int teamID) {

        Ship ship = GetShip(shipID,teamID);
        //Port port = ship.getNode().Port;

        //foreach(Ship s in ship.getNode().Ships) {
        //    s.NeedCaptureChoice = false;
        //}

        PortCaptureAnimation anim = new PortCaptureAnimation(ship);
        anim.playAnimation();

        //PhotonView.Get(this).RPC("RunPortCaptureAnimation",RpcTarget.Others,teamID,(int)port.Team.TeamFaction,ship.Position.x,ship.Position.y);

    }

    [PunRPC]
    public void CheckPortCaptureChoice() {
        foreach(Ship s in playerTeam.ships) {
            if (s.NeedCaptureChoice) {
                ActivatePortPrompt(s.Id,(int)s.team.TeamFaction,s.PortID);
                return;
            }
        }
    }
}

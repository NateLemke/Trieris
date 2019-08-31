using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port {

    public int id;

    public bool IsCapital { get; set; }

    // the team that the port belongs to
    private Team originalTeam;
    //private int teamBackup;
    public Team OriginalTeam { get { return originalTeam; } }

    //public Team.Faction teamFaction { get; set; }
    public Team Team {
        get {
            return team;
        }
        set {
            team = value;
            setSprite(team);
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
                PhotonView.Get(GameManager.main).RPC("SetPortTeam",RpcTarget.Others,id,(int)team.TeamFaction);
            }
            go.transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = team.getColor();
        }
    }
    private Team team;

    // The reference to the instantiated port prefab object that contains sprite renders and capture UI
    private GameObject go;

    // the location of the port
    public Node node { get; set; }

    // reference to port's main sprite renderer (not the minimap renderer)
    SpriteRenderer spriteRenderer;

    //public Port(Team t) {
    //    team = t;
    //}

    /// <summary>
    /// Constructor
    /// Instantiates a port prefab gameobject and keeps the reference to it
    /// </summary>
    /// <param name="boardPos">the port's node position on the board</param>
    /// <param name="t">the team the port belongs to</param>
    /// <param name="isCaptial">sets the port to be a capital or not</param>
    public Port(Vector2Int boardPos,Team t,bool isCaptial) {
        team = t;
        IsCapital = isCaptial;
        originalTeam = team;
        node = GameManager.main.Board.getNodeAt(boardPos);
        go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Port"));
        go.transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = t.getColor();
        go.transform.position = node.getRealPos();
        spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = IsCapital ? t.CapitalSprite : t.PortSprite;
        go.name = IsCapital ? t.ToString() + " captial" : "port";
        GameObject parent;
        if ((parent = GameObject.Find("Ports")) == null) {
            parent = GameObject.Instantiate(new GameObject());
            parent.name = "Ports";
        }
        go.transform.SetParent(parent.transform);
        //updateTransparency();
    }

    public GameObject getGameObject() {
        return go;
    }

    /// <summary>
    /// Changes ownership of the port to another team
    /// also changes the sprite and minimap sprite
    /// </summary>
    /// <param name="t"></param>
    //public void setTeam(Team t) {
    //    team = t;
    //    setSprite(team);
    //    go.transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = t.getColor();
    //}

    //public void setColor(Color color) {
    //    this.color = color;
    //}

    /// <summary>
    /// changes the sprite for the port
    /// gets the sprite from the given team input
    /// </summary>
    /// <param name="t">the team to change the port to</param>
    private void setSprite(Team t) {
        spriteRenderer.sprite = IsCapital ? t.CapitalSprite : t.PortSprite;
    }

    /// <summary>
    /// Checks if a ship is present on the node and sets the sprite to be transparent if so
    /// </summary>
    public void TransparencyCheck() {

        float alpha;
        if (node.getNumberOfShips() == 0) {
            alpha = 1;
        } else {
            alpha = 0.5f;
        }
        SetTransparency(alpha);


        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
            PhotonView.Get(GameManager.main).RPC("SetPortTransparency",RpcTarget.Others,id,alpha);
        }
    }

    public void SetTransparency(float alpha) {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    /// <summary>
    /// Activates the port capture prompt UI attached to this port
    /// </summary>
    /// <param name="s">The particular ship the prompt is for</param>
    public void activatePrompt(Ship s) {
        go.GetComponent<PortPrompt>().activateNotification(this,s);
    }

    //public void BackupData() {
    //    teamBackup = (int)team.TeamFaction;
    //}

    //public void Restore() {
    //    Team = GameManager.main.teams[teamBackup];
    //}
}

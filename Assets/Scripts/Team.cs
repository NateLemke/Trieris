using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Represents one team in the game, and contains it's ships and ports.
/// There are 6 teams in the game, including the player's team.
/// </summary>
public class Team {
    public enum Type { ai, player, empty }
    public enum Faction { red = 0 , orange = 1, yellow =2, green=3, blue=4, black=5 }
    public int shipIdCounter = 0;
    public string playerName;

    public Team.Faction teamFaction;
    
    public List<Ship> ships { get; set; }
    public List<Port> ports { get; set; }

    public Sprite ShipSprite { get { return Sprites.getTeamShip(this); } }
    public Sprite PortSprite { get { return Sprites.getTeamPort(this); } }
    public Sprite CapitalSprite { get { return Sprites.getColoredCaptial(this); } }

    // new for multiplayer
    public bool aiTeam = false;

    private bool ready;
    public bool Ready {
        get { return ready; }
        set
        {
            ready = value;
            GameObject.Find("OverlayCanvas/UIBottomPanel/TeamImage" + (int)teamFaction + "/ReadyIcon").gameObject.SetActive(ready);
        }
    }

    public bool eliminated = false;

    /// <summary>
    /// Basic constructor for the team.
    /// Creates new lists for ports and ships.
    /// </summary>
    /// <param name="t"></param>
    public Team(Team.Faction t) {
        ships = new List<Ship>();
        ports = new List<Port>();
        teamFaction = t;
        //loadSprites();
        setPortsAndCapital();
    }

    public int portsCaptured() {
        int count = 0;
        foreach(Port p in GameManager.main.Board.ports) {
            if(p.Team.teamFaction == teamFaction) {
                count++;
            }
        }
        return count;
    }

    public void toggleReady() {
        ready = !ready;
        if (ready) {

        }
    }

    public bool needRammingChoice() {
        foreach(Ship s in ships) {
            if (s.needRammingChoice) {
                return true;
            }
        }
        return false;
    }

    internal bool needCatapultChoice() {
        foreach (Ship s in ships) {
            if (s.needCatapultChoice) {
                return true;
            }
        }
        return false;
    }

    internal bool needCaptureChoice() {
        foreach (Ship s in ships) {
            if (s.needCaptureChoice) {
                return true;
            }
        }
        return false;
    }

    internal bool needRedirectChoice() {
        foreach(Ship s in ships) {
            if (s.needRedirect) {
                return true;
            }
        }
        return false;
    }
    // end of new for multiplayer




    /// <summary>
    /// Returns the port sprite for this particular team.
    /// </summary>
    /// <returns>The port sprite for this team.</returns>
    public Sprite getPortSprite() {
        //return portSprite;
        switch (teamFaction) {
            case Faction.red:
            return Sprites.main.RedPort;
            case Faction.orange:
            return Sprites.main.OrangePort;
            case Faction.yellow:
            return Sprites.main.YellowPort;
            case Faction.green:
            return Sprites.main.GreenPort;
            case Faction.blue:
            return Sprites.main.BluePort;
            case Faction.black:
            return Sprites.main.BlackPort;
            default:
            return null;

        }

    }
    //public Sprite getcaptialSprite() {
    //    return capitalSprite;
    //}

    //private void loadSprites() {
        //try {
        //    shipSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Ship");
        //} catch (Exception e) {
        //    Debug.LogWarning("Could not load ship sprite for team " + teamType.ToString());
        //}

        //try {
        //    portSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Port");
        //} catch (Exception e) {
        //    Debug.LogWarning("Could not load port sprite for team " + teamType.ToString());
        //}

        //try {
        //    capitalSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Cap");
        //} catch (Exception e) {
        //    Debug.LogWarning("Could not load capital sprite for team " + teamType.ToString());
        //}
    //}

    private void setPortsAndCapital() {
        

        //StreamReader reader = new StreamReader("Assets/Resources/WorldData/"+ teamType.ToString() + "Ports.txt");
        //string s;
        //while ((s = reader.ReadLine()) != null) {
        //    //Debug.Log(s);
        //    string[] split = s.Split(',');
        //    int x = int.Parse(split[0]);
        //    int y = int.Parse(split[1]);
        //    bool isCapital = !(int.Parse(split[2]) == 0);
        //    //Debug.Log("Making new port!");
        //    Port p = new Port(new Vector2Int(x,y),this,isCapital);
        //    GameManager.main.getBoard().getNodeAt(x,y).setPort(p);
        //}
        //reader.Close();
    }

    [Obsolete("Ships get auto spawned when ports and capitals are constructed")]
    private void spawnShips() {

    }

    /// <summary>
    /// Returns the teamtype for this Team.
    /// </summary>
    /// <returns>The teamtype for this team.</returns>
    public Faction getTeamType() {
        return teamFaction;
    }

    /// <summary>
    /// Returns the standard colour for this particular team.
    /// </summary>
    /// <returns>The standard colour this team.</returns>
    public Color getColor() {
        switch (teamFaction) {
            case Team.Faction.black:
            return CustomColor.TeamBlack;
            case Team.Faction.blue:
            return CustomColor.TeamBlue;
            case Team.Faction.green:
            return CustomColor.TeamGreen;
            case Team.Faction.orange:
            return CustomColor.TeamOrange;
            case Team.Faction.red:
            return CustomColor.TeamRed;
            case Team.Faction.yellow:
            return CustomColor.TeamYellow;
            default:
            Debug.LogError("Invalid team type");
            return Color.white;
        }
    }

    /// <summary>
    /// Returns the light colour for this particular team.
    /// </summary>
    /// <returns>The light colour this team.</returns>
    public Color getColorLight() {
        switch (teamFaction) {
            case Team.Faction.black:
            return CustomColor.TeamBlackLight;
            case Team.Faction.blue:
            return CustomColor.TeamBlueLight;
            case Team.Faction.green:
            return CustomColor.TeamGreenLight;
            case Team.Faction.orange:
            return CustomColor.TeamOrangeLight;
            case Team.Faction.red:
            return CustomColor.TeamRedLight;
            case Team.Faction.yellow:
            return CustomColor.TeamYellowLight;
            default:
            Debug.LogError("Invalid team type");
            return Color.white;
        }
    }

    /// <summary>
    /// Returns a string version of the teamtype for this team.
    /// </summary>
    /// <returns>the string version of the teamtype for this team.</returns>
    public override string ToString() {
        return teamFaction.ToString();
    }

    /// <summary>
    /// Creates all ports that belong to this team and places them on the board.
    /// </summary>
    public void createPorts() {
        if (GameManager.main.Board == null) {
            Debug.LogError("Cannot set team ports and capital until game board has been created");
            return;
        }

        TextAsset portsFile = Resources.Load<TextAsset>("WorldData/" + teamFaction.ToString() + "Ports");
        string portsText = portsFile.text;
        string[] portsLines = Regex.Split(portsText,"\n");
        foreach (string s in portsLines) {
            string[] split = s.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            bool isCapital = !(int.Parse(split[2]) == 0);
            //Debug.Log("Making new port!");
            Port p = new Port(new Vector2Int(x,y),this,isCapital);
            ports.Add(p);
            GameManager.main.Board.getNodeAt(x,y).Port = p;
            GameManager.main.Board.ports.Add(p);
        }
    }

    /// <summary>
    /// Creates all the ships belonging to this team and places them on each port.
    /// </summary>
    public void createShips() {

        if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {

        }

        foreach(Port p in ports) {
            GameManager.main.spawnShip(p.node,this);
        }
    }

    public void FindShips() {
        for(int i = 0; i < 5; i++) {
            Ship s = GameObject.Find(teamFaction.ToString() + " ship " + i).GetComponent<Ship>();
            ships.Add(s);
            s.team = this;
        }
    }


}

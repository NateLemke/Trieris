using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Team 
{
    public enum Type { red = 0 , orange = 1, yellow =2, green=3, blue=4, black=5 }
    public int shipIdCounter = 0;

    Team.Type teamType;
    public List<Ship> ships { get; set; }

    Sprite shipSprite;
    Sprite portSprite;
    Sprite capitalSprite;

    public Team(Team.Type t) {
        ships = new List<Ship>();
        teamType = t;
        loadSprites();
        setPortsAndCapital();
    }

    public Sprite getShipSprite() {
        return shipSprite;
    }
    public Sprite getPortSprite() {
        return portSprite;
    }
    public Sprite getcaptialSprite() {
        return capitalSprite;
    }

    private void loadSprites() {
        try {
            shipSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Ship");
        } catch (Exception e) {
            Debug.LogWarning("Could not load ship sprite for team " + teamType.ToString());
        }

        try {
            portSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Port");
        } catch (Exception e) {
            Debug.LogWarning("Could not load port sprite for team " + teamType.ToString());
        }

        try {
            capitalSprite = Resources.Load<Sprite>("Sprites/" + teamType.ToString() + "Cap");
        } catch (Exception e) {
            Debug.LogWarning("Could not load capital sprite for team " + teamType.ToString());
        }
    }

    private void setPortsAndCapital() {
        if(GameManager.main.getBoard() == null) {
            Debug.LogError("Cannot set team ports and capital until game board has been created");
            return;
        }

        TextAsset portsFile = Resources.Load<TextAsset>("WorldData/" + teamType.ToString() + "Ports");
        string portsText = portsFile.text;
        string[] portsLines = Regex.Split(portsText,"\n");
        foreach(string s in portsLines) {
            string[] split = s.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            bool isCapital = !(int.Parse(split[2]) == 0);
            //Debug.Log("Making new port!");
            Port p = new Port(new Vector2Int(x,y),this,isCapital);
            GameManager.main.getBoard().getNodeAt(x,y).setPort(p);
        }

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

    public Type getTeamType() {
        return teamType;
    }

    public Color getColor() {
        switch (teamType) {
            case Team.Type.black:
            return Color.black;
            case Team.Type.blue:
            return Color.blue;
            case Team.Type.green:
            return Color.green;
            case Team.Type.orange:
            return new Color(1f,0.444f,0f);
            case Team.Type.red:
            return Color.red;
            case Team.Type.yellow:
            return Color.yellow;
            default:
            Debug.LogError("Invalid team type");
            return Color.white;
        }
    }

    public override string ToString() {
        return teamType.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port {
    private bool capital;

    // type TeamColor -> Color
    private Color tColor;

    private Color color;

    public Team.Type teamType { get; set; }
    public Team Team {
        get {
            return team;
        }
        set {
            team = value;
            setSprite(team);
        }
    }
    private Team team;

    private GameObject go;

    //private Vector2 pos;

    public Node node { get; set; }

    SpriteRenderer spriteRenderer;

    public Port(Team t) {
        team = t;
    }

    public Port(Vector2Int p,Team t,bool isCaptial) {
        //Debug.Log("New port!");
        team = t;
        capital = isCaptial;
        node = GameManager.main.getBoard().getNodeAt(p);
        go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Port"));

        go.transform.position = node.getRealPos();
        spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = capital ? t.getcaptialSprite() : t.getPortSprite();
        GameObject parent;
        if((parent = GameObject.Find("Ports")) == null) {
            parent = GameObject.Instantiate(new GameObject());
            parent.name = "Ports";
        }
        go.transform.SetParent(parent.transform);
        GameManager.main.spawnShip(node,t);
    }

    public GameObject getGameObject()
    {
        return go;
    }

    //public Port(bool isCapital,Color teamColor) {
    //    setCapital(isCapital);
    //    setTeamColor(teamColor);
    //    setColor(teamColor);
    //}

    public bool getCapital() {
        return capital;
    }

    //public Color getTeamColor() {
    //    return tColor;
    //}

    public Team getTeam() {
        return team;
    }

    public Color getColor() {
        return color;
    }

    public void setCapital(bool isCapital) {
        capital = isCapital;
    }

    //public void setTeamColor(Color teamColor) {
    //    tColor = teamColor;
    //}

    public void setTeam(Team t) {
        team = t;
        setSprite(team);
    }

    public void setColor(Color color) {
        this.color = color;
    }

    private void setSprite(Team t) {
        spriteRenderer.sprite = capital ? t.getcaptialSprite() : t.getPortSprite();
    }


}

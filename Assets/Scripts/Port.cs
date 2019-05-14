using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port{
    private bool capital;

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

    public Node node { get; set; }

    SpriteRenderer spriteRenderer;

    public Port(Team t) {
        team = t;
    }

    /// <summary>
    /// Constructor
    /// Instantiates a port prefab gameobject and keeps the reference to it
    /// </summary>
    /// <param name="boardPos">the port's node position on the board</param>
    /// <param name="t">the team the port belongs to</param>
    /// <param name="isCaptial">sets the port to be a capital or not</param>
    public Port(Vector2Int boardPos,Team t,bool isCaptial) {
        team = t;
        capital = isCaptial;
        node = GameManager.main.getBoard().getNodeAt(boardPos);
        go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Port"));
        go.transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = t.getColor();
        go.transform.position = node.getRealPos();
        spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = capital ? t.CapitalSprite : t.PortSprite;
        go.name = capital ? t.ToString() + " captial" : "port";
        GameObject parent;
        if((parent = GameObject.Find("Ports")) == null) {
            parent = GameObject.Instantiate(new GameObject());
            parent.name = "Ports";
        }
        go.transform.SetParent(parent.transform);        
        setTransparency();
    }

    public GameObject getGameObject()
    {
        return go;
    }


    public bool getCapital() {
        return capital;
    }

    public Team getTeam() {
        return team;
    }

    public Color getColor() {
        return color;
    }

    public void setCapital(bool isCapital) {
        capital = isCapital;
    }

    /// <summary>
    /// Changes ownership of the port to another team
    /// also changes the sprite and minimap sprite
    /// </summary>
    /// <param name="t"></param>
    public void setTeam(Team t) {
        team = t;
        setSprite(team);
        go.transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = t.getColor();
    }

    //public void setColor(Color color) {
    //    this.color = color;
    //}

    /// <summary>
    /// changes the sprite for the port
    /// gets the sprite from the given team input
    /// </summary>
    /// <param name="t">the team to change the port to</param>
    private void setSprite(Team t) {
        spriteRenderer.sprite = capital ? t.CapitalSprite : t.PortSprite;
    }

    /// <summary>
    /// Checks if a ship is present on the node and sets the sprite to be transparent if so
    /// </summary>
    public void setTransparency() {
        Color c = spriteRenderer.color;
        if(node.getNumberOfShips() == 0) {
            c.a = 1;
        } else {
            c.a = 0.5f;
        }
        spriteRenderer.color = c;
    }

    /// <summary>
    /// Activates the port capture prompt UI attached to this port
    /// </summary>
    /// <param name="s">The particular ship the prompt is for</param>
    public void activatePrompt(Ship s) {
        go.GetComponent<PortPrompt>().activateNotification(this,s);
    }


}

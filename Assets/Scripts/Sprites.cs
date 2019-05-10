using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour {

    [SerializeField]
    private Sprite attackIcon;
    public Sprite AttackIcon { get { return attackIcon; } }

    [SerializeField]
    private Sprite targetIcon;
    public Sprite TargetIcon { get { return targetIcon; } }

    [SerializeField]
    private Sprite emptyIcon;
    public Sprite EmtpyIcon { get { return emptyIcon; } }

    [SerializeField]
    private Sprite sinkIcon;
    public Sprite SinkIcon { get { return sinkIcon; } }

    [SerializeField]
    private Sprite captial;
    public Sprite Capital { get { return captial; } }

    [SerializeField]
    private Sprite redPort;
    public Sprite RedPort { get { return redPort; } }

    [SerializeField]
    private Sprite orangePort;
    public Sprite OrangePort { get { return orangePort; } }

    [SerializeField]
    private Sprite yellowPort;
    public Sprite YellowPort { get { return yellowPort; } }

    [SerializeField]
    private Sprite greenPort;
    public Sprite GreenPort { get { return greenPort; } }

    [SerializeField]
    private Sprite bluePort;
    public Sprite BluePort { get { return bluePort; } }

    [SerializeField]
    private Sprite blackPort;
    public Sprite BlackPort { get { return blackPort; } }

    [SerializeField]
    private Sprite straightArrow;
    public Sprite StraightArrow { get { return straightArrow; } }

    [SerializeField]
    private Sprite curvedArrow;
    public Sprite CurvedArrow { get { return curvedArrow; } }

    [SerializeField]
    private Sprite holdSprite;
    public Sprite HoldSprite { get { return holdSprite; } }

    [SerializeField]
    private Sprite emptySprite;
    public Sprite EmptySprite { get { return emptySprite; } }

    [SerializeField]
    private List<Sprite> teamShips = new List<Sprite>();
    public List<Sprite> TeamShips { get { return teamShips; } }

    public static Sprites main;

    void Awake() {
        main = this;
    }

    public static Sprite getColoredCaptial(Team team) {
        Sprite capital = main.Capital;
        Texture2D t = new Texture2D(capital.texture.width,capital.texture.height);

        //Sprite2 = 

        for (int y = 0; y < t.height; y++) {
            for (int x = 0; x < t.width; x++) {
                //if( y > 10 && y < 200) {
                //    t.SetPixel(x,y,Color.red);
                //} else {
                //    t.SetPixel(x,y,s.texture.GetPixel(x,y));
                //}
                if (capital.texture.GetPixel(x,y) == Color.magenta) {
                    t.SetPixel(x,y,team.getColor());
                } else {
                    t.SetPixel(x,y,capital.texture.GetPixel(x,y));
                }
            }
        }
        t.Apply();
        return Sprite.Create(t,new Rect(0f,0f,t.width,t.height),new Vector2(0.5f,0.5f));
    }

    public static Sprite getTeamPort(Team team) {
        switch (team.getTeamType()) {
            case Team.Type.red:
            return main.RedPort;
            case Team.Type.orange:
            return main.OrangePort;
            case Team.Type.yellow:
            return main.YellowPort;
            case Team.Type.green:
            return main.GreenPort;
            case Team.Type.blue:
            return main.BluePort;
            case Team.Type.black:
            return main.BlackPort;
            default:
            return null;
        }
    }

    public static Sprite getTeamShip(Team team) {
        return main.TeamShips[(int)team.getTeamType()];
    }

}

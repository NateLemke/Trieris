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

    public static Sprites main;

    void Start() {
        main = this;
    }

    

}

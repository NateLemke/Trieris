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

    public static Sprites main;

    void Start() {
        main = this;
    }

    

}

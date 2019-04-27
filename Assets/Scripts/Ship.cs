using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ship : MonoBehaviour {

    public const int FORWARD = 1;
    public const int PORT = 2;
    public const int STARBOARD = 3;
    public const int HOLD = 4;
    public const int REVERSE = 5;

    public const int MAX_HEALTH = 4;

    // Combat Text
    public GameObject CBTprefab;

    //private List<Action> actions = new List<Action>();
    public Action[] actions;
    private Node node;

    // type TeamColor -> Color
    //private Color teamColor;
    
    //public int life = MAX_HEALTH;
    private int momentum = 0;
    private int portRepairCount = 0;
    private bool movedForward = false;

    public int fireDirection = -1;

    // ship front means ship direction
    private int front = 4;
    private bool canAct = true;
    public bool canActAfterCollision = true;
    private int frontAfterCollision = -1;
    private int id = -1;
    TrierisAI ai = null;

    // new variables
    //public bool playingAnimation { get; set; }
    delegate bool animateDelegate();
    animateDelegate animate;
    public AnimationCurve MoveCurve;
    //public bool crashed;
    public bool needRedirect;
    public bool needCaptureChoice;
    //private 
    private float animationSpeed = 0.7f;
    private float animationStart;
    SpriteRenderer underlay;
    public Team team;
    public Vector3 Position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }

    public int currentActionIndex;


    private int lifeValue;
    public int life
    {
        get
        {
            return lifeValue;
        }
        set
        {
            if (value < lifeValue)
            {
                InitCBT((lifeValue - value).ToString());
                lifeValue = value;
            }
            else
            {
                lifeValue = value;
            }
        }
    }

    

    // shouldn't use constructors with monobehavior game objects

    //public Ship(Color teamColor,int id,Node node) {
    //    this.teamColor = teamColor;
    //    this.id = id;
    //    this.node = node;
    //    node.getShips().Add(this);

    //    Debug.Log("hello from ship constructor!");

    //    //for (int i = 0; i < 4; i++) {
    //    //    actions.Add(null);
    //    //}

    //    // game object stuff
    //    //GameObject go = new GameObject();        
    //    //go.AddComponent<SpriteRenderer>();
    //    //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
    //    //sr.sprite = Resources.Load<Sprite>("blueShip");
    //    //go.transform.position = node.getPosition();
    //    //go.transform.localScale = new Vector3(2,2,2);
    //    //Debug.Log("hello!");
    //}

    public void intialize(Team team,Node node) {
        this.team = team;
        team.ships.Add(this);
        this.id = team.shipIdCounter++;
        this.life = MAX_HEALTH;
        
        this.node = node;
        gameObject.transform.position = node.getRealPos();

        actions = new Action[4];
        setSpriteRotation();

        populateDefaultActions();

        this.GetComponent<SpriteRenderer>().sprite = team.getShipSprite();

        //actions[0] = new ReverseAction(1,this);
        //actions[1] = new ReverseAction(1,this);
        //actions[2] = new ReverseAction(1,this);
        //actions[3] = new ReverseAction(1,this);

        currentActionIndex = 0;
    }

    public void setAction(int index,int actionNum,int firingDirection) {                   // throws CannotReverseException, InvalidActionException, InvalidActionIndexException
        if (index > life - 1 || index < 0) {
            //throw new InvalidActionIndexException();
        }
        if (actionNum == 5 && (index == 0 || actions[index - 1] == null ||
                !actions[index - 1].reverseReady)) {
            //throw new CannotReverseException();
        } else {
            Action a = getAction(actionNum,firingDirection);
            actions[index] = a;
        }
    }

    public void doAction(int index) {      //throws ShipCrashedException
        //Debug.Log("do action ...");
        actions[index].act();

    }

    public  abstract class Action {
        protected int catapultDirection = -1;
        public bool reverseReady = false;
        public int actionIndex;
        // since java inner classes are different than c sharp nested classes
        // we need a wave for these nested classes to communicate with their ship
        protected Ship ship;

        public Action(int catapultDirection, Ship ship, int index) {
            this.ship = ship;
            this.catapultDirection = catapultDirection;
            actionIndex = index;
        }

        public void act() {                 //throws ShipCrashedException 
            if (ship.canAct) {
                affectShip();
                ship.fireDirection = catapultDirection;
            }
        }

        protected abstract void affectShip();      // throws ShipCrashedException

    }

    public class ForwardAction : Action {

        public ForwardAction(int catapultDirection, Ship ship, int index) : base(catapultDirection,ship,index) {

        }

        protected override void affectShip() {           // throws ShipCrashedException 
            ship.move(ship.front);
            ship.speedup();
        }
    }

    public class ReverseAction : Action {

        public ReverseAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {

        }

        protected override void affectShip() {              //throws ShipCrashedException
            ship.move(ship.getRelativeDirection(-4));
        }
    }

    public class HoldAction : Action {

        public HoldAction(int catapultDirection,Ship ship,int index) : base(catapultDirection,ship,index) {
            reverseReady = true;
        }

        protected override void affectShip() {
            ship.hold();
            if (catapultDirection == -1) {
                ship.repair();
            }
        }
    }

    public class StarboardAction : Action {

        public StarboardAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {
            
        }

        protected override void affectShip() {
            ship.turn(1);
        }
    }

    public class PortAction : Action {

        public PortAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {
            
        }

        protected override void affectShip() {
            ship.turn(-1);
        }
    }

    public int getFront() {
        return front;
    }

    public int getMomentum() {
        return momentum;
    }

    public Node getCatapultNode() {
        if (fireDirection == -1) {
            return null;
        }
        if (fireDirection == 8) {
            return node;
        }
        return node.getAdjacentNode(getRelativeDirection(fireDirection));
    }

    public Node getNode() {
        return node;
    }

    //public Color getTeamColor() {
    //    return teamColor;
    //}

    public int getLife() {
        return life;
    }

    public bool getCanAct() {
        return canAct;
    }

    public bool getMoved() {
        return movedForward;
    }

    public int getID() {
        return id;
    }

    public bool ready() {
        foreach (Action action in actions) {
            if (action == null) {
                Debug.Log("ship not ready");
                return false;
            }
        }
        return true;
    }

    public void ram(Ship target) {
        if (target != null) {
            // debug from previous project
            //if ((this.teamColor == TeamColor.BLUE && this.id == 1 || this.teamColor == TeamColor.BLACK && this.id == 2)) {
            //    Debug.Log(teamColor + " 1 front: " + front + " target: " + target.front + " moved forward " + movedForward);
            //}
            ramDamageAndAngle(target);
            // debug from previous project
            //if ((this.teamColor == TeamColor.BLUE && this.id == 1 || this.teamColor == TeamColor.BLACK && this.id == 2)) {
            //    Debug.Log(teamColor + " 1 front: " + frontAfterCollision + " target: " + target.frontAfterCollision + " moved forward " + movedForward);
            //}
            momentum = 0;

            // pause the turn I guess
            //Debug.Log("RAMED");

            //playingAnimation = true;
        }
    }

    public void catapult(Ship target) {
        if (target != null) {
            target.life -= 1;
            GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
            CatapultBullet bullet = Instantiate(go,Position,Quaternion.identity).GetComponent<CatapultBullet>();
            bullet.target = target;
            bullet.startPos = Position;
            //Debug.Log("firing catapult");
        }

    }

    public void reset() {
        Debug.Log("resetting...");
        canAct = true;
        portRepairCount = 0;
        momentum = 0;
        movedForward = false;
        frontAfterCollision = -1;
        canActAfterCollision = true;
        fireDirection = -1;
        for (int i = 0; i < 4; i++) {
            actions[i] = null;
        }
    }

    public void move(int direction) {                           //throws ShipCrashedException 
        //Debug.Log("Begin")
        DebugControl.log("action","----moving ship");
        Node destNode = node.getAdjacentNode(direction);
        if (destNode == null) {
            life--;
            canActAfterCollision = false;
            Debug.Log("----Ship crashed");
            needRedirect = true;
            return;
            //GameManager.main.shipCrashed = true;
            //throw new ShipCrashedException(this);
        }

        //new line
        //endNode = node;

        //startAnimation(forwardsAnimate);
        //startAnimationCo(animateForwardsCo(node.getRealPos(),destNode.getRealPos()));


        AnimationManager.actionAnimations.Add(this,new MovementAnimation(node,destNode,this));



        node.getShips().Remove(this);
        node = destNode;
        node.getShips().Add(this);
    }


    public void turn(int relativeDirection) {
        front = getRelativeDirection(relativeDirection);
        momentum = 0;
        //Debug.Log("----Turning ship "+relativeDirection);

        //startAnimationCo(animateRotate(this.transform.rotation,this.transform.rotation * Quaternion.Euler(0,0,-45 * relativeDirection)));
        AnimationManager.actionAnimations.Add(this,new RotationAnimation(this.transform.rotation,this.transform.rotation * Quaternion.Euler(0,0,-45 * relativeDirection),this));



    }

    public void setFront(int direction) {
        front = direction % 8;
    }

    public void hold() {
        //Debug.Log("holding...");
        momentum = 0;
    }

    public void speedup() {
        DebugControl.log("movement","Speeding up...");
        momentum++;
        movedForward = true;
    }

    public void repair() {
        if (node.getPort() != null && life < MAX_HEALTH) {
            if (node.getPort().getCapital() && node.getPort().getTeam() == team) {
                life++;
            } else {
                portRepairCount++;
                if (portRepairCount == life) {
                    life++;
                    portRepairCount = 0;
                }
            }
        }
    }

    public void sink() {
        if (node != null) {
            node.getShips().Remove(this);
        }
        node = null;
    }

    public void capturePort() {
        node.getPort().setTeam(team);
        canActAfterCollision = false;
        canAct = false;
        
    }

    public void updateFrontAfterCollision() {
        movedForward = false;
        if (frontAfterCollision != -1) {
            front = frontAfterCollision;
        }
        canAct = canActAfterCollision;
        frontAfterCollision = -1;
        setSpriteRotation();
    }

    public void populateDefaultActions() {
        for (int i = 0; i < MAX_HEALTH; i++) {
            if (actions[i] == null) {
                actions[i] = new HoldAction(-1,this,4);
            }
        }
    }

    private Action getAction(int actionNum,int firingDirection) {       // throws InvalidActionException
        switch (actionNum) {
            case 1:
            //Debug.Log("adding forwards action");
            return new ForwardAction(firingDirection,this,1);
            case 2:
            return new PortAction(firingDirection,this,2);
            case 3:
            return new StarboardAction(firingDirection,this,3);
            case 4:
            return new HoldAction(firingDirection,this,4);
            case 5:
            return new ReverseAction(firingDirection,this,5);
            default:
            throw new InvalidActionException();
        }
    }

    private int getRelativeDirection(int relativeDelta) {
        relativeDelta %= 8;
        int result = front + relativeDelta;
        if (result > 7) {
            result -= 8;
        } else if (result < 0) {
            result += 8;
        }
        return result;
    }

    private void ramDamageAndAngle(Ship target) {
        int enemyAngle = target.front;
        if (!target.movedForward && (enemyAngle == getRelativeDirection(2) ||
                enemyAngle == getRelativeDirection(6))) {
            broadsideRam(target);
        } else if (enemyAngle == getRelativeDirection(1) ||
                enemyAngle == getRelativeDirection(2) ||
                enemyAngle == getRelativeDirection(3)) {
            glancingRam(target,-1);
        } else if (enemyAngle == getRelativeDirection(5) ||
                enemyAngle == getRelativeDirection(6) ||
                enemyAngle == getRelativeDirection(7)) {
            glancingRam(target,1);
        } else if (Mathf.Abs(this.front - target.front) == 4) {
            headOnRam(target);
        } else {
            glancingRam(target,0);
        }

    }

    private void broadsideRam(Ship target) {
        DebugControl.log("ramming","broadside ram");
        //target.life -= momentum * 2;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        AnimationManager.addRamming(this,target,momentum * 2);
    }

    private void headOnRam(Ship target) {
        DebugControl.log("ramming","head on ram");
        //target.life -= momentum;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        if (!target.movedForward) {
            this.life--;
        }
        AnimationManager.addRamming(this,target,momentum);
    }

    private void glancingRam(Ship target,int relativeTurn) {
        DebugControl.log("ramming","glancing ram");
        //target.life -= momentum;
        target.frontAfterCollision = target.getRelativeDirection(relativeTurn);        
        if (!target.movedForward && this.front != target.front) {
            this.frontAfterCollision = this.getRelativeDirection(-relativeTurn);
            this.life--;
        }
        AnimationManager.addRamming(this,target,momentum);
    }

    public void setAI(TrierisAI ai) {
        this.ai = ai;
    }

    public TrierisAI getAI() {
        return ai;
    }

    /*
    public static void main(String[] args) {

        Node node1 = new Node(1, 1);
        Node node2 = new Node(1, 2);
        Node node3 = new Node(1, 2);
        Node node4 = new Node(1, 2);
        node1.setAdjacentNode(3, node3);
        node2.setAdjacentNode(5, node3);
        node3.setAdjacentNode(4, node4);

        Ship ship1 = new Ship(TeamColor.ORANGE, 1, node1);
        Ship ship2 = new Ship(TeamColor.BLACK, 1, node3);
        ship1.setFront(3);
        ship2.setFront(7);

        ArrayList<Ship> ships = new ArrayList<Ship>();
        ships.add(ship1);
        ships.add(ship2);
        Controller controller = new Controller(ships);

        try {
            ship1.setAction(0, 1, -1);
            ship2.setAction(0, 4, -1);
            ship1.setAction(1, 4, -1);
            ship2.setAction(1, 4, -1);
            ship1.setAction(2, 4, -1);
            ship2.setAction(2, 4, -1);
            ship1.setAction(3, 4, -1);
            ship2.setAction(3, 4, -1);

            controller.executeTurn();

        } catch (Exception e) {
            e.printStackTrace();
        }

        System.out.println(1);
        ship1.printNodeShips(node1);
        System.out.println(2);
        ship1.printNodeShips(node2);
        System.out.println(3);
        ship1.printNodeShips(node3);
        System.out.println(4);
        ship1.printNodeShips(node4);

    }
    */

    private void printNodeShips(Node node) {
        foreach (Ship ship in node.getShips()) {
            //System.out.println(ship.getTeamColor());
            //System.out.println("Front" + ship.getFront());
            //System.out.println("Life" + ship.getLife());
            //System.out.println();
        }
    }

    public string toString() {
        return team.ToString() + " Ship " + id;
    }

    // NEW STUFF

    

    private void Start() {
        underlay = transform.GetChild(0).GetComponent<SpriteRenderer>();
        underlay.color = Color.clear;
        needRedirect = true;
    }
    
    private void Update() {
        //if (playingAnimation) {
        //    playingAnimation = animate();
        //}

        scaleToCamera();

        //chooseDirection();

        if(team == GameManager.main.playerTeam)
        {
            chooseDirection();
        }

    canHold();
    }

    public void scaleToCamera()
    {
        float currentSize = (Camera.main.orthographicSize / 3) - 1F;
        float maxUISize = 1.25F;
        if (currentSize > maxUISize)
            currentSize = maxUISize;
        else if (currentSize < 1)
            currentSize = 1F;

        transform.Find("ShipUI/Direction").gameObject.transform.localScale = new Vector3(currentSize, currentSize, 0);
        transform.Find("ShipUI/Controls").gameObject.transform.localScale = new Vector3(currentSize, currentSize, 0);

        float btnCurSize = 1F / 4;
        if (currentSize > 1F)
            btnCurSize = 1F/4 + (currentSize-1)/4;

        for (int i = 0; i < transform.Find("ShipUI/Direction").childCount; i++)
        {
            transform.Find("ShipUI/Direction").GetChild(i).gameObject.transform.localScale = new Vector3(btnCurSize, btnCurSize, 0);
        }

        for (int i = 0; i < transform.Find("ShipUI/Controls").childCount; i++)
        {
            transform.Find("ShipUI/Controls").GetChild(i).gameObject.transform.localScale = new Vector3(btnCurSize, btnCurSize, 0);
        }
    }

    public void chooseDirection()
    {
        if (needRedirect)
        {
            transform.Find("ShipUI/Direction").gameObject.SetActive(true);
        }
    }

    public void canHold()
    {
    }

    //private void startAnimation(animateDelegate a) {
    //    Debug.Log("Begin animation");
    //    animationStart = Time.time;
    //    animate = a;
    //    playingAnimation = true;
    //}

    //private void startAnimationCo(IEnumerator e) {
    //    animationStart = Time.time;
    //    playingAnimation = true;
    //    StartCoroutine(e);
    //}

    //private bool forwardsAnimate() {
    //    transform.position = Vector3.Lerp(startNode.getRealPos(),endNode.getRealPos(),Time.time - animationStart);
    //    if(Time.time - animationStart > animationSpeed) {
    //        Debug.Log("animation stop");
    //        return false;
    //    }
    //    return true;
    //}


    IEnumerator animateForwardsCo(Vector3 startPos, Vector3 endPos) {

        while(Time.time - animationStart < animationSpeed) {
            transform.position = Vector3.Lerp(startPos,endPos,(Time.time - animationStart)/animationSpeed);
            //Debug.Log("animating...");
            yield return null;
        }
        DebugControl.log("animation","animation stop");
        //playingAnimation = false;

        yield return null;
    }

    IEnumerator animateRotate(Quaternion start, Quaternion end) {
        while (Time.time - animationStart < animationSpeed) {
            transform.rotation = Quaternion.Lerp(start,end,(Time.time - animationStart)/animationSpeed);
            yield return null;
        }
        DebugControl.log("animation","animation stop");
        //playingAnimation = false;

    }

    public void setSpriteRotation() {
        transform.eulerAngles = new Vector3(0,0,(front)*-45);
    }

    public void redirect(int newDirection) {
        setDirection(newDirection);
        needRedirect = false;
        transform.Find("ShipUI/Direction").gameObject.SetActive(false);
    }

    private void setDirection(int newDirection) {
        front = newDirection;
        setSpriteRotation();
    }

    public void underlayUpdate(Ship hover,Ship selected) {
        Color c;
        
        if (needRedirect && (selected == this || hover == this)) {
            c = Color.red;
        } else if (needRedirect) {
            c = new Color(1,0,0,0.6f);
        } else if(selected == this){
            c = Color.green;
        } else if (hover == this) {
            c = new Color(0,1,0,0.5f);
        } else {
            c = Color.clear;
        }
        underlay.color = c;
    }

    public void shipUIOn()
    {
        transform.Find("ShipUI").gameObject.SetActive(true);
    }

    public void shipUIOff()
    {
        transform.Find("ShipUI").gameObject.SetActive(false);
    }

    private void OnDrawGizmos() {
        
    }

    // Initiates Combat Text
    void InitCBT(string text)
    {
        GameObject temp = Instantiate(CBTprefab) as GameObject;
        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(transform.Find("ShipUI"));
        tempRect.transform.localPosition = CBTprefab.transform.localPosition;
        tempRect.transform.localScale = CBTprefab.transform.localScale;
        tempRect.transform.localRotation = CBTprefab.transform.localRotation;

        temp.GetComponent<Text>().text = text;
        temp.GetComponent<Animator>().SetTrigger("Hit");
        Destroy(temp.gameObject, 2);
    }
}
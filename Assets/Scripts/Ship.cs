using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public Action[] actions;
    private Node node;    

    private int momentum = 0;
    private int portRepairCount = 0;
    private bool movedForward { get { return movedForwardField; } set {
            movedForwardField = value;
            if(team.getTeamType() == Team.Type.green && id == 3) {
                ;
            }
        }
    }

    private bool movedForwardField = false;

    public int fireDirection = -1;

    // ship front means ship direction
    private int front = 4;
    private bool canAct = true;
    public bool canActAfterCollision = true;
    private int frontAfterCollision = -1;
    private int id = -1;
    TrierisAI ai = null;

    // new variables

    //public bool crashed;
    public bool needRedirect = true;

    public bool needCaptureChoice;

    public bool needRammingChoice;

    public bool needCatapultChoice;

    public Image icon;

    private float animationStart;
    //SpriteRenderer underlay;
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
    public int catapultIndex; 

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
                if(lifeValue == 0) {
                    Sounds.main.playClip(Sounds.main.LongRip);
                }
            }
            else
            {
                lifeValue = value;
            }
            setHealthBar();
        }
    }

    private GameObject redirectUI;

    private GameObject redirectNotification;

    private GameObject directionLabel;


    public void intialize(Team team,Node node) {
        this.team = team;
        team.ships.Add(this);
        this.id = team.shipIdCounter++;
        this.life = MAX_HEALTH;
        
        this.node = node;
        node.getShips().Add(this);
        if (node.getPort() != null) {
            node.getPort().setTransparency();
        }
        gameObject.transform.position = node.getRealPos();

        actions = new Action[4];
        setSpriteRotation();

        populateDefaultActions();

        this.transform.Find("ShipSprite").GetComponent<SpriteRenderer>().sprite = team.ShipSprite;
        transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = team.getColorLight();
        currentActionIndex = 0;
        setHealthBar();
        catapultIndex = -1;

        icon = transform.Find("ShipUI/NonRotation/Icon").GetComponent<Image>();
        icon.GetComponentInChildren<Text>().gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
        
        redirectNotification = transform.Find("ShipUI/RedirectNotification").gameObject;
        redirectUI = transform.Find("ShipUI/Direction").gameObject;
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
        actions[index].act();
    }

    public  abstract class Action {
        public int catapultDirection = -1;
        public bool reverseReady = false;
        public int actionIndex;
        // since java inner classes are different than c sharp nested classes
        // we need a way for these nested classes to communicate with their ship
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

        public void setCatapult(int i)
        {
            catapultDirection = i;
        } 

    }

    public class ForwardAction : Action {

        public ForwardAction(int catapultDirection, Ship ship, int index) : base(catapultDirection,ship,index) {

        }

        protected override void affectShip() {           // throws ShipCrashedException 
            ship.speedup();
            ship.move(ship.front);
            
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

    public class EmptyAction : Action
    {

        public EmptyAction(int catapultDirection, Ship ship, int index) : base(catapultDirection, ship, index)
        {
            reverseReady = true;
        }

        protected override void affectShip()
        {
            ship.hold();
            if (catapultDirection == -1)
            {
                ship.repair();
            }
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

            ramDamageAndAngle(target);
            canAct = canActAfterCollision;
            momentum = 0;
        }
    }

    public void catapult(Ship target) {
        if (target != null) {
            //target.life -= 1;
            //Debug.Log(team.ToString() + getNumeralID() + " shot " + target.team.ToString() + target.getNumeralID());
            PhaseManager.addCatapultAnimation(this,target);
        }
    }

    public void reset() {
        //Debug.Log("resetting...");
        canAct = true;
        portRepairCount = 0;
        //momentum = 0;
        movedForward = false;
        frontAfterCollision = -1;
        canActAfterCollision = true;
        fireDirection = -1;
        //for (int i = 0; i < 4; i++) {
            //actions[i] = null;
        //}
    }

    public void move(int direction) {                           //throws ShipCrashedException 

        DebugControl.log("action","----moving ship");
        Node destNode = node.getAdjacentNode(direction);
        if (destNode == null) {
            life--;
            canAct = false;
            canActAfterCollision = false;
            Debug.Log("----Ship crashed");
            needRedirect = true;
            //redirectUI.SetActive(true);
            return;
        }
       

        Node startNode = node;

        node.getShips().Remove(this);
        node = destNode;
        node.getShips().Add(this);
        //if (PhaseManager.actionAnimations.ContainsKey(this)) {
        //    ;
        //}
        bool reverse = direction == getRelativeDirection(-4);

        PhaseManager.actionAnimations.Add(this,new MovementAnimation(startNode,destNode,this,momentum,reverse));
        
    }


    public void turn(int relativeDirection) {
        front = getRelativeDirection(relativeDirection);
        momentum = 0;
        bool portTurn = (relativeDirection == -1);
        PhaseManager.actionAnimations.Add(this,new RotationAnimation(this.transform.rotation,this.transform.rotation * Quaternion.Euler(0,0,-45 * relativeDirection),this,portTurn));

        movedForward = false;
    }

    public void setFront(int direction) {
        front = direction % 8;
    }

    public void hold() {
        momentum = 0;
        movedForward = false;
    }

    public void speedup() {
        DebugControl.log("movement","Speeding up...");
        momentum++;
        movedForward = true;
    }

    public void repair() {
        if (node.getPort() != null && node.getPort().getTeam() == team) {
            if (node.getPort().getCapital() && node.getPort().getTeam() == team) {
                if (life < MAX_HEALTH)
                    life++;
            } else {
                portRepairCount++;
                if (portRepairCount == life) {
                    if (life < MAX_HEALTH)
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
        if(this.team == GameManager.main.playerTeam)
        {
            Button st = GameManager.main.uiControl.ShipTabs[this.getID()].GetComponent<Button>();

            ColorBlock cb = st.colors;
            cb.normalColor = CustomColor.TeamBlack;
            st.colors = cb;

            Text tt = GameManager.main.uiControl.Tabtexts[this.getID()].GetComponent<Text>();

            tt.color = CustomColor.TeamBlack;

            st.interactable = false;
        }
        PhaseManager.sinkAnimations.Add(new SinkAnimation(this));
        //node = null;
        team.ships.Remove(this);

        if(this == GameManager.main.uiControl.Selected)
        {
            GameManager.main.uiControl.setSelection(GameManager.main.getPlayerShips()[0].getID());
        }

        Destroy(this.gameObject);
    }

    public void capturePort() {
        
        canActAfterCollision = false;
        canAct = false;
        movedForward = false;
        momentum = 0;
        if(team.getTeamType() == GameManager.main.playerTeam.getTeamType()) {
            //GameManager.main.uiControl.updatePlayerScore();
        }
        PhaseManager.addCaptureAnimation(this);
    }
    
    public void playerCapture()
    {
        needCaptureChoice = false;
        canActAfterCollision = false;
        canAct = false;
        movedForward = false;
        needRedirect = true;
        portRepairCount = -5;
        //setRedirectUI(true);
        activateRedirectNotification();
    }

    public void updateFrontAfterCollision() {
        movedForward = false;
        if (frontAfterCollision != -1) {
            front = frontAfterCollision;
        }
        canAct = canActAfterCollision;
        frontAfterCollision = -1;
    }

    public void populateDefaultActions() {
        for (int i = 0; i < MAX_HEALTH; i++) {
            if (actions[i] == null) {
                actions[i] = new EmptyAction(-1,this,6);
            }
        }
    }

    private Action getAction(int actionNum,int firingDirection) {       // throws InvalidActionException
        switch (actionNum) {
            case 1:
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
        Debug.Log(name + " rammed " + target.name);
        disableCatapults(target);
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

    private void disableCatapults(Ship target)
    {
        foreach(Action action in actions)
            action.setCatapult(-1);
        foreach (Action action in target.actions)
            action.setCatapult(-1);
    }

    private void broadsideRam(Ship target) {
        DebugControl.log("ramming","broadside ram");
        //target.life -= momentum * 2;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        PhaseManager.addRammingResolution(this,target,momentum * 2);
    }

    private void headOnRam(Ship target) {
        DebugControl.log("ramming","head on ram");
        //target.life -= momentum;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        int dmgToSelf = target.movedForward ? 0 : 1;
        //if (!target.movedForward) {
        //    this.life--;
        //}
        PhaseManager.addRammingResolution(this,target,momentum,dmgToSelf);
    }

    private void glancingRam(Ship target,int relativeTurn) {
        DebugControl.log("ramming","glancing ram");
        //target.life -= momentum;
        target.frontAfterCollision = target.getRelativeDirection(relativeTurn);
        int dmgToSelf = 0;
        if (!target.movedForward && this.front != target.front) {
            this.frontAfterCollision = this.getRelativeDirection(-relativeTurn);
            dmgToSelf = 1;
        }
        PhaseManager.addRammingResolution(this,target,momentum,dmgToSelf);
        //PhaseManager.addRamming(this,target,momentum);
        //addRammingAnimation(target,momentum);
    }
    //void addRammingAnimation(Ship target, int dmg) {
    //    PhaseManager.involvedInRamming.Add(this);
    //    PhaseManager.involvedInRamming.Add(target);
    //    PhaseManager.addRamming(this,target,dmg);
    //}

    public void setAI(TrierisAI ai) {
        this.ai = ai;
    }

    public TrierisAI getAI() {
        return ai;
    }

    public string toString() {
        return team.ToString() + " Ship " + id;
    }

    private void Start() {
        
    }
    
    private void Update() {  

        //scaleToCamera();

        CheckUnfocus();

        //canHold();
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
            transform.Find("ShipUI/RedirectNotification").gameObject.SetActive(true);
        }
    }

    //public void canHold()
    //{
    //}    

    public void setSpriteRotation() {
        transform.eulerAngles = new Vector3(0,0,(front)*-45);
    }

    public void redirect(int newDirection) {
        setDirection(newDirection);
        needRedirect = false;
        Destroy(directionLabel);
        redirectUI.SetActive(false);
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
        //underlay.color = c;
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
        
        Handles.color = Color.magenta;

        if (needRedirect) {
            Handles.Label(transform.position + new Vector3(0,-0.25f),"need redirect");
        }

        if (canAct)
        {
            Handles.Label(transform.position + new Vector3(0, -0.5f), "Can Act");

            //Gizmos.DrawIcon(transform.position + new Vector3(-0.25f,0),"needRedirect.png",true);
        }

        if (needCaptureChoice) {
            Handles.Label(transform.position + new Vector3(0,0.0f),"need capture");
        }

        if (needRammingChoice) {
            Handles.Label(transform.position + new Vector3(0,0.25f),"need ramming");
        }

        if (needCatapultChoice) {
            Handles.Label(transform.position + new Vector3(0,0.5f),"need catapult");
        }       
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

    void setHealthBar() {
        GameObject healthBar = transform.Find("ShipUI/NonRotation/NewHealthBar").gameObject;
        Image[] healthBoxes = healthBar.transform.GetComponentsInChildren<Image>();
        for(int i = 1; i <= 4; i++) {
            healthBoxes[i].color = (i <= life) ? team.getColorLight() : Color.black;
        }
    }

    public void setRedirectUI(bool b) {
        redirectNotification.SetActive(b);
    }

    public void repositionIcon()
    {
        if (redirectNotification.activeSelf)
            icon.gameObject.transform.position = new Vector2(Position.x, Position.y + 0.6f);
        else
            icon.gameObject.transform.position = new Vector2(Position.x, Position.y + 0.486f);
    }

    public void setIconString(String s) {
        icon.gameObject.SetActive(true);
        icon.GetComponentInChildren<Text>(true).gameObject.SetActive(true);
        icon.sprite = Sprites.main.EmtpyIcon;
        icon.color = team.getColorLight();
        icon.GetComponentInChildren<Text>().text = s;
    }

    public void setIcon(Sprite sprite) {
        icon.color = Color.white;
        icon.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        icon.gameObject.SetActive(true);
        icon.sprite = sprite;        
    }

    public void disableIcon() {
        if(UIControl.main.Selected == this) {
            setIconString(getNumeralID());
        } else {
            icon.gameObject.SetActive(false);
            icon.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        }        
    }

    public void selectThisShip()
    {
        GameObject.Find("GameManager").GetComponent<UIControl>().Selected = this;
    }

    public string getNumeralID() {
        return getNumeral(this.id + 1);
    }

    public static string getNumeral(int i) {
        switch (i) {
            case 1:
            return "I";
            case 2:
            return "II"; 
            case 3:
            return "III"; 
            case 4:
            return "IV"; 
            case 5:
            return "V"; 
            default:
            return null;
        }
    }
    
    private void CheckUnfocus()
    {
        if (Input.GetMouseButton(0) && redirectUI.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(redirectUI.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            activateRedirectNotification();
        }
    }

    public void activateRedirectNotification()
    {
        Destroy(directionLabel);
        redirectNotification.SetActive(true);
        redirectUI.SetActive(false);
    }

    public void activateRedirectPanel()
    {
        icon.gameObject.SetActive(false);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/ChooseText");
        directionLabel = GameObject.Instantiate(prefab, new Vector2(Position.x, Position.y + 1f), Quaternion.identity);
        directionLabel.GetComponentInChildren<Text>().text = "Direction";
        directionLabel.GetComponent<Canvas>().sortingOrder = 10;

        redirectUI.SetActive(true);
        redirectNotification.SetActive(false);
    }
}
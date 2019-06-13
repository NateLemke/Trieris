using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to control and calculate actions for a single Trieris ship
/// </summary>
public class Ship : MonoBehaviour {

    public const int FORWARD = 1;
    public const int PORT = 2;
    public const int STARBOARD = 3;
    public const int HOLD = 4;
    public const int REVERSE = 5;

    public const int MAX_HEALTH = 4;

    public int currentActionIndex;
    public int catapultIndex;
    public int catapultDirection;
    

    public bool CanFire { get { return canFire; } set { canFire = value; } }
    bool canFire;

    // used to display damage text
    public GameObject CBTprefab;

    public Action[] actions;
    private Node node;    

    public int momentum { get; set; }

    private int portRepairCount = 0;
    private bool movedForward = false;

    public int fireDirection = -1;

    // ship front means ship direction
    private int front = 4;
    private bool canAct = true;
    public bool canActAfterCollision = true;
    private int frontAfterCollision = -1;
    public int Id { get { return id; } }
    private int id = -1;
    public TrierisAI Ai { get; set; }

    public bool needRedirect = true;
    public bool needCaptureChoice;
    public bool needRammingChoice;
    public bool needCatapultChoice;

    public Image icon;

    public Team team;

    // the ship's position in the world (not the game board)
    public Vector3 Position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }   

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
                if(lifeValue <= 0) {
                    Sounds.main.playClip(Sounds.main.LongRip, 0.8f);
                }
            }
            else
            {
                lifeValue = value;
            }
            setHealthBar();
        }
    }

    // references to the ship's redirect UI
    private GameObject redirectUI;
    private GameObject redirectNotification;
    private GameObject directionLabel;

    /// <summary>
    /// Used to initialze the ship upon instantiation
    /// Sets the ship's team, node, position, health bar, and grabs references to UI/sprites
    /// </summary>
    /// <param name="team"></param>
    /// <param name="node"></param>
    public void intialize(Team team,Node node) {
        this.team = team;
        team.ships.Add(this);
        id = team.shipIdCounter++;
        life = MAX_HEALTH;

        catapultIndex = -1;
        catapultDirection = -1;
        canFire = true;

        this.node = node;
        node.Ships.Add(this);
        if (node.Port != null) {
            node.Port.setTransparency();
        }
        gameObject.transform.position = node.getRealPos();

        actions = new Action[4];
        setSpriteRotation();

        populateDefaultActions();

        this.transform.Find("ShipSprite").GetComponent<SpriteRenderer>().sprite = team.ShipSprite;
        transform.Find("MinimapSprite").GetComponent<SpriteRenderer>().color = team.getColorLight();
        currentActionIndex = 0;
        setHealthBar();

        icon = transform.Find("ShipUI/NonRotation/Icon").GetComponent<Image>();
        icon.GetComponentInChildren<Text>().gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
        
        redirectNotification = transform.Find("ShipUI/RedirectNotification").gameObject;
        redirectUI = transform.Find("ShipUI/Direction").gameObject;
    }

    /// <summary>
    /// Adds an action for the ship at the given index
    /// </summary>
    /// <param name="index">the phase for the action</param>
    /// <param name="actionNum">the action number the determines the type of action</param>
    /// <param name="firingDirection">whether or not the ship is firing this turn, and in which direction</param>
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

    /// <summary>
    /// Calculates the ship's action for the given phase
    /// </summary>
    /// <param name="index"></param>
    public void doAction(int index) {      //throws ShipCrashedException
        actions[index].act();
    }

    /// <summary>
    /// Abstract classed used for different action
    /// ALSO STORES CATAPULT SHOOTING DIRECTION FOR THE PHASE
    /// </summary>
    public  abstract class Action {
        public int catapultDirection = -1;
        public bool reverseReady = false;
        public int actionIndex;
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

    /// <summary>
    /// Action for forwards movement
    /// </summary>
    public class ForwardAction : Action {

        public ForwardAction(int catapultDirection, Ship ship, int index) : base(catapultDirection,ship,index) {

        }

        protected override void affectShip() {           // throws ShipCrashedException 
            ship.speedup();
            ship.move(ship.front);
            
        }
    }

    /// <summary>
    /// Action for reversing
    /// </summary>
    public class ReverseAction : Action {

        public ReverseAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {

        }

        protected override void affectShip() {              //throws ShipCrashedException
            ship.move(ship.getRelativeDirection(-4));
        }
    }

    /// <summary>
    /// Action for holding
    /// </summary>
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

    /// <summary>
    /// Action for a starboard turn
    /// </summary>
    public class StarboardAction : Action {

        public StarboardAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {
            
        }

        protected override void affectShip() {
            ship.turn(1);
        }
    }

    /// <summary>
    /// Action for a port turn
    /// </summary>
    public class PortAction : Action {

        public PortAction(int catapultDirection, Ship ship,int index) : base(catapultDirection,ship,index) {
            
        }

        protected override void affectShip() {
            ship.turn(-1);
        }
    }

    /// <summary>
    /// Default action for an action that has not yet been set, counts as a hold
    /// </summary>
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

    /// <summary>
    /// Gets the node that the ship is aiming at, returns null if no such node exists
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Returns true if the ship has no null actions in their action list
    /// </summary>
    /// <returns></returns>
    public bool ready() {
        foreach (Action action in actions) {
            if (action == null) {
                Debug.Log("ship not ready");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Calls the required functions to calcuated a ram against the given ship
    /// </summary>
    /// <param name="target">the target that this ship is ramming</param>
    public void ram(Ship target) {
        if (target != null) {
            CanFire = false;
            target.CanFire = false;
            ramDamageAndAngle(target);
            canAct = canActAfterCollision;
            momentum = 0;
        }
    }

    /// <summary>
    /// Adds a catapult animation targetting the given ship
    /// </summary>
    /// <param name="target"></param>
    public void catapult(Ship target) {
        if (target != null) {
            PhaseManager.addCatapultAnimation(this,target);
        }
    }

    /// <summary>
    /// Resets turn-specific variables for the next turn
    /// </summary>
    public void reset() {
        canAct = true;
        portRepairCount = 0;
        canFire = true;
        movedForward = false;
        frontAfterCollision = -1;
        canActAfterCollision = true;
        fireDirection = -1;
    }

    /// <summary>
    /// Moves in the given direction (should be forwards or backwards)
    /// </summary>
    /// <param name="direction">the direction to move</param>
    public void move(int direction) {                           //throws ShipCrashedException 

        DebugControl.log("action","----moving ship");
        Node destNode = node.getAdjacentNode(direction);
        if (destNode == null) {
            life--;
            canAct = false;
            canActAfterCollision = false;
            //Debug.Log("----Ship crashed");
            needRedirect = true;
            movedForward = false;
            momentum = 0;
            if(team == GameManager.playerTeam)
                activateRedirectNotification();
            return;
        }
       

        Node startNode = node;

        node.Ships.Remove(this);
        node = destNode;
        node.Ships.Add(this);
        bool reverse = direction == getRelativeDirection(-4);

        PhaseManager.actionAnimations.Add(this,new MovementAnimation(startNode,destNode,this,momentum,reverse));
        
    }

    /// <summary>
    /// Turns the ship in the given direction
    /// </summary>
    /// <param name="relativeDirection">the direction to turn the ship</param>
    public void turn(int relativeDirection) {
        front = getRelativeDirection(relativeDirection);
        momentum = 0;
        bool portTurn = (relativeDirection == -1);
        if (PhaseManager.actionAnimations.ContainsKey(this)) {
            ;
        }

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

    /// <summary>
    /// Repairs the current ship IF they are on a node that contains a port belonging to their team
    /// </summary>
    public void repair() {
        if (node.Port != null && node.Port.Team == team) {
            if (node.Port.IsCapital && node.Port.Team == team) {
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

    /// <summary>
    /// Adds an animation to sink this ship
    /// </summary>
    public void sink() {
        if (node != null) {
            node.Ships.Remove(this);
        }
        if(this.team == GameManager.playerTeam)
        {
            Button st = GameManager.main.uiControl.ShipTabs[this.Id].GetComponent<Button>();

            ColorBlock cb = st.colors;
            cb.normalColor = CustomColor.TeamBlack;
            st.colors = cb;

            Text tt = GameManager.main.uiControl.Tabtexts[this.Id].GetComponent<Text>();

            tt.color = CustomColor.TeamBlack;

            st.interactable = false;
        }
        //PhaseManager.sinkAnimations.Add(new SinkAnimation(this));
        //node = null;
        team.ships.Remove(this);

        if(this == GameManager.main.uiControl.Selected)
        {
            GameManager.main.uiControl.setSelection(GameManager.main.getPlayerShips()[0].Id);
        }

        if (this.getNode().Ships.Contains(this))
        {
            this.getNode().Ships.Remove(this);
        }

        if (this.getNode().Port != null)
        {
            this.getNode().Port.setTransparency();
        }

        foreach (Ship s in this.getNode().Ships)
        {
            s.updateNodePos();
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Adds an animation for this ship to capture the port they are on
    /// Sets ship stats according to capture rules
    /// </summary>
    public void capturePort() {
        
        canActAfterCollision = false;
        canAct = false;
        movedForward = false;
        momentum = 0;
        PhaseManager.addCaptureAnimation(this);
    }
    
    /// <summary>
    /// Used when the a player ship captures a port, activates port capture UI
    /// </summary>
    public void playerCapture()
    {
        needCaptureChoice = false;
        canActAfterCollision = false;
        canAct = false;
        movedForward = false;
        needRedirect = true;
        portRepairCount = -5;
        activateRedirectNotification();
    }

    /// <summary>
    /// Sets the ships front facing and canAct, according to if they have collided this turn
    /// </summary>
    public void updateFrontAfterCollision() {
        movedForward = false;
        if (frontAfterCollision != -1) {
            front = frontAfterCollision;
        }
        canAct = canActAfterCollision;
        frontAfterCollision = -1;
    }

    /// <summary>
    /// sets the all actions for this ship to be default action
    /// </summary>
    public void populateDefaultActions() {
        for (int i = 0; i < MAX_HEALTH; i++) {
            if (actions[i] == null) {
                actions[i] = new EmptyAction(-1,this,6);
            }
        }
    }

    /// <summary>
    /// Returns an instance of an action class depending upon the actionNum
    /// </summary>
    /// <param name="actionNum">the action class's action number</param>
    /// <param name="firingDirection">the firing direction for the catapult for this phase</param>
    /// <returns>An action class</returns>
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

    /// <summary>
    /// Returns the relavtive direction between this ship and the given direction
    /// </summary>
    /// <param name="relativeDelta">direction to compare to this ship</param>
    /// <returns>the relative direction as an int</returns>
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

    /// <summary>
    /// Calculates the proper ramming type between this ship and its target and executes its function (glancing, broadside, headon)
    /// </summary>
    /// <param name="target">the ramming target of this ship</param>
    private void ramDamageAndAngle(Ship target) {
        int enemyAngle = target.front;
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

    /// <summary>
    /// Disables catapults for the current turn for both this ship and its target
    /// </summary>
    /// <param name="target">the target to also disable catapults this turn</param>
    private void disableCatapults(Ship target)
    {
        foreach(Action action in actions)
            action.setCatapult(-1);
        foreach (Action action in target.actions)
            action.setCatapult(-1);
    }

    /// <summary>
    /// Calculates a broadside ram between this ship and its target. Adds a pending ramming resolution to the phase manager.
    /// </summary>
    /// <param name="target"></param>
    private void broadsideRam(Ship target) {
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        PhaseManager.addRammingResolution(this,target,momentum * 2);
    }

    /// <summary>
    /// Checks if the ramming between this ship and its target is an adjacent head on ram
    /// </summary>
    /// <param name="s">this ships target</param>
    /// <param name="phase">the phase to check for</param>
    /// <returns>True if the two ships are in adajcent nodes and moving towards each other</returns>
    public bool AdjHeadOnRamCheck(Ship s, int phase) {
        return (Mathf.Abs(getFront() - s.getFront()) == 4 && s.actions[phase].GetType().Name == "ForwardAction" && actions[phase].GetType().Name == "ForwardAction");            
    }

    /// <summary>
    /// Calculates a head on ram. Adds a pending ramming resolution to the phase manager.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    private void headOnRam(Ship target) {
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        int dmgToSelf = target.movedForward ? 0 : 1;
        if (AdjHeadOnRamCheck(target,GameLogic.phaseIndex)) {
            movedForward = false;
            PhaseManager.addAdjHeadOnRamming(this,target);
        } else {
            PhaseManager.addRammingResolution(this,target,momentum,dmgToSelf);
        }        
    }

    /// <summary>
    /// Calculates a glancing ram. Adds a ramming resolution to the phase manager.
    /// </summary>
    /// <param name="target">the ship being rammed by this ship</param>
    /// <param name="relativeTurn">the relative rotation</param>
    private void glancingRam(Ship target,int relativeTurn) {

        target.frontAfterCollision = target.getRelativeDirection(relativeTurn);
        int dmgToSelf = 0;
        if (!target.movedForward && this.front != target.front) {
            this.frontAfterCollision = this.getRelativeDirection(-relativeTurn);
            dmgToSelf = 1;
        }
        PhaseManager.addRammingResolution(this,target,momentum,dmgToSelf);

    }

    /// <summary>
    /// Returns this ship's team and ID as a string
    /// </summary>
    /// <returns></returns>
    public string toString() {
        return team.ToString() + " Ship " + id;
    }
    
    /// <summary>
    /// Checks if the player has selected another redirect UI this frame
    /// </summary>
    private void Update() {
        CheckUnfocus();
    }

    /// <summary>
    /// Scales the ship's redirect UI to the camera
    /// </summary>
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

   /// <summary>
   /// Disables this ship's redirect UI
   /// </summary>
    public void chooseDirection()
    {
        if (needRedirect)
        {
            transform.Find("ShipUI/RedirectNotification").gameObject.SetActive(true);
        }
    }
 
    /// <summary>
    /// Sets the rotation of the ships sprite according to its front facing
    /// </summary>
    public void setSpriteRotation() {
        transform.eulerAngles = new Vector3(0,0,(front)*-45);
    }

    /// <summary>
    /// Sets the ship's direction to a new facing
    /// </summary>
    /// <param name="newDirection">the new direction for this ship to face</param>
    public void redirect(int newDirection) {
        setDirection(newDirection);
        needRedirect = false;
        Destroy(directionLabel);
        redirectUI.SetActive(false);
    }

    /// <summary>
    /// Sets this ship's facing
    /// </summary>
    /// <param name="newDirection">the new direction for this ship to face</param>
    private void setDirection(int newDirection) {
        front = newDirection;
        setSpriteRotation();
    }

     /// <summary>
     /// enables this ship's UI
     /// </summary>
    public void shipUIOn()
    {
        transform.Find("ShipUI").gameObject.SetActive(true);
    }

    /// <summary>
    /// disables this ship's UI
    /// </summary>
    public void shipUIOff()
    {
        transform.Find("ShipUI").gameObject.SetActive(false);
    }

    /// <summary>
    /// Draws debug gizmos for this ship
    /// </summary>
    private void OnDrawGizmos() {

        Handles.color = Color.magenta;

        if (needRedirect) {
            Handles.Label(transform.position + new Vector3(0,-0.25f),"need redirect");
        }

        if (!canAct) {
            Handles.Label(transform.position + new Vector3(0,-0.5f),"cannot act");
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

    /// <summary>
    /// Initiates the combat damage text for this ship
    /// </summary>
    /// <param name="text">the text to display</param>
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

    /// <summary>
    /// Updates this ship's health bar
    /// </summary>
    void setHealthBar() {
        GameObject healthBar = transform.Find("ShipUI/NonRotation/NewHealthBar").gameObject;
        Image[] healthBoxes = healthBar.transform.GetComponentsInChildren<Image>();
        for(int i = 1; i <= 4; i++) {
            healthBoxes[i].color = (i <= life) ? team.getColorLight() : Color.black;
        }
    }

    /// <summary>
    /// Activates this ship's redirect UI
    /// </summary>
    /// <param name="b"></param>
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

    /// <summary>
    /// Activates the ship's icon with the given string, used to show ship ID when the ship is selected
    /// </summary>
    /// <param name="s">the string to display</param>
    public void setIconString(String s) {
        icon.gameObject.SetActive(true);
        icon.GetComponentInChildren<Text>(true).gameObject.SetActive(true);
        icon.sprite = Sprites.main.EmtpyIcon;
        icon.color = team.getColorLight();
        icon.GetComponentInChildren<Text>().text = s;
    }

    /// <summary>
    /// Activates the ship's icon and sets it to the given icon
    /// </summary>
    /// <param name="sprite">the sprite to get the ship's icon to</param>
    public void setIcon(Sprite sprite) {
        icon.color = Color.white;
        icon.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        icon.gameObject.SetActive(true);
        icon.sprite = sprite;        
    }

    /// <summary>
    /// Disables the ship's icon. Sets the icon to active if the ship is currently selected.
    /// </summary>
    public void disableIcon() {
        if(UIControl.main.Selected == this) {
            setIconString(getNumeralID());
        } else {
            icon.gameObject.SetActive(false);
            icon.GetComponentInChildren<Text>(true).gameObject.SetActive(false);
        }        
    }

    /// <summary>
    /// Sets this ship to be the currently selected ship
    /// </summary>
    public void selectThisShip()
    {
        GameObject.Find("GameManager").GetComponent<UIControl>().Selected = this;
    }

    /// <summary>
    /// Returns this ship's ID as a roman numeral string
    /// </summary>
    /// <returns></returns>
    public string getNumeralID() {
        return getNumeral(this.id + 1);
    }

    /// <summary>
    /// Returns a string of a roman numeral from 1 to 5
    /// </summary>
    /// <param name="i">the number to numeralize</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Checks if another ship has had its redirection UI enabled
    /// </summary>
    private void CheckUnfocus()
    {
        if (Input.GetMouseButton(0) && redirectUI.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(redirectUI.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            activateRedirectNotification();
        }
    }

    /// <summary>
    /// Activates the redirection notification UI
    /// </summary>
    public void activateRedirectNotification()
    {
        Destroy(directionLabel);
        redirectNotification.SetActive(true);
        redirectUI.SetActive(false);
    }

    /// <summary>
    /// Activates the redirection notification panel
    /// </summary>
    public void activateRedirectPanel()
    {
        icon.gameObject.SetActive(false);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/ChooseText");
        directionLabel = GameObject.Instantiate(prefab, new Vector2(Position.x, Position.y + 1f), Quaternion.identity);
        directionLabel.GetComponentInChildren<Text>().text = "Set Direction";
        directionLabel.GetComponent<Canvas>().sortingOrder = 8;

        redirectUI.SetActive(true);
        redirectNotification.SetActive(false);
    }

    /// <summary>
    /// Used to set the ship's position on a node, spacing it out with any other ships sharing the node
    /// </summary>
    public void updateNodePos(float xSpacing = Node.shipSpacingX, float ySpacing = Node.shipSpacingY) {
        Position = node.shipNodePos(this,xSpacing,ySpacing);
    }

    /// <summary>
    /// Used to get this ship's intended position on its node, without updating its actual position
    /// </summary>
    public Vector2 getNodePos() {
        return node.shipNodePos(this); 
    }

    public bool belongsToAI() {
        return this.team.aiTeam;
    }
}
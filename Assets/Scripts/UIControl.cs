using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the functions that are used by the UI to set ship actions, selected ship, attacks.
/// Contains functions that are used to update the UI to reflect changes in the game.
/// </summary>
public class UIControl : MonoBehaviour
{
    GameManager gameManager;
    GameLogic gameLogic;

    Text animationText;
    Text redirectText;

    GameObject debugMenu;
    //GameObject DevUI;
    GameObject LogToggle;
    GameObject phaseTracker;
    Text turnPhase;
    Text captureTracker;
    Text victoryTracker;
    GameObject redirectNotice;
    GameObject captureNotice;
    GameObject rammingNotice;
    GameObject catapultNotice;

    GameObject TeamSelectUI;

    Image[] actionImages = new Image[4];
    GameObject[] actionPanels = new GameObject[4];
    Button[] attackPanels = new Button[4];
    Button[] attackArrows = new Button[9];
    Button[] commandPanels = new Button[5];
    GameObject[] shipTabs = new GameObject[5];
    Text[] tabTexts = new Text[5];
    Image[,] bottomIcons = new Image[6, 5];
    Text[] portTexts = new Text[6];
    Text goText;
    Button goButton;

    Sprite straightArrow;
    Sprite curvedArrow;
    Sprite holdSprite;
    Sprite emptySprite;
    GameObject selection;

    Color defaultGreen;
    Color attackUnclicked;
    Color attackClicked;
    Color arrowYellow;
    Color greyedOut;

    bool controlsEnabled = true;

    public GameObject phaseAnnouncer;
    public GameObject phase;
    public GameObject subPhase;
    //public GameObject subPhaseProgress;

    public GameObject optionsPanel;
    public static UIControl main;

    public Ship Selected { get { return selected; } set { setSelection(value); } }
    private Ship selected;

    public GameObject[] ShipTabs { get { return shipTabs; } }

    public Text[] Tabtexts {  get { return tabTexts; } }

    public Text GoText {  get { return goText; } }

    public bool fadeObjective;

    private Color objectiveColor;


    // new multiplayer functions
    



    /// <summary>
    /// basic initialization.
    /// </summary>
    private void Awake()
    {
        main = this;
        gameManager = gameObject.GetComponent<GameManager>();
        gameLogic = gameObject.GetComponent<GameLogic>();
        optionsPanel = GameObject.Find("OverlayCanvas");
        optionsPanel = optionsPanel.transform.Find("OptionsMenu").gameObject;
    }

    /// <summary>
    /// Initializes UI elements, resources, colours, and notices.
    /// </summary>
    void Start()
    {

    }

    public void SetUpUI() {
        Selected = null;

        actionImages[0] = GameObject.Find("ActionImage1").GetComponent<Image>();
        actionImages[1] = GameObject.Find("ActionImage2").GetComponent<Image>();
        actionImages[2] = GameObject.Find("ActionImage3").GetComponent<Image>();
        actionImages[3] = GameObject.Find("ActionImage4").GetComponent<Image>();

        actionPanels[0] = GameObject.Find("PanelAction1");
        actionPanels[1] = GameObject.Find("PanelAction2");
        actionPanels[2] = GameObject.Find("PanelAction3");
        actionPanels[3] = GameObject.Find("PanelAction4");

        commandPanels[0] = GameObject.Find("PanelUp").GetComponent<Button>();
        commandPanels[1] = GameObject.Find("PanelHold").GetComponent<Button>();
        commandPanels[2] = GameObject.Find("PanelRev").GetComponent<Button>();
        commandPanels[3] = GameObject.Find("PanelTurnR").GetComponent<Button>();
        commandPanels[4] = GameObject.Find("PanelTurnL").GetComponent<Button>();

        attackPanels[0] = GameObject.Find("AttackCrosshair1").GetComponent<Button>();
        attackPanels[1] = GameObject.Find("AttackCrosshair2").GetComponent<Button>();
        attackPanels[2] = GameObject.Find("AttackCrosshair3").GetComponent<Button>();
        attackPanels[3] = GameObject.Find("AttackCrosshair4").GetComponent<Button>();

        attackArrows[0] = GameObject.Find("ArrowN").GetComponent<Button>();
        attackArrows[1] = GameObject.Find("ArrowNE").GetComponent<Button>();
        attackArrows[2] = GameObject.Find("ArrowE").GetComponent<Button>();
        attackArrows[3] = GameObject.Find("ArrowSE").GetComponent<Button>();
        attackArrows[4] = GameObject.Find("ArrowS").GetComponent<Button>();
        attackArrows[5] = GameObject.Find("ArrowSW").GetComponent<Button>();
        attackArrows[6] = GameObject.Find("ArrowW").GetComponent<Button>();
        attackArrows[7] = GameObject.Find("ArrowNW").GetComponent<Button>();
        attackArrows[8] = GameObject.Find("Middle").GetComponent<Button>();

        shipTabs[0] = GameObject.Find("ShipTab1");
        shipTabs[1] = GameObject.Find("ShipTab2");
        shipTabs[2] = GameObject.Find("ShipTab3");
        shipTabs[3] = GameObject.Find("ShipTab4");
        shipTabs[4] = GameObject.Find("ShipTab5");

        tabTexts[0] = GameObject.Find("TabText1").GetComponent<Text>();
        tabTexts[1] = GameObject.Find("TabText2").GetComponent<Text>();
        tabTexts[2] = GameObject.Find("TabText3").GetComponent<Text>();
        tabTexts[3] = GameObject.Find("TabText4").GetComponent<Text>();
        tabTexts[4] = GameObject.Find("TabText5").GetComponent<Text>();

        bottomIcons[0,0] = GameObject.Find("BottomRed1").GetComponent<Image>();
        bottomIcons[0,1] = GameObject.Find("BottomRed2").GetComponent<Image>();
        bottomIcons[0,2] = GameObject.Find("BottomRed3").GetComponent<Image>();
        bottomIcons[0,3] = GameObject.Find("BottomRed4").GetComponent<Image>();
        bottomIcons[0,4] = GameObject.Find("BottomRed5").GetComponent<Image>();

        bottomIcons[1,0] = GameObject.Find("BottomOrange1").GetComponent<Image>();
        bottomIcons[1,1] = GameObject.Find("BottomOrange2").GetComponent<Image>();
        bottomIcons[1,2] = GameObject.Find("BottomOrange3").GetComponent<Image>();
        bottomIcons[1,3] = GameObject.Find("BottomOrange4").GetComponent<Image>();
        bottomIcons[1,4] = GameObject.Find("BottomOrange5").GetComponent<Image>();

        bottomIcons[2,0] = GameObject.Find("BottomYellow1").GetComponent<Image>();
        bottomIcons[2,1] = GameObject.Find("BottomYellow2").GetComponent<Image>();
        bottomIcons[2,2] = GameObject.Find("BottomYellow3").GetComponent<Image>();
        bottomIcons[2,3] = GameObject.Find("BottomYellow4").GetComponent<Image>();
        bottomIcons[2,4] = GameObject.Find("BottomYellow5").GetComponent<Image>();

        bottomIcons[3,0] = GameObject.Find("BottomGreen1").GetComponent<Image>();
        bottomIcons[3,1] = GameObject.Find("BottomGreen2").GetComponent<Image>();
        bottomIcons[3,2] = GameObject.Find("BottomGreen3").GetComponent<Image>();
        bottomIcons[3,3] = GameObject.Find("BottomGreen4").GetComponent<Image>();
        bottomIcons[3,4] = GameObject.Find("BottomGreen5").GetComponent<Image>();

        bottomIcons[4,0] = GameObject.Find("BottomBlue1").GetComponent<Image>();
        bottomIcons[4,1] = GameObject.Find("BottomBlue2").GetComponent<Image>();
        bottomIcons[4,2] = GameObject.Find("BottomBlue3").GetComponent<Image>();
        bottomIcons[4,3] = GameObject.Find("BottomBlue4").GetComponent<Image>();
        bottomIcons[4,4] = GameObject.Find("BottomBlue5").GetComponent<Image>();

        bottomIcons[5,0] = GameObject.Find("BottomBlack1").GetComponent<Image>();
        bottomIcons[5,1] = GameObject.Find("BottomBlack2").GetComponent<Image>();
        bottomIcons[5,2] = GameObject.Find("BottomBlack3").GetComponent<Image>();
        bottomIcons[5,3] = GameObject.Find("BottomBlack4").GetComponent<Image>();
        bottomIcons[5,4] = GameObject.Find("BottomBlack5").GetComponent<Image>();

        portTexts[0] = GameObject.Find("BottomPortsRed").GetComponent<Text>();
        portTexts[1] = GameObject.Find("BottomPortsOrange").GetComponent<Text>();
        portTexts[2] = GameObject.Find("BottomPortsYellow").GetComponent<Text>();
        portTexts[3] = GameObject.Find("BottomPortsGreen").GetComponent<Text>();
        portTexts[4] = GameObject.Find("BottomPortsBlue").GetComponent<Text>();
        portTexts[5] = GameObject.Find("BottomPortsBlack").GetComponent<Text>();

        goText = GameObject.Find("GoText").GetComponent<Text>();
        goButton = GameObject.Find("GoButton").GetComponent<Button>();

        defaultGreen = actionPanels[0].GetComponent<Image>().color;
        attackUnclicked = attackPanels[0].colors.normalColor;
        attackClicked = attackPanels[0].colors.pressedColor;
        arrowYellow = attackArrows[0].colors.normalColor;
        greyedOut = new Color(50,50,50,255);

        straightArrow = Sprites.main.StraightArrow;
        curvedArrow = Sprites.main.CurvedArrow;
        holdSprite = Sprites.main.HoldSprite;
        emptySprite = Sprites.main.EmptySprite;

        //straightArrow = Resources.Load("UpArrow", typeof(Sprite)) as Sprite;
        //curvedArrow = Resources.Load("CurvedArrow", typeof(Sprite)) as Sprite;
        //holdSprite = Resources.Load("holdicon", typeof(Sprite)) as Sprite;
        //emptySprite = Resources.Load("setaction", typeof(Sprite)) as Sprite;

        victoryTracker = GameObject.Find("VictoryText").GetComponent<Text>();
        turnPhase = GameObject.Find("TurnPhase").GetComponentInChildren<Text>();

        redirectNotice = GameObject.Find("PendingRedirect");
        captureNotice = GameObject.Find("PendingCapture");
        rammingNotice = GameObject.Find("PendingRamming");
        catapultNotice = GameObject.Find("PendingCatapult");

        TeamSelectUI = GameObject.Find("TeamSelectPanel");

        phase = GameObject.Find("Phase");
        subPhase = GameObject.Find("SubPhase");

        phaseAnnouncer = GameObject.Find("PhaseAnnouncer");
        phaseAnnouncer.SetActive(false);

        //fadeObjective = false;
    }

    /// <summary>
    /// Activates the Redirect, Port Capture, Ramming Target, and Catapult Target notices
    /// when they are needed.
    /// </summary>
    void Update()
    {
        rammingNotice.SetActive(gameManager.needRammingChoice());
        redirectNotice.SetActive(gameManager.needRedirect());
        captureNotice.SetActive(gameManager.needCaptureChoice());
        catapultNotice.SetActive(gameManager.needCatapultChoice());

        turnPhase.text = "Turn: " + gameLogic.TurnIndex;

        //if (fadeObjective)
        //{
        //    GameObject.Find("OverlayCanvas/Objective").gameObject.GetComponent<Image>().material.color = Color.Lerp(GameObject.Find("OverlayCanvas/Objective").gameObject.GetComponent<Image>().material.color, objectiveColor, 1f * Time.deltaTime);
        //}
        //if(GameObject.Find("OverlayCanvas/Objective") != null && GameObject.Find("OverlayCanvas/Objective").gameObject.GetComponent<Image>().material.color == objectiveColor)
        //{
        //    GameObject.Find("OverlayCanvas/Objective").gameObject.SetActive(false);
        //    fadeObjective = false;
        //}

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            foreach (Team t in gameManager.teams)
            {
                if (t.TeamType == (Team.Type)1 && t.Ready == false)
                    return;
            }
            startTurn(1);
        }
        
    }

    /// <summary>
    /// Sets the currently selected ship when a ship on the board is clicked.
    /// Sets the ship control UI to reflect the new ship.
    /// Changes camera focus to the ship that was just selected.
    /// </summary>
    /// <param name="value">The ship on the board that is clicked</param>
    private void setSelection(Ship value)
    {
        Ship previous = selected;

        if (value != null)
        {
            selected = value;
            onShipSelection();
            if (previous != null)
            {
                previous.DisableIcon();
            }

            updateActionUI();
            updateAttackUI();
            updateTabsUI();
        }
    }

    /// <summary>
    /// Sets the currently selected ship when a ship tab on the UI is clicked.
    /// Sets the ship control UI to reflect the new ship.
    /// Changes camera focus to the ship that was just selected.
    /// </summary>
    /// <param name="value">The id of the ship to be selected</param>
    public void setSelection(int value)
    {
        Ship previous = selected;

        foreach(Ship s in gameManager.getPlayerShips())
        {
            if (s.Id == value)
                selected = s;
        }
        onShipSelection();

        if (previous != null) {
            previous.DisableIcon();
        }

        updateActionUI();
        updateAttackUI();
        updateTabsUI();
    }

    /// <summary>
    /// Changes camera focus to the ship that was just selected.
    /// </summary>
    void onShipSelection() {
        if(selected != null) {
            StartCoroutine(PhaseManager.focus(selected.Position));
            selected.setIconString(selected.getNumeralID());
        }
    }

    /// <summary>
    /// Redirects the selected ship to face the given direction.
    /// Used when the redirect UI is clicked.
    /// </summary>
    /// <param name="newDirection">The desired new direction. Forward is 0, then increases clockwise.</param>
    public void redirect(int newDirection)
    {
        selected.redirect(newDirection);
    }

    public void readyBtnClick()
    {
        PhotonView.Get(this).RPC("toggleReady", RpcTarget.All, (int)GameManager.playerTeam.TeamFaction);
    }

    [PunRPC]
    public void toggleReady(int teamValue) {

        foreach (Team t in gameManager.teams)
        {
            if (t.TeamFaction == (Team.Faction)teamValue)
            {
                t.Ready = !t.Ready;
                break;
            }
        }
        

        //GameManager.playerTeam.Ready = !GameManager.playerTeam.Ready;
        //if (GameManager.playerTeam.Ready) {
        //    foreach(Team t in gameManager.getHumanTeams()) {
        //        if (!t.Ready) {
        //            return;
        //        }
        //    }
        //    startTurn();
        //}
    }

    /// <summary>
    /// Executes the next turn of the game.
    /// Used when the player clicks the start turn button.
    /// Disables ship controls (besides ship tabs) during turn.
    /// </summary>
    public void startTurn(int input)
    {
        if (PhotonNetwork.IsConnected && input == 0)
        {
            readyBtnClick();
            return;
        }
        if (GameManager.main.needRedirect()) {
            return;
        }
        disableControls();
        setShipAttacks();

        GameLogic gl = GameManager.main.gameLogic;
        if (GameLogic.phaseIndex == 4)
        {
            if (PhaseManager.actionAnimations.Count != 0)
            {
                Debug.LogError("Animation manager not finished yet");
            }
            gameLogic.executeTurn();
        }
        else
        {
            Debug.Log("Phase not == 4");
        }
    }
    

    // new for multiplayer
    //public void toggleReady() {
    //    gameManager.playerTeam
    //}

    /// <summary>
    /// Adds the given action to the selected ship's actions.
    /// Used when the player clicks a ship command button.
    /// Updates the Action Images to reflect the new action.
    /// If the selected ship still has remaining open actions, automatically increments to the next action panel.
    /// </summary>
    /// <param name="i">The action to be set. Forward=1, Turn left=2, Turn right=3, Hold=4, Reverse=5</param>
    public void setAction(int i)
    {
        if (i == 5)
        {
            if(selected.currentActionIndex > 0)
            {
                if(selected.actions[selected.currentActionIndex -1].actionIndex == 4)
                {
                    if (PhotonNetwork.IsConnected)
                        PhotonView.Get(selected.gameObject).RPC("setAction", RpcTarget.MasterClient, selected.currentActionIndex, i, -1);
                    else
                        selected.setAction(selected.currentActionIndex, i, -1);
                    setActionImages(i);

                    if (selected.currentActionIndex < (selected.life - 1))
                    {
                        Outline selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
                        Color color = selectedOutline.effectColor;
                        color.a = 0;
                        selectedOutline.effectColor = color;

                        selected.currentActionIndex++;

                        selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
                        color.a = 255;
                        selectedOutline.effectColor = color;
                    }
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsConnected)
                PhotonView.Get(selected.gameObject).RPC("setAction", RpcTarget.MasterClient, selected.currentActionIndex, i, -1);
            else
                selected.setAction(selected.currentActionIndex, i, -1);
            setActionImages(i);
            if (selected.currentActionIndex < (selected.life - 1))
            {
                Outline selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
                Color color = selectedOutline.effectColor;
                color.a = 0;
                selectedOutline.effectColor = color;

                selected.currentActionIndex++;

                selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
                color.a = 255;
                selectedOutline.effectColor = color;
            }
        }
    }

    /// <summary>
    /// Toggles the Debug menu on/off.
    /// </summary>
    public void toggleDebugMenu()
    {
        debugMenu.SetActive(!debugMenu.activeSelf);
    }

    /// <summary>
    /// Sets the player's Team to the given team.
    /// </summary>
    /// <param name="i">The team to switch the player to.
    /// Red = 0, Orange = 1, Yellow = 2, Green = 3, Blue = 4, Black = 5</param>
    public void setTeam(int i)
    {
        gameManager.setupGame(i);
        PostTeamSelection();
    }

    public void PostTeamSelection() {
        if (TeamSelectUI != null)
            TeamSelectUI.SetActive(false);
                    

        GameObject overlay = GameObject.Find("OverlayCanvas");
        overlay.transform.Find("HelpPanel").gameObject.SetActive(true);
        overlay.transform.Find("HelpPanel/Rules").gameObject.SetActive(false);

        overlay.transform.Find("Objective").gameObject.SetActive(true);
        objectiveColor = overlay.transform.Find("Objective").GetComponent<Image>().material.color;
        objectiveColor.a = 0;

        setSelection(gameManager.getPlayerShips()[0].Id);
    }

    /// <summary>
    /// Starts the fade out of the objective panel
    /// </summary>
    public void startObjectiveFade()
    {
        StartCoroutine(objectiveTime());
    }
    
    /// <summary>
    /// Waits 2 seconds and then begins the fade of the objective text 
    /// </summary>
    /// <returns></returns>
    public IEnumerator objectiveTime()
    {
        yield return new WaitForSeconds(2f);
        fadeObjective = true;
        GameObject.Find("OverlayCanvas/Objective").GetComponent<Image>().CrossFadeAlpha(0, 2f, false);
        GameObject.Find("OverlayCanvas/Objective/Text").GetComponent<Text>().CrossFadeAlpha(0, 2f, false);
        StartCoroutine(setObjectiveInactive());
    }

    /// <summary>
    /// Sets the objective panel inactive once the fade out is finished
    /// </summary>
    /// <returns></returns>
    public IEnumerator setObjectiveInactive()
    {
        yield return new WaitForSeconds(2f);
        GameObject.Find("OverlayCanvas/Objective").gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the current action index of the currently selected ship to the given one.
    /// Changes the action panels to outline the new current action.
    /// Used when the player clicks on an undamaged action panel.
    /// </summary>
    /// <param name="i">The desired action index</param>
    public void setCurrentActionIndex(int i)
    {
        Outline selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
        Color color = selectedOutline.effectColor;
        color.a = 0;
        selectedOutline.effectColor = color;

        selected.currentActionIndex = i;

        selectedOutline = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
        color.a = 255;
        selectedOutline.effectColor = color;

    }

    /// <summary>
    /// Changes the sprite in the current action panel to reflect the action that is stored there.
    /// Used when the player gives a new command to a ship, or when the selected ship is changed.
    /// EmptyAction(6) is used to populate the ships default actions before the player sets them.
    /// NoImage(default) is used for damaged panels.
    /// </summary>
    /// <param name="i">Represents the action that is being set.
    /// Forward=1, Turn left=2, Turn right=3, Hold=4, Reverse=5, EmptyAction=6, NoImage = default</param>
    public void setActionImages(int i)
    {
        Image image = actionImages[selected.currentActionIndex].GetComponent<Image>();
        var tempCol = image.color;
        switch (i)
        {
            case 1:
                image.sprite = straightArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, 0);
                break;

            case 2:
                image.sprite = curvedArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 180, -10);
                break;

            case 3:
                image.sprite = curvedArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, -10);
                break;

            case 4:
                image.sprite = holdSprite;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, 0);
                break;

            case 5:
                image.sprite = straightArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, 180);
                break;

            case 6:
                image.sprite = emptySprite;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            default:
                image.sprite = null;
                tempCol.a = 0;
                image.color = tempCol;
                break;
        }
    }

    /// <summary>
    /// Sets the current action panel to damaged.
    /// Changes it's colour and disables it.
    /// </summary>
    private void setDamaged()
    {
        actionPanels[selected.currentActionIndex].GetComponent<Button>().interactable = false;
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = new Color(1, 0, 0, 1);
    }

    /// <summary>
    /// Sets the current action panel to undamaged.
    /// Changes it's colour and enables it.
    /// </summary>
    private void setUndamaged()
    {
        if(selected == null) {
            Debug.Log("Selected is null");
        }
        actionPanels[selected.currentActionIndex].GetComponent<Button>().interactable = true;
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = defaultGreen;
    }

    /// <summary>
    /// Sets the catapult index of the currently selected ship to the given value.
    /// Updates the attack panels on the UI to reflect the new catapult index.
    /// Used when an attack panel is clicked.
    /// Doesn't work if the given index is damaged for the selected ship.
    /// </summary>
    /// <param name="i">The desired catapult index</param>
    public void setCatapultIndex(int i)
    {
        if (selected != null && i < selected.getLife() && controlsEnabled)
        {
            foreach (Button b in attackPanels)
            {
                ColorBlock cb = b.colors;
                cb.normalColor = attackUnclicked;
                b.colors = cb;
            }

            ColorBlock colBlock = attackPanels[i].colors;
            colBlock.normalColor = attackClicked;
            attackPanels[i].colors = colBlock;

            selected.catapultIndex = i;
        }
    }

    /// <summary>
    /// Sets the attack direction for the currently selected ship.
    /// Updates the attack direction arrows on the UI to reflect the new attack direction.
    /// Doesn't work if an catapult index has not been set yet.
    /// </summary>
    /// <param name="i">The desired attack direction. Forward is 0, the increases clockwise. Middle is 8</param>
    public void setAttackDirection(int i)
    {
        if (controlsEnabled)
        {
            foreach (Button b in attackArrows)
            {
                ColorBlock cb = b.colors;
                cb.normalColor = arrowYellow;
                b.colors = cb;
            }

            if (selected.catapultIndex >= 0 && selected != null)
            {


                ColorBlock colBlock = attackArrows[i].colors;
                colBlock.normalColor = attackClicked;
                attackArrows[i].colors = colBlock;

                selected.catapultDirection = i;

            }
        }
    }

    /// <summary>
    /// Updates the bottom UI to show that a ship has been destroyed.
    /// </summary>
    /// <param name="teamNo">The team the dead ship was on
    /// Red = 0, Orange = 1, Yellow = 2, Green = 3, Blue = 4, Black = 5</param>
    /// <param name="shipNo">The ID of the dead ship</param>
    public void setDead(int teamNo, int shipNo)
    {
        bottomIcons[teamNo, shipNo].color = new Color(0.2f, 0.2f, 0.2f, 1f);
    }

    /// <summary>
    /// Updates the bottom UI to show the port totals of each team.
    /// Called each time a port is captured.
    /// Also updates the port total for the player's team on the victory panel.
    /// </summary>
    public void updatePortsUI()
    {
        int[] scores = new int[6];

        foreach (Port p in GameManager.main.Board.ports)
        {
            scores[(int)p.Team.TeamFaction]++;
        }

        for (int i = 0; i < 6; i++)
        {
            if (i == (int)GameManager.playerTeam.TeamFaction)
                GameObject.Find("VictoryText").GetComponent<Text>().text = scores[i] + "/12\nports";
            portTexts[i].text = scores[i] + " / 12";
        }
    }

    public void disableControls()
    {
        controlsEnabled = false;
        goButton.enabled = false;

        foreach(Button b in commandPanels)
        {
            b.interactable = false;
        }
    }


    /// <summary>
    /// Enables ship command buttons, attack panels, attack direction arrows, and start turn button.
    /// Called when a turn is ended.
    /// </summary>
    public void enableControls()
    {
        controlsEnabled = true;
        goButton.enabled = true;

        foreach (Button b in commandPanels)
        {
            b.interactable = true;
        }
    }

    /// <summary>
    /// Sets the ship attacks for each player ship.
    /// Called when the turn is started, before actions are executed.
    /// Doesn't set any attack if a ship doesn't have both it's catapult index and catapult direction set.
    /// </summary>
    public void setShipAttacks()
    {
        foreach(Ship s in gameManager.getHumanShips())
        {
            foreach(Ship.Action a in s.actions)
            {
                a.setCatapult(-1);
            }

            if (s.catapultIndex >= 0 && s.catapultDirection >= 0)
                s.actions[s.catapultIndex].setCatapult(s.catapultDirection);
        }
    }

    /// <summary>
    /// Updates the action panels to reflect the newly selected ship's actions.
    /// Called in the setSelection function when a new ship is selected.
    /// </summary>
    private void updateActionUI()
    {
        selected.currentActionIndex = 0;
        for (int j = 0; j < Ship.MAX_HEALTH; j++)
        {
            if (j >= selected.getLife())
            {
                setDamaged();
                setActionImages(-1);
            }
            else
            {
                setUndamaged();
                setActionImages(selected.actions[selected.currentActionIndex].actionIndex);
            }
            selected.currentActionIndex++;
        }
        selected.currentActionIndex = 0;


        foreach (GameObject go in actionPanels)
        {
            Outline o = go.GetComponent<Outline>();
            Color c = o.effectColor;
            c.a = 0;
            o.effectColor = c;
        }

        Outline currentPanel = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
        Color currentPanelColor = currentPanel.effectColor;
        currentPanelColor.a = 255;
        currentPanel.effectColor = currentPanelColor;
    }

    /// <summary>
    /// Updates the Attack panels and attack direction arrows to reflect the newly selected ship's actions.
    /// Called in the setSelection function when a new ship is selected.
    /// </summary>
    private void updateAttackUI()
    {
        foreach (Button b in attackPanels)
        {
            ColorBlock cb = b.colors;
            cb.normalColor = attackUnclicked;
            b.colors = cb;
        }

        foreach (Button b in attackArrows)
        {
            ColorBlock cb = b.colors;
            cb.normalColor = arrowYellow;
            b.colors = cb;
        }

        if (selected.catapultIndex >= 0)
        {
            ColorBlock colBlock = attackPanels[selected.catapultIndex].colors;
            colBlock.normalColor = attackClicked;
            attackPanels[selected.catapultIndex].colors = colBlock;

            if (selected.catapultDirection >= 0)
            {
                ColorBlock colorBlock = attackArrows[selected.catapultDirection].colors;
                colorBlock.normalColor = attackClicked;
                attackArrows[selected.catapultDirection].colors = colBlock;
            }
        }
    }

    /// <summary>
    /// Updates the ship tabs to reflect the newly selected ship's actions.
    /// Called in the setSelection function when a new ship is selected.
    /// </summary>
    private void updateTabsUI()
    {
        foreach (GameObject go in shipTabs)
        {
            Outline o = go.GetComponent<Outline>();
            Color c = o.effectColor;
            c.a = 0;
            o.effectColor = c;
        }

        Outline selectedOutline = shipTabs[selected.Id].GetComponent<Outline>();
        Color color = selectedOutline.effectColor;
        color.a = 255;
        selectedOutline.effectColor = color;
    }

    public void openMenu()
    {
        GameObject optionsPanel = GameObject.Find("OverlayCanvas").gameObject;
        optionsPanel = optionsPanel.transform.Find("OptionsMenu").gameObject;
        optionsPanel.SetActive(true);
        optionsPanel.GetComponent<OptionsMenu>().OpenOptions();
    }

    public void closeEliminationNotice() {
        GameObject.Find("EliminationScreen").SetActive(false);
    }
}


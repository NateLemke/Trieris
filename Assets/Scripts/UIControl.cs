﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        main = this;
        gameManager = gameObject.GetComponent<GameManager>();
        gameLogic = gameObject.GetComponent<GameLogic>();
        optionsPanel = GameObject.Find("OverlayCanvas");
        optionsPanel = optionsPanel.transform.Find("OptionsMenu").gameObject;
        //DebugControl.init();
    }

    void Start()
    {
        
        //captureTracker = GameObject.Find("captureStatus").GetComponent<Text>();
        //phaseTracker = GameObject.Find("PhaseTracker");
        //animationText = GameObject.Find("AnimationStatus").GetComponent<Text>();
        //redirectText = GameObject.Find("RedirectStatus").GetComponent<Text>();
        //debugMenu = GameObject.Find("DebugControls").transform.GetChild(1).gameObject;

        Selected = null;
        //DevUI = GameObject.Find("DevUI");
        //DevUI.SetActive(false);
        //LogToggle = GameObject.Find("DebugToggle");
        //LogToggle.SetActive(false);

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

        bottomIcons[0, 0] = GameObject.Find("BottomRed1").GetComponent<Image>();
        bottomIcons[0, 1] = GameObject.Find("BottomRed2").GetComponent<Image>();
        bottomIcons[0, 2] = GameObject.Find("BottomRed3").GetComponent<Image>();
        bottomIcons[0, 3] = GameObject.Find("BottomRed4").GetComponent<Image>();
        bottomIcons[0, 4] = GameObject.Find("BottomRed5").GetComponent<Image>();

        bottomIcons[1, 0] = GameObject.Find("BottomOrange1").GetComponent<Image>();
        bottomIcons[1, 1] = GameObject.Find("BottomOrange2").GetComponent<Image>();
        bottomIcons[1, 2] = GameObject.Find("BottomOrange3").GetComponent<Image>();
        bottomIcons[1, 3] = GameObject.Find("BottomOrange4").GetComponent<Image>();
        bottomIcons[1, 4] = GameObject.Find("BottomOrange5").GetComponent<Image>();

        bottomIcons[2, 0] = GameObject.Find("BottomYellow1").GetComponent<Image>();
        bottomIcons[2, 1] = GameObject.Find("BottomYellow2").GetComponent<Image>();
        bottomIcons[2, 2] = GameObject.Find("BottomYellow3").GetComponent<Image>();
        bottomIcons[2, 3] = GameObject.Find("BottomYellow4").GetComponent<Image>();
        bottomIcons[2, 4] = GameObject.Find("BottomYellow5").GetComponent<Image>();

        bottomIcons[3, 0] = GameObject.Find("BottomGreen1").GetComponent<Image>();
        bottomIcons[3, 1] = GameObject.Find("BottomGreen2").GetComponent<Image>();
        bottomIcons[3, 2] = GameObject.Find("BottomGreen3").GetComponent<Image>();
        bottomIcons[3, 3] = GameObject.Find("BottomGreen4").GetComponent<Image>();
        bottomIcons[3, 4] = GameObject.Find("BottomGreen5").GetComponent<Image>();

        bottomIcons[4, 0] = GameObject.Find("BottomBlue1").GetComponent<Image>();
        bottomIcons[4, 1] = GameObject.Find("BottomBlue2").GetComponent<Image>();
        bottomIcons[4, 2] = GameObject.Find("BottomBlue3").GetComponent<Image>();
        bottomIcons[4, 3] = GameObject.Find("BottomBlue4").GetComponent<Image>();
        bottomIcons[4, 4] = GameObject.Find("BottomBlue5").GetComponent<Image>();

        bottomIcons[5, 0] = GameObject.Find("BottomBlack1").GetComponent<Image>();
        bottomIcons[5, 1] = GameObject.Find("BottomBlack2").GetComponent<Image>();
        bottomIcons[5, 2] = GameObject.Find("BottomBlack3").GetComponent<Image>();
        bottomIcons[5, 3] = GameObject.Find("BottomBlack4").GetComponent<Image>();
        bottomIcons[5, 4] = GameObject.Find("BottomBlack5").GetComponent<Image>();

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
        greyedOut = new Color(50, 50, 50, 255);

        straightArrow = Resources.Load("UpArrow", typeof(Sprite)) as Sprite;
        curvedArrow = Resources.Load("CurvedArrow", typeof(Sprite)) as Sprite;
        holdSprite = Resources.Load("holdicon", typeof(Sprite)) as Sprite;
        emptySprite = Resources.Load("setaction", typeof(Sprite)) as Sprite;

        victoryTracker = GameObject.Find("VictoryText").GetComponent<Text>();
        turnPhase = GameObject.Find("TurnPhase").GetComponentInChildren<Text>();

        redirectNotice = GameObject.Find("PendingRedirect");
        captureNotice = GameObject.Find("PendingCapture");
        rammingNotice = GameObject.Find("PendingRamming");
        catapultNotice = GameObject.Find("PendingCatapult");
        //rammingNotice.SetActive(false);

        TeamSelectUI = GameObject.Find("TeamSelectPanel");

        phase = GameObject.Find("Phase");
        subPhase = GameObject.Find("SubPhase");

        phaseAnnouncer = GameObject.Find("PhaseAnnouncer");
        phaseAnnouncer.SetActive(false);

           
        //subPhase.SetActive(false);
        //subPhaseProgress = GameObject.Find("SubPhase Progress");
        //subPhaseProgress.SetActive(false);
    }

    void Update()
    {

        rammingNotice.SetActive(gameManager.needRammingChoice);
        redirectNotice.SetActive(gameManager.needRedirect);
        captureNotice.SetActive(gameManager.needCaptureChoice);
        catapultNotice.SetActive(gameManager.needCatapultChoice);

        turnPhase.text = "Turn: " + gameLogic.TurnIndex;


    }

    private void setSelection(Ship value)
    {


        Ship previous = selected;

        if (value != null)
        {
            selected = value;
            onShipSelection();
            if (previous != null)
            {
                previous.disableIcon();
            }

            updateActionUI();
            updateAttackUI();
            updateTabsUI();

        }
    }

    public void setSelection(int value)
    {
        Ship previous = selected;

        //selected = gameManager.getPlayerShips()[value];
        foreach(Ship s in gameManager.getPlayerShips())
        {
            if (s.getID() == value)
                selected = s;
        }
        onShipSelection();

        if (previous != null) {
            previous.disableIcon();
        }

        //compass.SetActive(true);
        //for (int i = 0; i < gameManager.getPlayerShips()[value].life; i++) {
        //Debug.Log(i); 
        //actions[i].interactable = false;                
        //actions[i].value = gameManager.getPlayerShips()[value].actions[i].actionIndex - 1;
        //actions[i].interactable = true;
        //}

        updateActionUI();
        updateAttackUI();
        updateTabsUI();
    }

    void onShipSelection() {
        if(selected != null) {
            StartCoroutine(PhaseManager.focus(selected.Position));
            selected.setIconString(selected.getNumeralID());
        }
    }

    public void redirect(int newDirection)
    {
        selected.redirect(newDirection);
    }

    public void testMove()
    {

        disableControls();
        setShipAttacks();

        GameLogic gl = GameManager.main.gameLogic;
        if (GameLogic.phaseIndex == 4)
        {
            if (PhaseManager.playingAnimation || PhaseManager.actionAnimations.Count != 0)
            {
                Debug.LogError("Animation manager not finished yet");
            }
            gameLogic.newExecuteTurn();
        }
        else
        {
            Debug.Log("Phase not == 4");
        }
    }

    public void setAction(int i)
    {
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

    public void setSelected(Ship ship)
    {
        if (ship == null)
        {
            selection.SetActive(false);
        }
        else
        {
            selection.transform.position = ship.transform.position;
            selection.SetActive(true);
        }
    }

    public void toggleDebugMenu()
    {
        debugMenu.SetActive(!debugMenu.activeSelf);
    }

    public void setTeam(int i)
    {
        TeamSelectUI.SetActive(false);
        gameManager.setPlayerTeam(i);

        GameObject overlay = GameObject.Find("OverlayCanvas");
        overlay.transform.Find("HelpPanel").gameObject.SetActive(true);
        overlay.transform.Find("HelpPanel/Rules").gameObject.SetActive(false);
    }

    //public void toggleDevUI()
    //{
    //    DevUI.SetActive(!DevUI.activeSelf);
    //    LogToggle.SetActive(!LogToggle.activeSelf);
    //}

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
                //image.rectTransform.Rotate(new Vector3(0, 0, -90));
                break;

            case 2:
                image.sprite = curvedArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 180, -10);
                //image.rectTransform.Rotate(new Vector3(0, 180, 100));
                break;

            case 3:
                image.sprite = curvedArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, -10);
                //image.rectTransform.Rotate(new Vector3(0, 0, 100));
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
                //image.rectTransform.Rotate(new Vector3(0, 0, 90));
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

    public void devPhaseTrack(int i)
    {
        if (phaseTracker.activeSelf)
        {
            phaseTracker.GetComponentInChildren<Text>().text = "Phase " + (i + 1);
            Image phaseImg = phaseTracker.GetComponent<Image>();
            switch (i)
            {
                case 0:
                    phaseImg.color = Color.blue; break;
                case 1:
                    phaseImg.color = Color.green; break;
                case 2:
                    phaseImg.color = Color.yellow; break;
                case 3:
                    phaseImg.color = Color.red; break;
                case 4:
                    phaseImg.color = Color.black;
                    phaseTracker.GetComponentInChildren<Text>().text = "planning phase"; break;
            }
        }
    }

    public static void postNotice(string s, float time)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TempText");
        GameObject go = Instantiate(prefab, GameObject.Find("NoticeHolder").transform);
        go.GetComponent<TempText>().lifetime = time;
        go.GetComponent<Text>().text = s;
        //Debug.Break();
    }

    private void setDamaged()
    {
        actionPanels[selected.currentActionIndex].GetComponent<Button>().interactable = false;
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = new Color(1, 0, 0, 1);
    }

    private void setUndamaged()
    {
        actionPanels[selected.currentActionIndex].GetComponent<Button>().interactable = true;
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = defaultGreen;
    }

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

                /*
                foreach (Ship.Action a in selected.actions)
                {
                    a.setCatapult(-1);
                }

                selected.actions[selected.catapultIndex].setCatapult(i);
                */
            }
        }
    }

    public void setDead(int teamNo, int shipNo)
    {
        bottomIcons[teamNo, shipNo].color = new Color(0.2f, 0.2f, 0.2f, 1f);
    }

    public void updatePortsUI()
    {
        int[] scores = new int[6];

        foreach (Port p in GameManager.main.getBoard().getAllPorts())
        {
            scores[(int)p.Team.getTeamType()]++;
        }

        for (int i = 0; i < 6; i++)
        {
            if (i == (int)gameManager.playerTeam.getTeamType())
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

    public void enableControls()
    {
        controlsEnabled = true;
        goButton.enabled = true;

        foreach (Button b in commandPanels)
        {
            b.interactable = true;
        }
    }

    public void setShipAttacks()
    {
        foreach(Ship s in gameManager.getPlayerShips())
        {
            foreach(Ship.Action a in s.actions)
            {
                a.setCatapult(-1);
            }

            if (s.catapultIndex >= 0 && s.catapultDirection >= 0)
                s.actions[s.catapultIndex].setCatapult(s.catapultDirection);
        }
    }

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

    private void updateTabsUI()
    {
        foreach (GameObject go in shipTabs)
        {
            Outline o = go.GetComponent<Outline>();
            Color c = o.effectColor;
            c.a = 0;
            o.effectColor = c;
        }

        Outline selectedOutline = shipTabs[selected.getID()].GetComponent<Outline>();
        Color color = selectedOutline.effectColor;
        color.a = 255;
        selectedOutline.effectColor = color;
    }
}


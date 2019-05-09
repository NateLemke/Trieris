using System;
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
    GameObject DevUI;
    GameObject LogToggle;
    GameObject phaseTracker;
    Text turnPhase;
    Text captureTracker;
    Text victoryTracker;
    GameObject redirectNotice;
    GameObject captureNotice;
    GameObject rammingNotice;

    GameObject TeamSelectUI;

    Image[] actionImages = new Image[4];
    GameObject[] actionPanels = new GameObject[4];
    Button[] attackPanels = new Button[4];
    Button[] attackArrows = new Button[9];
    GameObject[] shipTabs = new GameObject[5];
    Image[,] bottomIcons = new Image[6, 5];
    Text[] portTexts = new Text[6];
    Text phaseText;

    Sprite straightArrow;
    Sprite curvedArrow;
    Sprite holdSprite;
    GameObject selection;

    Color defaultGreen;
    Color attackUnclicked;
    Color attackClicked;
    Color arrowYellow;
    Color greyedOut;

    public GameObject phaseAnnouncer;
    public GameObject phase;
    public GameObject subPhase;
    //public GameObject subPhaseProgress;

    public GameObject optionsPanel;
    public static UIControl main;

    public Ship Selected { get { return selected; } set { setSelection(value); } }
    private Ship selected;

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameLogic = gameObject.GetComponent<GameLogic>();
        optionsPanel = GameObject.Find("OverlayCanvas");
        optionsPanel = optionsPanel.transform.Find("OptionsMenu").gameObject;
        //DebugControl.init();
    }

    void Start()
    {
        main = this;
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

        attackPanels[0] = GameObject.Find("PanelAttack1").GetComponent<Button>();
        attackPanels[1] = GameObject.Find("PanelAttack2").GetComponent<Button>();
        attackPanels[2] = GameObject.Find("PanelAttack3").GetComponent<Button>();
        attackPanels[3] = GameObject.Find("PanelAttack4").GetComponent<Button>();

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

        phaseText = GameObject.Find("GoText").GetComponent<Text>();

        defaultGreen = actionPanels[0].GetComponent<Image>().color;
        attackUnclicked = attackPanels[0].colors.normalColor;
        attackClicked = attackPanels[0].colors.pressedColor;
        arrowYellow = attackArrows[0].colors.normalColor;
        greyedOut = new Color(50, 50, 50, 255);

        straightArrow = Resources.Load("StraightArrow", typeof(Sprite)) as Sprite;
        curvedArrow = Resources.Load("CurvedArrow", typeof(Sprite)) as Sprite;
        holdSprite = Resources.Load("StopSymbol", typeof(Sprite)) as Sprite;

        victoryTracker = GameObject.Find("VictoryCounter").GetComponentInChildren<Text>();
        turnPhase = GameObject.Find("TurnPhase").GetComponentInChildren<Text>();

        redirectNotice = GameObject.Find("PendingRedirect");
        captureNotice = GameObject.Find("PendingCapture");
        rammingNotice = GameObject.Find("PendingRamming");
        rammingNotice.SetActive(false);

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

        //if (gameManager.animationPlaying)
        //{
        //    animationText.text = "animation playing";
        //    animationText.color = Color.red;
        //}
        //else
        //{
        //    animationText.text = "no animation";
        //    animationText.color = Color.green;
        //}

        if (gameManager.needRedirect)
        {
            //redirectText.text = "need redirect";
            //redirectText.color = Color.red;
            redirectNotice.SetActive(true);
        }
        else
        {
            //redirectText.text = "no redirect";
            //redirectText.color = Color.green;
            redirectNotice.SetActive(false);
        }

        if (gameManager.needCaptureChoice)
        {
            //captureTracker.text = "need capture";
            //captureTracker.color = Color.red;
            captureNotice.SetActive(true);
        }
        else
        {
            //captureTracker.text = "no capture";
            //captureTracker.color = Color.green;
            captureNotice.SetActive(false);
        }

        string s = (gameLogic.phaseIndex == 4) ? " Planning phase" : " Phase: " + gameLogic.phaseIndex;
        turnPhase.text = "Turn: " + gameLogic.TurnIndex + s;


    }

    private void setSelection(Ship value) {
        if(selected != null) {
            selected.disableIcon();
        }

        if(value != null) {
            selected = value;
            onShipSelection();

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

                if (selected.actions[selected.catapultIndex].catapultDirection >= 0)
                {
                    ColorBlock colorBlock = attackArrows[selected.actions[selected.catapultIndex].catapultDirection].colors;
                    colorBlock.normalColor = attackClicked;
                    attackArrows[selected.actions[selected.catapultIndex].catapultDirection].colors = colBlock;
                }
            }

            foreach (GameObject go in shipTabs)
            {
                Outline o = go.GetComponent<Outline>();
                Color c = o.effectColor;
                c.a = 0;
                o.effectColor = c;
            }

            Outline selectedOutline = shipTabs[value.getID()].GetComponent<Outline>();
            Color color = selectedOutline.effectColor;
            color.a = 255;
            selectedOutline.effectColor = color;

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
            currentPanel.effectColor = color;
        }

    }

    public void setSelection(int value)
    {
        if (selected != null) {
            selected.disableIcon();
        }

        selected = gameManager.getPlayerShips()[value];
        onShipSelection();

        //compass.SetActive(true);
        for (int i = 0; i < gameManager.getPlayerShips()[value].life; i++) {
            //Debug.Log(i); 
            //actions[i].interactable = false;                
            //actions[i].value = gameManager.getPlayerShips()[value].actions[i].actionIndex - 1;
            //actions[i].interactable = true;
        }

        selected.currentActionIndex = 0;
        for (int j = 0; j < Ship.MAX_HEALTH; j++) {
            if (j >= selected.getLife()) {
                setDamaged();
                setActionImages(-1);
            } else {
                setUndamaged();
                setActionImages(selected.actions[selected.currentActionIndex].actionIndex);
            }
            selected.currentActionIndex++;
        }
        selected.currentActionIndex = 0;

        foreach (Button b in attackPanels) {
            ColorBlock cb = b.colors;
            cb.normalColor = attackUnclicked;
            b.colors = cb;
        }

        foreach (Button b in attackArrows) {
            ColorBlock cb = b.colors;
            cb.normalColor = arrowYellow;
            b.colors = cb;
        }

        if (selected.catapultIndex >= 0) {
            ColorBlock colBlock = attackPanels[selected.catapultIndex].colors;
            colBlock.normalColor = attackClicked;
            attackPanels[selected.catapultIndex].colors = colBlock;

            if (selected.actions[selected.catapultIndex].catapultDirection >= 0) {
                ColorBlock colorBlock = attackArrows[selected.actions[selected.catapultIndex].catapultDirection].colors;
                colorBlock.normalColor = attackClicked;
                attackArrows[selected.actions[selected.catapultIndex].catapultDirection].colors = colBlock;
            }
        }

        foreach (GameObject go in shipTabs) {
            Outline o = go.GetComponent<Outline>();
            Color c = o.effectColor;
            c.a = 0;
            o.effectColor = c;
        }

        Outline selectedOutline = shipTabs[value].GetComponent<Outline>();
        Color color = selectedOutline.effectColor;
        color.a = 255;
        selectedOutline.effectColor = color;

        foreach (GameObject go in actionPanels) {
            Outline o = go.GetComponent<Outline>();
            Color c = o.effectColor;
            c.a = 0;
            o.effectColor = c;
        }

        Outline currentPanel = actionPanels[selected.currentActionIndex].GetComponent<Outline>();
        Color currentPanelColor = currentPanel.effectColor;
        currentPanelColor.a = 255;
        currentPanel.effectColor = color;

    }

    void onShipSelection() {
        if(selected != null) {
            StartCoroutine(PhaseManager.focus(selected.Position,0.0f,SpeedManager.CameraFocusSpeed));
            selected.setIconString(selected.getNumeralID());
        }
    }

    public void redirect(int newDirection)
    {
        selected.redirect(newDirection);
    }

    public void testMove()
    {

        GameLogic gl = GameManager.main.gameLogic;
        if (GameManager.main.gameLogic.phaseIndex == 4)
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
    }

    public void toggleDevUI()
    {
        DevUI.SetActive(!DevUI.activeSelf);
        LogToggle.SetActive(!LogToggle.activeSelf);
    }

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
                image.transform.eulerAngles = new Vector3(0, 0, -90);
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
                image.transform.eulerAngles = new Vector3(0, 0, 90);
                //image.rectTransform.Rotate(new Vector3(0, 0, 90));
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
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = new Color(1, 0, 0, 1);
    }

    private void setUndamaged()
    {
        Image i = actionPanels[selected.currentActionIndex].GetComponent<Image>();
        i.color = defaultGreen;
    }

    public void setCatapultIndex(int i)
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

    public void setAttackDirection(int i)
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

            foreach (Ship.Action a in selected.actions)
            {
                a.setCatapult(-1);
            }

            selected.actions[selected.catapultIndex].setCatapult(i);
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
            if (i == 3)
                GameObject.Find("VictoryCounter").GetComponentInChildren<Text>().text = scores[i] + "/12\nports";
            portTexts[i].text = scores[i] + " / 12";
        }
    }
}


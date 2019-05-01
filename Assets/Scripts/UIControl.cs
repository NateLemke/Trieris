﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    GameManager gameManager;
    GameLogic gameLogic;
    //public Sprite ship

    //private int test = 99;
    Text animationText;
    Text redirectText;
    Button startTurn;
    Dropdown[] actions = new Dropdown[4];
    Text shipID;
    Dropdown teamSelect;
    Image teamColor;

   // GameObject compass;
    GameObject debugMenu;
    GameObject DevUI;
    GameObject LogToggle;
    GameObject phaseTracker;
    Text captureTracker;

    GameObject TeamSelectUI;

    Image[] actionImages = new Image[4];
    Button[] actionPanels = new Button[4];
    Button[] attackPanels = new Button[4];
    Button[] attackArrows = new Button[9];

    Sprite straightArrow;
    Sprite curvedArrow;
    Sprite holdSprite;
    //GameObject selection;

    Color defaultGreen;
    Color attackUnclicked;
    Color attackClicked;
    Color arrowYellow;

    public static UIControl main;

    public Ship Selected { get { return selected; } set { setSelection(value); } }

    private void setSelection(Ship value) {
        if(value == null) {
            for (int i = 0; i < actions.Length; i++) {
                selected = value;
                actions[i].interactable = false;
                //compass.SetActive(false);
                shipID.text = "No Ship";
            }
        } else {
            selected = value;
            //compass.SetActive(true);
            shipID.text = "Ship " + (value.getID() + 1);
            for (int i = 0; i < value.life; i++) {
                //Debug.Log(i); 
                //actions[i].interactable = false;                
                //actions[i].value = value.actions[i].actionIndex -1;
               // actions[i].interactable = true;
            }

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
        }
        
    }

    public void setSelection(int value)
    {
        if (gameManager.getPlayerShips()[value] == null)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                selected = gameManager.getPlayerShips()[value];
                actions[i].interactable = false;
                //compass.SetActive(false);
                shipID.text = "No Ship";
            }
        }
        else
        {
            selected = gameManager.getPlayerShips()[value];
            //compass.SetActive(true);
            shipID.text = "Ship " + (gameManager.getPlayerShips()[value].getID() + 1);
            for (int i = 0; i < gameManager.getPlayerShips()[value].life; i++)
            {
                //Debug.Log(i); 
                //actions[i].interactable = false;                
                //actions[i].value = gameManager.getPlayerShips()[value].actions[i].actionIndex - 1;
                //actions[i].interactable = true;
            }

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

            foreach(Button b in attackPanels)
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

            if(selected.catapultIndex >= 0)
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
        }

    }

    private Ship selected;

    private void Awake() {
        gameManager = gameObject.GetComponent<GameManager>();
        gameLogic = gameObject.GetComponent<GameLogic>();
        DebugControl.init();
    }

    // Use this for initialization
    void Start () {
        main = this;
        captureTracker = GameObject.Find("captureStatus").GetComponent<Text>();
        phaseTracker = GameObject.Find("PhaseTracker");
        animationText = GameObject.Find("AnimationStatus").GetComponent<Text>();
        redirectText = GameObject.Find("RedirectStatus").GetComponent<Text>();
        startTurn = GameObject.Find("Go").GetComponent<Button>();
        debugMenu = GameObject.Find("DebugControls").transform.GetChild(1).gameObject;
        shipID = GameObject.Find("ShipLabel").GetComponent<Text>();
        for (int i = 0; i < actions.Length; i++) {
            actions[i] = GameObject.Find("Phase" + (i+1)).GetComponent<Dropdown>();
        }
        teamSelect = GameObject.Find("TeamChoose").GetComponent<Dropdown>();
        teamSelect.ClearOptions();

        TeamSelectUI = GameObject.Find("TeamSelectPanel");

        foreach (Team t in gameManager.teams) {            
            teamSelect.options.Add(new Dropdown.OptionData() { text = t.getTeamType().ToString() });
        }
        teamColor = GameObject.Find("TeamColor").GetComponent<Image>();
        teamSelect.value = 1;

        Selected = null;
        DevUI = GameObject.Find("DevUI");
        DevUI.SetActive(false);
        LogToggle = GameObject.Find("DebugToggle");
        LogToggle.SetActive(false);


        actionImages[0] = GameObject.Find("ActionImage1").GetComponent<Image>();
        actionImages[1] = GameObject.Find("ActionImage2").GetComponent<Image>();
        actionImages[2] = GameObject.Find("ActionImage3").GetComponent<Image>();
        actionImages[3] = GameObject.Find("ActionImage4").GetComponent<Image>();

        actionPanels[0] = GameObject.Find("PanelAction1").GetComponent<Button>();
        actionPanels[1] = GameObject.Find("PanelAction2").GetComponent<Button>();
        actionPanels[2] = GameObject.Find("PanelAction3").GetComponent<Button>();
        actionPanels[3] = GameObject.Find("PanelAction4").GetComponent<Button>();

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


        defaultGreen = actionPanels[0].colors.normalColor;
        attackUnclicked = attackPanels[0].colors.normalColor;
        attackClicked = attackPanels[0].colors.pressedColor;
        arrowYellow = attackArrows[0].colors.normalColor;


        straightArrow = Resources.Load("StraightArrow", typeof(Sprite)) as Sprite;
        curvedArrow = Resources.Load("CurvedArrow", typeof(Sprite)) as Sprite;
        holdSprite = Resources.Load("StopSymbol", typeof(Sprite)) as Sprite;
    }
	
	// Update is called once per frame
	void Update () {
        //if(gameManager.animationPlaying || gameManager.needRedirect) {
        //    startTurn.interactable = false;
        //} else {
        //    startTurn.interactable = true;
        //}

        if (gameManager.animationPlaying) {
            animationText.text = "animation playing";
            animationText.color = Color.red;
        } else {
            animationText.text = "no animation";
            animationText.color = Color.green;
        }

        if (gameManager.needRedirect) {
            //compass.SetActive(true);
            redirectText.text = "need redirect";
            redirectText.color = Color.red;
        } else {
            //compass.SetActive(false);
            redirectText.text = "no redirect";
            redirectText.color = Color.green;
        }

        if (gameManager.needCaptureChoice) {
            captureTracker.text = "need capture";
            captureTracker.color = Color.red;
        } else {
            captureTracker.text = "no capture";
            captureTracker.color = Color.green;
        }


    }

    public void redirect(int newDirection) {
        selected.redirect(newDirection);
        //compass.SetActive(false);
    }

    public void testMove() {
        //Ship ship = selected;

        

        //ship.setAction(0,actions[0].value+1,-1);
        //ship.setAction(1,actions[1].value + 1,-1);
        //ship.setAction(2,actions[2].value + 1,-1);
        //ship.setAction(3,actions[3].value + 1,-1);
        
        gameLogic.newExecuteTurn();

        //gameLogic.executePhase(0);
    }

    public void setAction(int i) {
        //Debug.Log("action "+i+" was " + selected.actions[i].actionIndex);
        selected.setAction(selected.currentActionIndex,i,-1);
        setActionImages(i);
        
        if(selected.currentActionIndex < (selected.life - 1))
            selected.currentActionIndex++;
        //Debug.Log("action " + i + " is now " + selected.actions[i].actionIndex);
    }

    //public void setSelected(Ship ship) {
    //    if(ship == null) {
    //        selection.SetActive(false);
    //    } else {
    //        selection.transform.position = ship.transform.position;
    //        selection.SetActive(true);
    //    }
    //}

    public void toggleDebugMenu() {
        debugMenu.SetActive(!debugMenu.activeSelf);
    }

    public void setTeam() {
        //if(teamSelect.value == 0) {
        //    teamColor.color = Color.white;
        //    gameManager.setPlayerTeam()
        //}
        Debug.Log("Player team set...");
        gameManager.setPlayerTeam((Team.Type)teamSelect.value);
        teamColor.color = gameManager.teams[teamSelect.value].getColor();

        foreach(Ship ship in gameManager.playerTeam.ships) {
            ship.needRedirect = true;
        }
    }

    public void setTeam(int i)
    {
        Debug.Log("Player team set...");
        gameManager.setPlayerTeam((Team.Type)i);
        teamColor.color = gameManager.teams[teamSelect.value].getColor();

        foreach (Ship ship in gameManager.playerTeam.ships)
        {
            ship.needRedirect = true;
        }

        gameManager.cameraLock = false;
        TeamSelectUI.SetActive(false);
    }

    public void toggleDevUI() {
        DevUI.SetActive(!DevUI.activeSelf);
        LogToggle.SetActive(!LogToggle.activeSelf);
    }

    public void setCurrentActionIndex(int i)
    {
        selected.currentActionIndex = i;
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
                image.transform.eulerAngles = new Vector3(0, 180, 100);
                //image.rectTransform.Rotate(new Vector3(0, 180, 100));
                break;

            case 3:
                image.sprite = curvedArrow;
                tempCol.a = 255;
                image.color = tempCol;
                image.transform.eulerAngles = new Vector3(0, 0, 100);
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

    public void devPhaseTrack(int i) {
        if(phaseTracker.activeSelf) {
            phaseTracker.GetComponentInChildren<Text>().text = "Phase " + (i + 1);
            Image phaseImg = phaseTracker.GetComponent<Image>(); 
            switch (i) {
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
                phaseTracker.GetComponentInChildren<Text>().text = "planning phase";break;
            }
        }
    }

    private void setDamaged()
    {
        Button b = actionPanels[selected.currentActionIndex];
        ColorBlock cb = b.colors;
        cb.normalColor = new Color(1, 0, 0, 1);
        b.colors = cb;
    }

    private void setUndamaged()
    {
        Button b = actionPanels[selected.currentActionIndex];
        ColorBlock cb = b.colors;
        cb.normalColor = defaultGreen;
        b.colors = cb;
    }

    public void setCatapultIndex(int i)
    {
        foreach(Button b in attackPanels)
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
}


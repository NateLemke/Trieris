using System;
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

    GameObject compass;
    GameObject debugMenu;
    GameObject DevUI;
    GameObject LogToggle;

    Image[] actionImages = new Image[4];

    Sprite straightArrow;
    Sprite curvedArrow;
    Sprite holdSprite;
    //GameObject selection;

    

    public Ship Selected { get { return selected; } set { setSelection(value); } }

    private void setSelection(Ship value) {
        if(value == null) {
            for (int i = 0; i < actions.Length; i++) {
                selected = value;
                actions[i].interactable = false;
                compass.SetActive(false);
                shipID.text = "No Ship";
            }
        } else {
            selected = value;
            compass.SetActive(true);
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
                setActionImages(-1);
                selected.currentActionIndex++;
            }
            selected.currentActionIndex = 0;
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
                compass.SetActive(false);
                shipID.text = "No Ship";
            }
        }
        else
        {
            selected = gameManager.getPlayerShips()[value];
            compass.SetActive(true);
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
                setActionImages(-1);
                selected.currentActionIndex++;
            }
            selected.currentActionIndex = 0;
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
        //Debug.Log("init UIcontrol");
        //Debug.Log("test: " + test);
        animationText = GameObject.Find("AnimationStatus").GetComponent<Text>();
        redirectText = GameObject.Find("RedirectStatus").GetComponent<Text>();
        startTurn = GameObject.Find("Go").GetComponent<Button>();
        compass = GameObject.Find("Compass");
        compass.SetActive(false);
        //debugMenu = GameObject.Find("DebugGrid");
        debugMenu = GameObject.Find("DebugControls").transform.GetChild(1).gameObject;
        
        //Debug.Log("debug name " + debugMenu.name);
        //debugMenu.SetActive(false);
        //selection = GameObject.Find("selection");
        //selection.SetActive(false);
        shipID = GameObject.Find("ShipLabel").GetComponent<Text>();
        for (int i = 0; i < actions.Length; i++) {
            actions[i] = GameObject.Find("Phase" + (i+1)).GetComponent<Dropdown>();
        }
        teamSelect = GameObject.Find("TeamChoose").GetComponent<Dropdown>();
        teamSelect.ClearOptions();
        //teamSelect.options.Add(new Dropdown.OptionData() { text = "none" });
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


    }

    public void redirect(int newDirection) {
        selected.redirect(newDirection);
        compass.SetActive(false);
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
        selected.setAction(selected.currentActionIndex,i,1);
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

}

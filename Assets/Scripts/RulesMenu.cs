using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for the Rules menu acessed via the pause menu
/// </summary>
public class RulesMenu : MonoBehaviour
{

    Text infoText;
    GameObject infoPanel;

    GameObject content;

    private int position;
    
    private List<Action> ruleList = new List<Action>();

    /// <summary>
    /// Sets the gameobject references at the first frame the object is loaded and then calls the first function for displaying rules.
    /// </summary>
    void Start()
    {
        content = transform.Find("Content").gameObject;

        infoPanel = transform.Find("InfoPanel").gameObject;
        infoText = transform.Find("InfoPanel/Text").GetComponent<Text>();

        position = 0;
        moveToActions();


        ruleList.Add(moveToActions);
        ruleList.Add(moveToCatapults);
        ruleList.Add(moveToRamming);
        ruleList.Add(moveToMultipleTargets);
        ruleList.Add(moveToRedirection);
        ruleList.Add(moveToPorts);
        ruleList.Add(moveToPortsCapitals);
        ruleList.Add(moveToShipsAlive);
    }

    public void next()
    {
        position++;
        if (position >= ruleList.Count)
            position = ruleList.Count - 1;
        ruleList[position]();
    }

    public void prev()
    {
        position--;
        if (position < 0)
            position = 0;
        ruleList[position]();
    }

    public void updatePosition(int input)
    {
        position = input;
    }

    /// <summary>
    /// Displays rules about Phases and Actions planned for phases
    /// </summary>
    public void moveToActions()
    {
        setImagesInactive();
        content.transform.Find("Actions").gameObject.SetActive(true);
        
        infoText.text = "Every turn, there are 4 phases in which actions can be set. You can only set actions for as much life as your ship has.\nExample: your ship has 3 life remaining, you can only set 3 actions for that ship";
    }

    /// <summary>
    /// Displays rules about Catapults
    /// </summary>
    public void moveToCatapults()
    {
        setImagesInactive();
        content.transform.Find("CatapultTarget").gameObject.SetActive(true);
        content.transform.Find("Catapult").gameObject.SetActive(true);

        infoText.text = "On the righthand side of the screen, there is a catapult targeting interface. On any turn, you may set a phase to aim and shoot a catapult for 1 damage.\nIf your ship is rammed, neither ship may shoot for the rest of the turn";
    }

    /// <summary>
    /// Displays rules about Ramming and the different types of Ramming
    /// </summary>
    public void moveToRamming()
    {
        setImagesInactive();
        content.transform.Find("Broadside").gameObject.SetActive(true);
        content.transform.Find("Glancing1").gameObject.SetActive(true);
        content.transform.Find("HeadOn").gameObject.SetActive(true);
        content.transform.Find("Momentum").gameObject.SetActive(true);

        infoText.text = "When a ship moves into a node that contains another ship, it will ram that ship. If two ships move into the same space, they will ram each other!\n<b><i>Broadside:</i></b>\nShip rams another ship that is perpendicular to itself. Double Damage!\n<b><i>Glancing:</i></b>\nShip rams another ship at an angle.\n<b><i>Head On:</i></b>\nTwo ships ram each other while facing each other. If they are adjacent to each other, they will not enter each other's nodes";
    }

    /// <summary>
    /// Explains what happens when multiple ships are potential targets when ramming or shooting a catapult
    /// </summary>
    public void moveToMultipleTargets()
    {
        setImagesInactive();
        content.transform.Find("MultipleTarget").gameObject.SetActive(true);

        infoText.text = "When a catapult or ram targets a node that contains multiple ships, you will get to choose the ship that damage is dealt to.";
    }

    /// <summary>
    /// Explains the rules about redirecting after port capture and crashing into land
    /// </summary>
    public void moveToRedirection()
    {
        setImagesInactive();
        content.transform.Find("Redirection1").gameObject.SetActive(true);
        content.transform.Find("Redirection2").gameObject.SetActive(true);

        infoText.text = "Whenever a ship runs into land or captures a port/capital, that ship loses the rest of its actions this turn and can immediately choose the direction that it faces.";
    }

    /// <summary>
    /// Explains the rules about port capture and repair
    /// </summary>
    public void moveToPorts()
    {
        setImagesInactive();
        content.transform.Find("Port").gameObject.SetActive(true);

        infoText.text = "When a ship lands on a port or capital, the player may choose to capture it. Doing so will make that ship lose its actions for the rest of the turn.\n If you hold on a port or capital that you own, your ship will begin to repair. On a port, you repair once a turn and on a capital, you repair every phase.";
    }

    /// <summary>
    /// Explains what must be done to win or lose a game regarding port/capital capture
    /// </summary>
    public void moveToPortsCapitals()
    {
        setImagesInactive();
        content.transform.Find("Capital").gameObject.SetActive(true);

        infoText.text = "If a team controls 12 ports at any given time, that team wins the game. However, losing your team's capital will immediately cause you to lose.";
    }
    
    /// <summary>
    /// Explains what happens when a team loses all of its ships.
    /// </summary>
    public void moveToShipsAlive()
    {
        setImagesInactive();
        content.transform.Find("Score").gameObject.SetActive(true);

        infoText.text = "If your team has no ships remaining, your team also loses. Even if you own more ports than your opponents. The number at the top is the number of ports you own and the ship icons are how many ships you own alive.";
    }
    
    /// <summary>
    /// Sets all childeren of the Content gameObject to be not active. This resets the panel and prevents multiple images from overlapping
    /// </summary>
    public void setImagesInactive()
    {
        foreach(Transform child in transform.Find("Content"))
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// This closes the rules menu
    /// </summary>
    public void exitHelp()
    {
        transform.parent.gameObject.SetActive(false);
    }
}

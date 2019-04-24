using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUIControl : MonoBehaviour, TrierisUIInterface {

    GameManager gameManager;

    public void executeTurn() {
        throw new System.NotImplementedException();
    }

    public bool promptPlayerCapture(string message) {
        throw new System.NotImplementedException();
    }

    public int promptPlayerDirection(string message) {
        throw new System.NotImplementedException();
    }

    public Ship promptPlayerShips(string message,List<Ship> ships) {
        throw new System.NotImplementedException();
    }

    public TeamColor promptPlayerTeam() {
        throw new System.NotImplementedException();
    }

    public void ready() {
        throw new System.NotImplementedException();
    }

    public void setShipAction(int shipIndex,int actionIndex,int actionType,int catapultDirection) {
        throw new System.NotImplementedException();
    }

    public void updateMapDisplay() {
        foreach(Ship ship in gameManager.getPlayerShips()) {
            
        }
    }

    public void victory(TeamColor color) {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        gameManager = GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the keyboard and mouse inputs for camera controls, menu controls, and game speed controls
/// </summary>
public class InputControl : MonoBehaviour {

    public GameObject mainCamera;
    public Camera camera;
    GameManager gameManager;
    UIControl uiControl;

    public float moveRate;
    public float topBound;
    public float bottomBound;
    public float leftBound;
    public float rightBound;
    public float cameraHeight;
    public float cameraWidth;
    public float zoomRate;
    public float zoomInputScale;
    public float zoomExponent;
    public float minCamSize = 2.0f;
    public float maxCamSize = 10.0f;
    float minZoomExpo;
    float maxZoomExpo;

    float shipSelectRadius = 0.2f;

    public static bool fastAnimation = false;

    public GameObject optionsPanel;
    public GameObject overlayCanvas;

    private void Awake() {
        mainCamera = GameObject.Find("Main Camera");
        camera = mainCamera.GetComponent<Camera>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiControl = GameObject.Find("GameManager").GetComponent<UIControl>();
        overlayCanvas = GameObject.Find("OverlayCanvas");
        optionsPanel = overlayCanvas.transform.Find("OptionsMenu").gameObject;
    }

    // Use this for initialization
    void Start() {

        moveRate = (moveRate == 0) ? 1 : moveRate;
        zoomRate = (zoomRate == 0) ? 1 : zoomRate;
        zoomInputScale = (zoomInputScale == 0) ? 1 : zoomInputScale;

        maxZoomExpo = Mathf.Log(minCamSize,zoomRate);
        minZoomExpo = Mathf.Log(maxCamSize,zoomRate);

        cameraZoom(-22);

        minCamSize = 2.0f;
        maxCamSize = 10.0f;

        topBound = 12f;
        bottomBound = 2f;
        leftBound = 7f;
        rightBound = 20f;
    }

    // Update is called once per frame
    void Update() {
        cameraUpdate();

        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient) {
            if (Input.GetKeyDown(KeyCode.F)) {
                SpeedManager.toggleFastAnimations();
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                SpeedManager.skipSubPhase();
            }
        }

        if (GameManager.playerTeam != null) {
            shipSelectUpdate();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Team types:");
            foreach (Team t in gameManager.teams)
            {
                Debug.Log(t.TeamFaction.ToString() + " is a/an " + t.TeamType);
            }
        }

        if (Input.GetKeyDown("escape") && GameManager.main.gameOver == false)
        {

            if (!optionsPanel.active)
            {
                if (overlayCanvas.transform.Find("HelpPanel").gameObject.active)
                {
                    if (GameObject.Find("OverlayCanvas/Objective").gameObject.active)
                        gameObject.GetComponent<UIControl>().startObjectiveFade();
                    overlayCanvas.transform.Find("HelpPanel").gameObject.SetActive(false);
                }
                else
                {
                    optionsPanel.SetActive(true);
                    optionsPanel.GetComponent<OptionsMenu>().OpenOptions();
                }

            }
            else
            {
                if (!overlayCanvas.transform.Find("HelpPanel").gameObject.active)
                    optionsPanel.GetComponent<OptionsMenu>().CloseOptions();
                else
                    overlayCanvas.transform.Find("HelpPanel").gameObject.SetActive(false);
            }
        }

        //if (Input.GetKeyDown("p") && GameManager.main.gameOver == false)
        //{
        //    Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        //}

        //if (Input.GetKeyDown(KeyCode.F1)) {
        //    foreach(Port p in GameManager.main.Board.ports) {
        //        if(PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) {
        //            PhotonView.Get(GameManager.main).RPC("SetPortTeam",RpcTarget.MasterClient,p.id,(int)GameManager.playerTeam.TeamFaction);
        //        } else {
        //            GameManager.main.SetPortTeam(p.id,(int)GameManager.playerTeam.TeamFaction);
        //        }
        //    }
        //}
//
        //if (Input.GetKeyDown(KeyCode.F2)) {
        //    int teamID = (int)GameManager.playerTeam.TeamFaction;
        //    foreach (Ship s in GameManager.main.getAllShips()) {
        //        if((int)s.team.TeamFaction != teamID) {
        //            if (PhotonNetwork.IsConnected) {
        //                if (PhotonNetwork.IsMasterClient) {
        //                    s.TakeDamage(s.life);
        //                } else {
        //                    PhotonView.Get(s).RPC("TakeDamage",RpcTarget.MasterClient,s.life);
        //                }
        //            }
        //            
        //        }
        //    }
        //}
//
        //if (Input.GetKeyDown(KeyCode.F3)) {
        //    foreach (Port p in GameManager.main.Board.ports) {
        //        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) {
        //            PhotonView.Get(GameManager.main).RPC("SetPortTeam",RpcTarget.MasterClient,p.id,((int)GameManager.playerTeam.TeamFaction + 1) % 6);
        //        } else {
        //            GameManager.main.SetPortTeam(p.id,((int)GameManager.playerTeam.TeamFaction + 1)%6);
        //        }
        //    }
        //}
//
        //if (Input.GetKeyDown(KeyCode.F4)) {
        //    int teamID = (int)GameManager.playerTeam.TeamFaction;
        //    foreach (Ship s in GameManager.playerTeam.ships) {
        //        if (PhotonNetwork.IsConnected) {
        //            if (PhotonNetwork.IsMasterClient) {
        //                s.TakeDamage(s.life);
        //            } else {
        //                PhotonView.Get(s).RPC("TakeDamage",RpcTarget.MasterClient,s.life);
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Makes the ship that is currently under the cursor to be the current selected ship
    /// </summary>
    public void shipSelectUpdate() {
        Ship hover = null;

        foreach (Ship s in gameManager.getPlayerShips()) {
            if (Vector2.Distance(s.transform.position,mouseWorldPos()) < shipSelectRadius) {                
                hover = s;
                break;
            }
        }        

        if (Input.GetMouseButtonDown(0)) {

            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }          

            uiControl.Selected = hover;
            if(hover != null) {
                DebugControl.log("select","selected ship "+hover.team.ToString()+" " + hover.Id);
            }
        }
    }

    /// <summary>
    /// Used to manipulate the camera's zoom levels based on mouse scroll wheel input
    /// </summary>
    public void cameraUpdate() {
        float input;
        cameraHeight = camera.orthographicSize * 2.0f;
        cameraWidth = cameraHeight * camera.aspect;

        if ((input = Input.mouseScrollDelta.y) != 0 && !gameManager.cameraLock) {
            cameraZoom(input);
        }
        
        Vector3 move = new Vector3();
        if ((input = Input.GetAxis("Horizontal")) != 0 && !gameManager.cameraLock) {
            if((camera.transform.position.x + input) < (rightBound + camera.orthographicSize)
                && (camera.transform.position.x + input) > (leftBound - camera.orthographicSize))
                move.x = input;
        }
        if ((input = Input.GetAxis("Vertical")) != 0 && !gameManager.cameraLock) {
            if ((camera.transform.position.y + input) < (topBound + camera.orthographicSize)
                && (camera.transform.position.y + input) > (bottomBound - camera.orthographicSize))
                move.y = input;
        }
        if (move != Vector3.zero) {
            cameraMove(move);
        }
    }

    /// <summary>
    /// Changes the zoom level of the camera
    /// </summary>
    /// <param name="input"></param>
    public void cameraZoom(float input) {
        input *= zoomInputScale;

        float newExpo = ((zoomExponent + input) > maxZoomExpo) ? maxZoomExpo : zoomExponent + input;
        zoomExponent = Mathf.Clamp(newExpo, -22f, maxZoomExpo);         

        camera.orthographicSize = Mathf.Clamp(Mathf.Pow(zoomRate,zoomExponent), minCamSize, maxCamSize);
    }

    /// <summary>
    /// Moves the main camera
    /// </summary>
    /// <param name="move">The amount that the screen moves</param>
    public void cameraMove(Vector3 move)
    {
        mainCamera.transform.position += move * Time.deltaTime * moveRate * camera.orthographicSize;
    }

    /// <summary>
    /// Gets the location of the camera based on the position it is on the screen and returns the location that it is over in the world space
    /// </summary>
    /// <returns></returns>
    public static Vector2 mouseWorldPos() {
        return Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
    }    
}



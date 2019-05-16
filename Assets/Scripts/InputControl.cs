using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Awake() {
        mainCamera = GameObject.Find("Main Camera");
        camera = mainCamera.GetComponent<Camera>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiControl = GameObject.Find("GameManager").GetComponent<UIControl>();
        optionsPanel = GameObject.Find("OverlayCanvas");
        optionsPanel = optionsPanel.transform.Find("OptionsMenu").gameObject;
    }

    // Use this for initialization
    void Start () {

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
        
        if (Input.GetKeyDown(KeyCode.F)) {
            SpeedManager.toggleFastAnimations();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpeedManager.skipSubPhase();
        }
        if (gameManager.playerTeam != null) {
            shipSelectUpdate();
        }

        if (Input.GetKeyDown("escape"))
        {
            if (!optionsPanel.active)
            {
                optionsPanel.SetActive(true);
                optionsPanel.GetComponent<OptionsMenu>().OpenOptions();
            }
            else
                optionsPanel.GetComponent<OptionsMenu>().CloseOptions();
        }

        if (Input.GetKeyDown("`"))
        {
            GameObject gameOverObj = GameObject.Find("GameOver").gameObject;
            gameOverObj.transform.Find("Screen").gameObject.SetActive(true);
            gameOverObj.GetComponent<GameOver>().Initialize("Victory");
        }
    }

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

    public void cameraZoom(float input) {
        input *= zoomInputScale;

        float newExpo = ((zoomExponent + input) > maxZoomExpo) ? maxZoomExpo : zoomExponent + input;
        zoomExponent = Mathf.Clamp(newExpo, -22f, maxZoomExpo);         

        camera.orthographicSize = Mathf.Clamp(Mathf.Pow(zoomRate,zoomExponent), minCamSize, maxCamSize);
    }

    public void cameraMove(Vector3 move)
    {
    
    //float
    mainCamera.transform.position += move * Time.deltaTime * moveRate * camera.orthographicSize;
    }

    public static Vector2 mouseWorldPos() {
        return Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
    }    
}



using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: This class manages an instance of a port capture UI prompt
///             This UI is used to allow the player to chose whether or not to capture a port
/// </summary>
public class PortPrompt : MonoBehaviour{

    private GameObject portPromptPanel;
    private GameObject portNotification;
    private Port thisPort;
    private Ship currentShip;

    /// <summary>
    /// Finds the references to the UI gameobjects
    /// </summary>
    void Start()
    {
        portPromptPanel = transform.Find("CapturePrompt").gameObject;
        portNotification = transform.Find("CaptureNotification").gameObject;
        //portNotification.SetActive(false);
        //portPromptPanel.SetActive(false);
    }

    /// <summary>
    /// Enables the port capture UI
    /// </summary>
    public void activatePrompt()
    {
        portPromptPanel.SetActive(true);
        portNotification.SetActive(false);
    }

    /// <summary>
    /// Changes the port capture UI from active to notification status if the player clicks on another capture UI
    /// </summary>
    private void CheckUnfocus()
    {
        if(Input.GetMouseButton(0) && portPromptPanel.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(portPromptPanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            notify();
        }
    }

    /// <summary>
    /// Activates the notification status for this capture UI and sets references to the port and the ship involved
    /// </summary>
    /// <param name="inPort">the reference to the port</param>
    /// <param name="inShip">the reference to the ship doing the capture</param>
    public void activateNotification(Port inPort, Ship inShip)
    {
        thisPort = inPort;
        currentShip = inShip;

        notify();
    }

    /// <summary>
    /// Activates the notification status for this capture UI
    /// </summary>
    public void notify()
    {
        portNotification.SetActive(true);
        portPromptPanel.SetActive(false);
    }


    /// <summary>
    /// Activated when the player chooses to accept the port capture
    /// Plays the port capture for this port and disables the UI
    /// </summary>
    public void accept()
    {
        foreach (Ship s in currentShip.getNode().Ships)
            s.needCaptureChoice = false;

        // needs to be changed for multiplayer
        //GameManager.PortsCaptured++;

        if (PhotonNetwork.IsConnected)
        {
            PhotonView.Get(currentShip).RPC("playerCapture", RpcTarget.MasterClient);
        }
        currentShip.playerCapture();
        portPromptPanel.SetActive(false);
        GameManager.main.StartCoroutine(acceptAnimation(currentShip));
    }

    /// <summary>
    /// Activated when the player declines the capture
    /// </summary>
    public void decline()
    {        
        portPromptPanel.SetActive(false);
        currentShip.needCaptureChoice = false;
        GameManager.main.StartCoroutine(declineAnimation());
    }

    /// <summary>
    /// Used to check if the player has clicked on another port capture UI
    /// </summary>
    void Update()
    {
        CheckUnfocus();
    }

    /// <summary>
    /// Plays the port capture animation
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static IEnumerator acceptAnimation(Ship s) {
        yield return new PortCaptureAnimation(s).playAnimation();
        foreach (Ship ps in GameManager.main.getPlayerShips()) {
            if (ps.needCaptureChoice) {
                yield return PhaseManager.focus(ps.Position);
                break;
            }
        }
        s.needCaptureChoice = false;
    }

    /// <summary>
    /// Focuses the camera on the next pending port capture choice, if there is one
    /// </summary>
    /// <returns></returns>
    static IEnumerator declineAnimation() {
        yield return new WaitForSeconds(0.5f);
        foreach (Ship ps in GameManager.main.getPlayerShips()) {
            if (ps.needCaptureChoice) {
                yield return PhaseManager.focus(ps.Position);
                break;
            }
        }
    }
}

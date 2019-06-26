using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Purpose:    This class manages the game over screen, and checks for a game over condition.
/// </summary>
public class GameOver : MonoBehaviour
{
    Text title;
    Text portsHeld;
    Text portsCaptured;
    Text shipsAlive;

    /// <summary>
    /// Gets references to UI elements
    /// </summary>
    void Awake()
    {
        title = transform.Find("GameoverScreen/Title/Text").GetComponent<Text>();
        portsHeld = transform.Find("GameoverScreen/Stats/PortsHeld").GetComponent<Text>();
        portsCaptured = transform.Find("GameoverScreen/Stats/PortsCapTotal").GetComponent<Text>();
        shipsAlive = transform.Find("GameoverScreen/Stats/ShipsAlive").GetComponent<Text>();
    }

    public void gameOverCapture(Team t) {
        Initialize();
        if (t == GameManager.playerTeam) {
            title.text = "Victory, you've captured 12 ports";
        } else {
            title.text = t.getTeamType().ToString() + " team won by capturng 12 ports";
        }
    }

    public void gameOverElimination(Team t) {
        Initialize();
        if (t == GameManager.playerTeam) {
            title.text = "Victory, you've destroyed all other ships";
        } else {
            title.text = t.getTeamType().ToString() + " team won by destroying all other ships";
        }
    }


    /// <summary>
    /// Sets the test for the game over screen
    /// </summary>
    /// <param name="gameOverState">Whether the player as won or lost</param>
    public void Initialize()
    {
        portsHeld.text = "Ports Owned: " + GameManager.playerTeam.ports.Count.ToString();
        shipsAlive.text = "Ships Owned: " + GameManager.playerTeam.ships.Count.ToString();
    }

    /// <summary>
    /// Returns the number of ports owned by the player
    /// </summary>
    /// <returns></returns>
    private int countPorts()
    {
        int portCount = 0;
        foreach(Port port in GameManager.main.Board.ports)
            if (port.Team == GameManager.playerTeam)
                portCount++;
        return portCount;
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    public void ExitGame()
    {
        //For Editor Closing
        //UnityEditor.EditorApplication.isPlaying = false;
        //For Build Closing
        Application.Quit();
    }
}

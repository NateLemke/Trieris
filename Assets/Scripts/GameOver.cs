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
        title = transform.Find("Screen/Title/Text").GetComponent<Text>();
        portsHeld = transform.Find("Screen/Stats/PortsHeld").GetComponent<Text>();
        portsCaptured = transform.Find("Screen/Stats/PortsCapTotal").GetComponent<Text>();
        shipsAlive = transform.Find("Screen/Stats/ShipsAlive").GetComponent<Text>();
    }

    /// <summary>
    /// Sets the test for the game over screen
    /// </summary>
    /// <param name="gameOverState">Whether the player as won or lost</param>
    public void Initialize(string gameOverState)
    {
        title.text = gameOverState;
        portsCaptured.text = "Ports Captured: " + GameManager.PortsCaptured.ToString();
        portsHeld.text = "Ports Owned: " + countPorts().ToString();
        shipsAlive.text = "Ships Owned: " + GameManager.main.playerTeam.ships.Count.ToString();
    }

    /// <summary>
    /// Returns the number of ports owned by the player
    /// </summary>
    /// <returns></returns>
    private int countPorts()
    {
        int portCount = 0;
        foreach(Port port in GameManager.main.Board.ports)
            if (port.Team == GameManager.main.playerTeam)
                portCount++;
        return portCount;
    }

    /// <summary>
    /// Restarts the game
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
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

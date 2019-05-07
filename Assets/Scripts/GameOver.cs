using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    Text title;
    Text portsHeld;
    Text portsCaptured;
    Text shipsAlive;

    void Awake()
    {
        title = transform.Find("Screen/Title/Text").GetComponent<Text>();
        portsHeld = transform.Find("Screen/Stats/PortsHeld").GetComponent<Text>();
        portsCaptured = transform.Find("Screen/Stats/PortsCapTotal").GetComponent<Text>();
        shipsAlive = transform.Find("Screen/Stats/ShipsAlive").GetComponent<Text>();
    }

    public void Initialize(string gameOverState)
    {
        title.text = gameOverState;
        portsCaptured.text = "Ports Captured: " + GameManager.PortsCaptured.ToString();
        portsHeld.text = "Ports Owned: " + countPorts().ToString();
        shipsAlive.text = "Ships Owned: " + GameManager.main.playerTeam.ships.Count.ToString();
    }

    private int countPorts()
    {
        int portCount = 0;
        foreach(Port port in GameManager.main.getBoard().ports)
            if (port.getTeam() == GameManager.main.playerTeam)
                portCount++;
        return portCount;
    }

    public void Restart()
    {
        SceneManager.LoadScene("ThompsonDevScene");
    }

    public void ExitGame()
    {
        //For Editor Closing
        UnityEditor.EditorApplication.isPlaying = false;
        //For Build Closing
        Application.Quit();
    }
}

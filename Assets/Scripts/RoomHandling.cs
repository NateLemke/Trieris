using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomHandling : MonoBehaviour
{
    GameObject thisRoom;
    // Start is called before the first frame update
    void Start()
    {
        thisRoom = GameObject.Find("Canvas").gameObject;
        thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            for(int i = 1; i < 6; i++)
            {
                thisRoom.transform.Find("Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text = "Empty";
            }
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                thisRoom.transform.Find("Teams/Team" + getSlotPosition(p) + "/InformationPanel/Name/Text").GetComponent<Text>().text = p.NickName;
            }
        }
    }

    public int getSlotPosition(Player inPlayer)
    {
        return orderedActorNumbers().IndexOf(inPlayer.ActorNumber) + 1;
    }

    private List<int> orderedActorNumbers()
    {
        List<int> tempPlayerList = new List<int>();
        if (PhotonNetwork.IsConnected)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                tempPlayerList.Add(p.ActorNumber);
            }
        }
        tempPlayerList.Sort();
        return tempPlayerList;
    }

    private Player playerInSlot(int slotNumber)
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if (orderedActorNumbers()[slotNumber-1] == p.ActorNumber)
                return p;
        }
        return PhotonNetwork.LocalPlayer;
    }

    public void setPlayerTeam(int slotNumber)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        Dropdown thisDropdown = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slotNumber + "/TeamImage/Dropdown").GetComponent<Dropdown>();
        customProperties.Add("Team", thisDropdown.options[thisDropdown.value].text);
        playerInSlot(slotNumber).SetCustomProperties(customProperties);
        Debug.Log("Player " + slotNumber + ", team changed to " + thisDropdown.options[thisDropdown.value].text);
    }

    public void setLocalPlayerTeam()
    {
        Debug.Log(getSlotPosition(PhotonNetwork.LocalPlayer));
        setPlayerTeam(getSlotPosition(PhotonNetwork.LocalPlayer));
    }

    void OnEnable()
    {
        setLocalPlayerTeam();
    }
}

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomHandling : MonoBehaviour
{
    GameObject thisRoom;
    public Toggle privateGame;


    // Start is called before the first frame update
    void Start()
    {
        thisRoom = GameObject.Find("Canvas").gameObject;
        thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
    }

    void OnEnable()
    {
        setLocalPlayerTeam();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            for(int i = 1; i <= 6; i++)
            {
                thisRoom.transform.Find("Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text = "Empty";
            }
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                thisRoom.transform.Find("Teams/Team" + getSlotPosition(p) + "/InformationPanel/Name/Text").GetComponent<Text>().text = p.NickName;
            }
            setRoomName();
        }
    }

    public void setRoomName()
    {
        thisRoom.transform.Find("RoomName/InputField");
        thisRoom.transform.Find("RoomName/InputField").gameObject.GetComponent<InputField>().text = PhotonNetwork.CurrentRoom.Name;
    }

    public int getSlotPosition(Player inPlayer)
    {
        return orderedActorNumbers().IndexOf(inPlayer.ActorNumber) + 1;
    }

    public List<int> orderedActorNumbers()
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

    public Player playerInSlot(int slotNumber)
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
        Hashtable customProperties = new Hashtable();
        Dropdown thisDropdown = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slotNumber + "/TeamImage/Dropdown").GetComponent<Dropdown>();
        customProperties.Add("TeamInt", thisDropdown.value);
        customProperties.Add("LoadedGame",false);
        playerInSlot(slotNumber).SetCustomProperties(customProperties);
        Debug.Log("Player " + slotNumber + ", team changed to " + playerInSlot(slotNumber).CustomProperties["TeamInt"]);
    }

    public void setLocalPlayerTeam()
    {
        Debug.Log(getSlotPosition(PhotonNetwork.LocalPlayer));
        setPlayerTeam(getSlotPosition(PhotonNetwork.LocalPlayer));
    }
}

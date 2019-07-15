using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCManager : MonoBehaviourPunCallbacks
{

    //Send local actions to master client

    //Figure out a way to detect port capture and redirects and get only the player whose ship it is to display the notification.
    ///1. Show the exclaimation mark on all instances (?)
    ///2. Pause the game while they make the choice
    ///     2a. Send the result of the prompt to the master which sends the appropriate animations to the other players
    ///3. Continue once all prompts are completed.


}

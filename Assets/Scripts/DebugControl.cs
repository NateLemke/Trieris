using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class DebugControl {
    static bool logSelect;
    static bool logAction;
    static bool logTurn;
    static bool logPhase;
    static bool logRamming;

    static Dictionary<string,bool> logTypes = new Dictionary<string,bool>();
    private static string[] types = { "select","action","turn","phase","ramming","position","gui","animation","movement","catapult","AI"};

    public static void log(string type, string message){
        if (logTypes.ContainsKey(type) && logTypes[type]) {
            Debug.Log(message);
        }        
    }

    public static void init() {
        //Debug.Log("Init debugcontrol");
        GameObject grid = GameObject.Find("DebugControls").transform.GetChild(1).gameObject;
        foreach (string s in types) {
            logTypes.Add(s,false);
            GameObject load = Resources.Load<GameObject>("LogToggle");
            GameObject toggle = GameObject.Instantiate(load,grid.transform);
            toggle.transform.parent = grid.transform;
            toggle.transform.GetChild(1).GetComponent<Text>().text = "log " + s;
            Toggle t = toggle.GetComponent<Toggle>();
            t.isOn = false;
            t.onValueChanged.AddListener(delegate { DebugControl.toggle(s,t); });
        }        
    }

    public static void toggle(string type, Toggle t) {
        if (logTypes.ContainsKey(type)) {
            logTypes[type] = !logTypes[type];
            t.isOn = logTypes[type];
        } else {
            Debug.Log("no such log type found");
        }
    }

    
}

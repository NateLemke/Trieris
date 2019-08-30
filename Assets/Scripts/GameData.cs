using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public float[,] shipPositions;
    public int[] shipHealth;
    public bool[] shipSunk;
    public int[] shipDirection;
    public int[,] shipActions;

    public int[] portOwners;

    public GameData() {
        Initialize();
    }

    public void Initialize() {
        shipPositions = new float[30,2];
        shipHealth = new int[30];
        shipSunk = new bool[30];
        shipDirection = new int[30];
        shipActions = new int[30,4];
        portOwners = new int[30];
    }
}

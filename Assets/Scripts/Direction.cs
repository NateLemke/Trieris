using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: The different directions ships can face. Currently is only used for creating the board.
/// </summary>
public class Direction {

    public static int[][] DIRECTIONS = {
        new int[] {0,-1, 0 },   // North
        new int[] {1,-1, 1 },   // Northeast
        new int[] {2, 0, 1 },   // East
        new int[] {3, 1, 1 },   // Southeast
        new int[] {4, 1, 0 },   // South
        new int[] {5, 1,-1 },   // Southwest
        new int[] {6, 0,-1 },   // West
        new int[] {7,-1,-1 },   // Northwest
    };

    int index;
    int dx;
    int dy;

    //public Direction(int index, int dx, int dy) {

    //}
}

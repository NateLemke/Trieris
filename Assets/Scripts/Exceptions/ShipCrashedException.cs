using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCrashedException : Exception {
    public ShipCrashedException(Ship ship) : base("Ship number " + ship.getID() + " has crashed into land.") {

    }
}

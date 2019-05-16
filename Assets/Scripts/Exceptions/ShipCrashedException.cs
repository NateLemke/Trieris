using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCrashedException : Exception {
    public ShipCrashedException(Ship ship) : base("Ship number " + ship.Id + " has crashed into land.") {

    }
}

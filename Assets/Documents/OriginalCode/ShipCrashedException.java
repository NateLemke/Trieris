package trieris;

public class ShipCrashedException extends Exception {
    public ShipCrashedException(Ship ship) {
        super("Ship number " + ship.getID() + " has crashed into land.");
    }
}

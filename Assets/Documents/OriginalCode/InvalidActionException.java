package trieris;

public class InvalidActionException extends Exception {
    public InvalidActionException() {
        super("Attempted to plan an invalid action.");
    }
}

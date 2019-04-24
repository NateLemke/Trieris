package trieris;


public class InvalidActionIndexException extends Exception {
    public InvalidActionIndexException() {
        super("This ship is damaged and cannot access this action.");
    }
}
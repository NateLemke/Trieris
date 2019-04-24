package trieris;


public class CannotReverseException extends Exception {
    public CannotReverseException() {
        super("The previous action must be a Hold in order to Reverse.");
    }
}
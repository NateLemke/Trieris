package trieris;

public class NoTrierisInterfaceException extends Exception {
    public NoTrierisInterfaceException() {
        super("There is no TrierisUI linked to this Trieris.");
    }
}

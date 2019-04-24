package trieris;
import java.util.Comparator;

public class NodePathComparator implements Comparator<NodePath> {

    @Override
    public int compare(NodePath o1, NodePath o2) {
        if (o1.getActionsList().size() < o2.getActionsList().size()) {
            return -1;
        }
        if (o1.getActionsList().size() > o2.getActionsList().size()) {
            return 1;
        }
        return 0;
    }
}

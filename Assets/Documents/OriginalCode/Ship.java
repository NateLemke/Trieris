package trieris;
import java.util.ArrayList;
import java.util.List;

public class Ship {
    public static final int FORWARD = 1;
    public static final int PORT = 2;
    public static final int STARBOARD = 3;
    public static final int HOLD = 4;
    public static final int REVERSE = 5;

    public static final int MAX_HEALTH = 4;
    
    private List<Action> actions = new ArrayList<>();
    private Node node;
    private TeamColor teamColor;
    private int life = MAX_HEALTH;
    private int momentum = 0;
    private int portRepairCount = 0;
    private boolean movedForward = false;

    private int fireDirection = -1;
    private int front = 0;
    private boolean canAct = true;
    private boolean canActAfterCollision = true;
    private int frontAfterCollision = -1;
    private int id = -1;
    TrierisAI ai = null;
    
    public Ship(TeamColor teamColor, int id, Node node) {
        this.teamColor = teamColor;
        this.id = id;
        this.node = node;
        node.getShips().add(this);
        for (int i = 0; i < 4; i++) {
            actions.add(null);
        }
    }
    
    public void setAction(int index, int actionNum, int firingDirection)
            throws CannotReverseException, InvalidActionException, InvalidActionIndexException {
        if (index > life - 1 || index < 0) { 
            throw new InvalidActionIndexException();
        }
        if (actionNum == 5 && (index == 0 || actions.get(index - 1) == null ||
                !actions.get(index - 1).reverseReady)) {
            throw new CannotReverseException();
        } else {
            actions.set(index, getAction(actionNum, firingDirection));
        }
    }
    
    public void doAction(int index) throws ShipCrashedException {
        actions.get(index).act();
        
    }

    private abstract class Action {
        int catapultDirection = -1;
        boolean reverseReady = false;
        
        public Action(int catapultDirection) {
            this.catapultDirection = catapultDirection;
        }
        
        public void act() throws ShipCrashedException {
            if (canAct) {
                affectShip();
                fireDirection = catapultDirection;
            }
        }
        
        protected abstract void affectShip() throws ShipCrashedException;
        
    }
    
    private class ForwardAction extends Action {
        
        public ForwardAction(int catapultDirection) {
            super(catapultDirection);
        }

        protected void affectShip() throws ShipCrashedException {
            move(front);
            speedup();
        }
    }
    
    private class ReverseAction extends Action {
        
        public ReverseAction(int catapultDirection) {
            super(catapultDirection);
        }

        protected void affectShip() throws ShipCrashedException {
            move(getRelativeDirection(-4));
        }
    }
    
    private class HoldAction extends Action {

        public HoldAction(int catapultDirection) {
            super(catapultDirection);
            reverseReady = true;
        }
        
        protected void affectShip() {
            hold();
            if (catapultDirection == -1) {
                repair();
            }
        }
    }

    private class StarboardAction extends Action {

        public StarboardAction(int catapultDirection) {
            super(catapultDirection);
        }
    
        protected void affectShip() {
            turn(1);
        }
    }
    
    private class PortAction extends Action {

        public PortAction(int catapultDirection) {
            super(catapultDirection);
        }
        
        protected void affectShip() {
            turn(-1);
        }
    }

    public int getFront() {
        return front;
    }
    
    public int getMomentum() {
        return momentum;
    }
    
    public Node getCatapultNode() {
        if (fireDirection == -1) {
            return null;
        }
        if (fireDirection == 8) {
            return node;
        }
        return node.getAdjacentNode(getRelativeDirection(fireDirection));
    }
    
    public Node getNode() {
        return node;
    }
    
    public TeamColor getTeamColor() {
        return teamColor;
    }
    
    public int getLife() {
        return life;
    }
    
    public boolean getCanAct() {
        return canAct;
    }
    
    public boolean getMoved() {
        return movedForward;
    }
    
    public int getID() {
        return id;
    }
    
    public boolean ready() {
        for (Action action : actions) {
            if (action == null) {
                return false;
            }
        }
        return true;
    }
    
    public void ram(Ship target) {
        if (target != null) {
            if ((this.teamColor == TeamColor.BLUE && this.id == 1 || this.teamColor == TeamColor.BLACK && this.id == 2)) {
                System.out.println(teamColor + " 1 front: " + front + " target: " + target.front + " moved forward " + movedForward);
            }
            ramDamageAndAngle(target);
            if ((this.teamColor == TeamColor.BLUE && this.id == 1 || this.teamColor == TeamColor.BLACK && this.id == 2)) {
                System.out.println(teamColor + " 1 front: " + frontAfterCollision + " target: " + target.frontAfterCollision + " moved forward " + movedForward);
            }
            momentum = 0;
        }
    }

    public void catapult(Ship target) {
        if (target != null) {
            target.life -= 1;
        }
    }

    public void reset() {
        canAct = true;
        portRepairCount = 0;
        momentum = 0;
        movedForward = false;
        frontAfterCollision = -1;
        canActAfterCollision = true;
        fireDirection = -1;
        for (int i = 0; i < 4; i++) {
            actions.set(i, null);
        }
    }

    public void move(int direction) throws ShipCrashedException {
        Node destNode = node.getAdjacentNode(direction);
        if (destNode == null) {
            life--;
            canActAfterCollision = false;
            throw new ShipCrashedException(this);
        }
        node.getShips().remove(this);
        node = destNode;
        node.getShips().add(this);
    }
    
    public void turn(int relativeDirection) {
        front = getRelativeDirection(relativeDirection);
        momentum = 0;
    }
    
    public void setFront(int direction) {
        front = direction % 8;
    }
    
    public void hold() {
        momentum = 0;
    }
    
    public void speedup() {
        momentum++;
        movedForward = true;
    }
    
    public void repair() {
        if (node.getPort() != null && life < MAX_HEALTH) {
            if (node.getPort().getCapital() && node.getPort().getTeamColor() == teamColor) {
                life++;
            } else {
                portRepairCount++;
                if (portRepairCount == life) {
                    life++;
                    portRepairCount = 0;
                }
            }
        }
    }
    
    public void sink() {
        if (node != null) {
            node.getShips().remove(this);
        }
        node = null;
    }
    
    public void capturePort() {
        node.getPort().setTeamColor(teamColor);
        canActAfterCollision = false;
        canAct = false;
    }
    
    void updateFrontAfterCollision() {
        movedForward = false;
        if (frontAfterCollision != -1) {
            front = frontAfterCollision;
        }
        canAct = canActAfterCollision;
        frontAfterCollision = -1;
    }
    
    void populateDefaultActions() {
        for (int i = 0; i < MAX_HEALTH; i++) {
            if (actions.get(i) == null) {
                actions.set(i, new HoldAction(-1));
            }
        }
    }
    
    private Action getAction(int actionNum, int firingDirection) throws InvalidActionException {
        switch (actionNum) {
        case 1 : return new ForwardAction(firingDirection);
        case 2 : return new PortAction(firingDirection);
        case 3 : return new StarboardAction(firingDirection);
        case 4 : return new HoldAction(firingDirection);
        case 5 : return new ReverseAction(firingDirection);
        default : throw new InvalidActionException();
        }
    }

    private int getRelativeDirection(int relativeDelta) {
        relativeDelta %= 8;
        int result = front + relativeDelta;
        if (result > 7) {
            result -= 8;
        } else if (result < 0) {
            result += 8;
        }
        return result;
    }

    private void ramDamageAndAngle(Ship target) {
        int enemyAngle = target.front;
        if (!target.movedForward && (enemyAngle == getRelativeDirection(2) ||
                enemyAngle == getRelativeDirection(6))) {
            broadsideRam(target);
        } else if (enemyAngle == getRelativeDirection(1) ||
                enemyAngle == getRelativeDirection(2) ||
                enemyAngle == getRelativeDirection(3)) {
            glancingRam(target, -1);
        } else if (enemyAngle == getRelativeDirection(5) ||
                enemyAngle == getRelativeDirection(6) ||
                enemyAngle == getRelativeDirection(7)) {
            glancingRam(target, 1);
        } else if (Math.abs(this.front - target.front) == 4) {
            headOnRam(target);
        } else {
            glancingRam(target, 0);
        }
    }
    
    private void broadsideRam(Ship target) {
        target.life -= momentum * 2;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
    }
    
    private void headOnRam(Ship target) {
        target.life -= momentum;
        target.canActAfterCollision = false;
        canActAfterCollision = false;
        if (!target.movedForward) {
            this.life--;
        }
    }
    
    private void glancingRam(Ship target, int relativeTurn) {
        target.life -= momentum;
        target.frontAfterCollision = target.getRelativeDirection(relativeTurn);
        
        if (!target.movedForward && this.front != target.front) {
            this.frontAfterCollision = this.getRelativeDirection(-relativeTurn);
            this.life--;
        }
    }
    
    void setAI(TrierisAI ai) {
        this.ai = ai;
    }
    
    TrierisAI getAI() {
        return ai;
    }
    
    /*
    public static void main(String[] args) {
        
        Node node1 = new Node(1, 1);
        Node node2 = new Node(1, 2);
        Node node3 = new Node(1, 2);
        Node node4 = new Node(1, 2);
        node1.setAdjacentNode(3, node3);
        node2.setAdjacentNode(5, node3);
        node3.setAdjacentNode(4, node4);
        
        Ship ship1 = new Ship(TeamColor.ORANGE, 1, node1);
        Ship ship2 = new Ship(TeamColor.BLACK, 1, node3);
        ship1.setFront(3);
        ship2.setFront(7);

        ArrayList<Ship> ships = new ArrayList<Ship>();
        ships.add(ship1);
        ships.add(ship2);
        Controller controller = new Controller(ships);
        
        try {
            ship1.setAction(0, 1, -1);
            ship2.setAction(0, 4, -1);
            ship1.setAction(1, 4, -1);
            ship2.setAction(1, 4, -1);
            ship1.setAction(2, 4, -1);
            ship2.setAction(2, 4, -1);
            ship1.setAction(3, 4, -1);
            ship2.setAction(3, 4, -1);
            
            controller.executeTurn();
            
        } catch (Exception e) {
            e.printStackTrace();
        }

        System.out.println(1);
        ship1.printNodeShips(node1);
        System.out.println(2);
        ship1.printNodeShips(node2);
        System.out.println(3);
        ship1.printNodeShips(node3);
        System.out.println(4);
        ship1.printNodeShips(node4);
        
    }
    */

    private void printNodeShips(Node node) {
        for (Ship ship : node.getShips()) {
            //System.out.println(ship.getTeamColor());
            //System.out.println("Front" + ship.getFront());
            //System.out.println("Life" + ship.getLife());
            //System.out.println();
        }
    }
    
    public String toString() {
        
        return teamColor + " Ship " + id;
        
    }
    
}

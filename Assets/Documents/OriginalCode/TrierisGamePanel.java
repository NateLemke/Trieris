package trierisUI;

import java.awt.BorderLayout;
import java.util.List;

import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.SwingUtilities;

import trieris.Board;
import trieris.Direction;
import trieris.InvalidActionException;
import trieris.InvalidActionIndexException;
import trieris.CannotReverseException;
import trieris.Ship;
import trieris.TeamColor;
import trieris.Trieris;
import trieris.TrierisUI;

public class TrierisGamePanel extends JPanel implements TrierisUI {
    private BoardPanel refBoard;
    private PlayerBoardPanel playerBoard;
    private JPanel eastContainer;
    private JPanel emptyPanel;
    private Trieris trieris;
    private TeamColor col;
    
    public TrierisGamePanel(Trieris trieris) {
        this.trieris = trieris;
        this.setLayout(new BorderLayout());
        refBoard = new BoardPanel(trieris.getBoard());
        playerBoard = new PlayerBoardPanel(this, trieris);
        emptyPanel = new JPanel();
        eastContainer = new JPanel();
        
        JScrollPane scrollPane = null;
        if (refBoard != null) {
            scrollPane = new JScrollPane(refBoard);
            refBoard.setScrollPane(scrollPane);
        }
        if (scrollPane != null) {
            this.add(scrollPane, BorderLayout.CENTER);
        }
        if (playerBoard != null) {
            eastContainer.setLayout(new BorderLayout());
            eastContainer.add(playerBoard, BorderLayout.CENTER);
            eastContainer.add(emptyPanel, BorderLayout.SOUTH);
            this.add(eastContainer, BorderLayout.EAST);
        }
        
    }

    @Override
    public Ship promptPlayerShips(String message, List<Ship> ships) {
        return (Ship) displayChoiceMessage(message + "\nChoose a target ship.",
                "Choose target ship", ships.toArray());
    }

    @Override
    public int promptPlayerDirection(String message) {
        Direction choice = (Direction) displayChoiceMessage(message + "\nChoose a direction for your ship to face.",
                "Choose Direction", Direction.values());
        if (choice != null) {
            return choice.index;
        }
        return 0;
    }

    @Override
    public boolean promptPlayerCapture(String message) {
        return displayYesNoMessage(message + "\nCapture this port?", "Port Capture");
    }
    
    @Override
    public TeamColor promptPlayerTeam() {
        
        col = (TeamColor) displayChoiceMessage("Choose which team color you will be.", "Choose Team", TeamColor.values());
        playerBoard.changeShipColor(col);
        return col;
    }
    
    @Override
    public void updateMapDisplay() {
        refBoard.repaint();
    }

    @Override
    public void setShipAction(int shipIndex, int actionIndex, int actionType, int catapultDirection) 
            throws CannotReverseException, InvalidActionException, InvalidActionIndexException {
        trieris.setShipAction(shipIndex, actionIndex, actionType, catapultDirection);
        
    }
    
    @Override
    public void executeTurn() {
        if (!trieris.executeTurn()) {
            displayErrorMessage("Not all ships are ready.");
        }
    }

    @Override
    public void ready() {
        trieris.setAIActions();
    }
    
    private void displayErrorMessage(String msg) {
        JOptionPane.showMessageDialog(this, msg, "Error", JOptionPane.ERROR_MESSAGE);
    }
    
    private boolean displayYesNoMessage(String msg, String title) {
        int reply = JOptionPane.showConfirmDialog(null, msg, title, JOptionPane.YES_NO_OPTION);
        return reply == JOptionPane.YES_OPTION;
        
    }
    
    private Object displayChoiceMessage(String msg, String title, Object[] options) {
        Object obj = JOptionPane.showInputDialog(SwingUtilities.getWindowAncestor(this),
                msg, title, JOptionPane.PLAIN_MESSAGE,
                null, options, options[0]);
        if (obj == null) {
            return options[0];
        }
        return obj;
    }

    @Override
    public void victory(TeamColor color) {
        JOptionPane.showMessageDialog(this, color + " is victorious!", "Game Over", JOptionPane.PLAIN_MESSAGE);
    }
    
    
    

}

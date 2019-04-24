package trierisUI;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Component;
import java.awt.Container;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.Font;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.GridBagLayout;

import java.awt.GridLayout;
import java.awt.Image;
import java.awt.Rectangle;
import java.awt.Shape;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.image.BufferedImage;
import java.awt.image.ImageObserver;
import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.text.AttributedCharacterIterator;
import java.util.ArrayList;
import java.util.List;

import javax.imageio.ImageIO;
import javax.swing.AbstractButton;
import javax.swing.BorderFactory;
import javax.swing.ButtonGroup;
import javax.swing.Icon;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextArea;
import javax.swing.SwingConstants;
import javax.swing.SwingUtilities;
import javax.swing.border.BevelBorder;

import trieris.Ship;
import trieris.TeamColor;
import trieris.Trieris;
import trieris.TrierisUI;

public class PlayerBoardPanel extends JPanel {

    private TrierisUI t;
    private Trieris trieris;
    
    ActionsPanel ap;
    MovesPanel mp;
    ClickedButton click;
    ShipAction[][] sa = new ShipAction[5][6];
    ShipCatapultDirection[] direction = new ShipCatapultDirection[5];
    ButtonGroup[] ships = new ButtonGroup[5];
    JPanel[][] panels = new JPanel[5][6];
    JLabel[] icons = new JLabel[5];
    ImageIcon icon;
    JPanel[] subPanel = new JPanel[5];
    
    private int testingIndexX, testingIndexY;

    int phaseCount = 0;

    PlayerBoardPanel(TrierisUI trierisUI, Trieris trieris) {
        t = trierisUI;
        this.trieris = trieris;
        ap = new ActionsPanel();
        mp = new MovesPanel();
        click = new ClickedButton();
      
        
        setLayout(new BorderLayout());
        setPreferredSize(new Dimension(650, 500));
        add(ap, BorderLayout.WEST);
        add(mp, BorderLayout.CENTER);

    }
    
    protected void changeShipColor(TeamColor col) {
        
        JLabel[] icons = new JLabel[5];
                        
        if(col.equals(TeamColor.ORANGE)) {
            
            URL imageURL = getClass().getClassLoader().getResource("trierisUI/orangeShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else if (col.equals(TeamColor.BLACK)) {

            URL imageURL = getClass().getClassLoader().getResource("trierisUI/blackShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else if (col.equals(TeamColor.BLUE)) {
            
            URL imageURL = getClass().getClassLoader().getResource("trierisUI/blueShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else if (col.equals(TeamColor.YELLOW)) {
            
            URL imageURL = getClass().getClassLoader().getResource("trierisUI/yellowShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else if (col.equals(TeamColor.GREEN)) {
            
            URL imageURL = getClass().getClassLoader().getResource("trierisUI/greenShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else if (col.equals(TeamColor.RED)) {
            
            URL imageURL = getClass().getClassLoader().getResource("trierisUI/redShip.png");
            
            icon = new ImageIcon(imageURL);
        }
        
        else {
        }
        
        
        for (int i = 0; i < 5; i++) {
            
            icons[i] = new JLabel();
            icons[i].setIcon(icon);

            panels[i][5].add(icons[i]);
            panels[i][5].add(subPanel[i]);
            
            
            panels[i][5].revalidate();

        }
        
                
    }

    class MovesPanel extends JPanel {
        
        BufferedImage ship = null;

        MovesPanel() {

            GridLayout layout = new GridLayout(5, 6);
            setLayout(layout);

            for (int i = 0; i < 5; i++) {

                ships[i] = new ButtonGroup();

                for (int j = 0; j < 6; j++) {

                    panels[i][j] = new JPanel();
                    sa[i][j] = new ShipAction(i, j, j, i);

                    if (j == 0) {

                        int x = i + 1;

                        ShipDescription sd = new ShipDescription("Ship " + x, i);

                        panels[i][j].setLayout(new GridLayout(2, 0));
                        panels[i][j].add(sd.label).setLocation(1, 0);
                        panels[i][j].add(sd.b).setLocation(2, 0);


                        add(panels[i][j]).setLocation(i, j);
                    }

                    else if (j == 5) {
                        
                        subPanel[i] = new JPanel();
                        
                        subPanel[i].setLayout(new GridLayout(3,3));
                        subPanel[i].setOpaque(false);
                        
                        

                        direction[i] = new ShipCatapultDirection();

                        for (int k = 0; k < 9; k++) {

                            subPanel[i].add(direction[i].dir[k].rb);

                        }

                        // panels[i][j].setLayout(new BorderLayout());
                        // panels[i][j].add(subPanel, BorderLayout.CENTER);

                        

                        add(panels[i][j]).setLocation(i, j);
                    }

                    else {
                        Color btnColor = new Color(179, 198, 255);

                        panels[i][j].setLayout(new BorderLayout());

                        sa[i][j].b.setText("Action " + j);
                        sa[i][j].b.setFont(new Font("TimesRoman", Font.ITALIC, 14));
                        sa[i][j].b.setBorder(new BevelBorder(BevelBorder.RAISED));
                        sa[i][j].b.setBackground(btnColor);

                        ships[i].add(sa[i][j].rb);

                        panels[i][j].add(sa[i][j].b, BorderLayout.NORTH);
                        panels[i][j].add(sa[i][j].rb);
                        sa[i][j].rb.setHorizontalAlignment(AbstractButton.CENTER);

                        add(panels[i][j]).setLocation(i, j);
                    }
                }
            }
        }

    }

    class ActionsPanel extends JPanel {

        JButton straight = new JButton("Straight");
        JButton port = new JButton("Port");
        JButton sb = new JButton("Starboard");
        JButton hold = new JButton("Hold");
        JButton rev = new JButton("Reverse");
        JButton save = new JButton("Save Actions");
        JButton done = new JButton("Execute Phase");

        ActionsPanel() {

            GridLayout layout = new GridLayout(7, 1);
            setLayout(layout);

            add(straight);
            add(port);
            add(sb);
            add(hold);
            add(rev);
            add(save);
            add(done);

            designActionButtons();
            initListeners();
        }

        private void designActionButtons() {

            Font btnFont = new Font("TimesRoman", Font.ITALIC, 22);
            BevelBorder btnBorder = new BevelBorder(BevelBorder.RAISED);
            Color btnColor = new Color(233, 237, 170);

            straight.setFont(btnFont);
            straight.setBorder(btnBorder);
            straight.setBackground(btnColor);
            straight.setForeground(Color.BLACK);

            hold.setFont(btnFont);
            hold.setBorder(btnBorder);
            hold.setBackground(btnColor);
            hold.setForeground(Color.BLACK);

            port.setFont(btnFont);
            port.setBorder(btnBorder);
            port.setBackground(btnColor);
            port.setForeground(Color.BLACK);

            sb.setFont(btnFont);
            sb.setBorder(btnBorder);
            sb.setBackground(btnColor);
            sb.setForeground(Color.BLACK);

            rev.setFont(btnFont);
            rev.setBorder(btnBorder);
            rev.setBackground(btnColor);
            rev.setForeground(Color.BLACK);

            save.setFont(new Font("TimesRoman", Font.ITALIC, 20));
            save.setBorder(btnBorder);
            //save.setBackground(new Color(255, 77, 77));
            // save.setBackground(new Color(255, 51, 51));
            save.setBackground(new Color(255, 128, 128));
            save.setForeground(Color.BLACK);

            done.setFont(new Font("TimesRoman", Font.ITALIC, 18));
            done.setBorder(btnBorder);
            done.setBackground(new Color(71, 209, 71));
            done.setForeground(Color.BLACK);

        }

        private void selectNextButton() {
            try {
                int loopCountMax = 20;
                int loopCount = 0;
                do {
                    testingIndexX++;
                    if (testingIndexX == 5) {
                        testingIndexX = 0;
                        testingIndexY++;
                        if (testingIndexY == 5) {
                            testingIndexY = 0;
                        }
                        if (testingIndexX == 0) {
                            testingIndexX++;
                        }
                    }
                    loopCount++;
                } while (loopCount < loopCountMax && !sa[testingIndexY][testingIndexX].b.isEnabled());
                sa[testingIndexY][testingIndexX].listener.actionPerformed(null);
            } catch (Exception ex) {
                ex.printStackTrace();
            }
        }

        public void initListeners() {

            straight.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (click.b != null) {
                        click.b.setText(straight.getText());

                        selectNextButton();
                    }

                }
            });

            port.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (click.b != null) {
                        click.b.setText(port.getText());

                        selectNextButton();

                    }

                }
            });

            sb.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (click.b != null) {
                        click.b.setText(sb.getText());

                        selectNextButton();

                    }

                }
            });

            hold.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (click.b != null) {
                        click.b.setText(hold.getText());

                        selectNextButton();

                    }

                }
            });

            rev.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (click.b != null) {
                        click.b.setText(rev.getText());

                        selectNextButton();

                    }

                }
            });

            save.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    verifyActions();
                    t.ready();

                }
            });

            done.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    if (!save.isEnabled()) {
                        if (phaseCount == 3) {
                            t.executeTurn();
                            enableShipActions();
                            updateShipActionLabels();
                            updateActionButtons();
                            save.setEnabled(true);
                            phaseCount = 0;
                        }

                        else {
                            t.executeTurn();
                            phaseCount++;
                        }
                    }

                    else {
                        displayErrorMessage("Set Actions First");
                    }

                }
            });

        }

        protected void disableShipActions() {

            for (int i = 0; i < sa.length; i++) {

                for (int j = 0; j < sa[i].length; j++) {

                    sa[i][j].b.setEnabled(false);

                }

            }

        }

        protected void enableShipActions() {

            for (int i = 0; i < sa.length; i++) {

                for (int j = 0; j < sa[i].length; j++) {

                    sa[i][j].b.setEnabled(true);

                }

            }

        }

        protected void verifyActions() {

            try {
                for (int i = 0; i < sa.length; i++) {

                    int actionLength = sa[i][0].numOfActions;

                    for (int j = 1; j <= actionLength; j++) {

                        int catapultDir = -1;
                        int actionType = getActionType(sa[i][j].b.getText());

                        if (actionType == -1) {
                            throw new Exception("You Must Set All Actions");
                        }

                        if (sa[i][j].rb.isSelected()) {

                            for (int k = 0; k < direction[i].dir.length; k++) {

                                if (direction[i].dir[k].rb.isSelected()) {
                                    catapultDir = direction[i].dir[k].direction;
                                }
                            }

                            if (catapultDir == -1) {
                                throw new Exception("No Direction Set for Catapult");
                            }

                        }
                    }
                }

                setActions();
            } catch (Exception e) {
                displayErrorMessage(e.getMessage());
            }
        }

        protected void setActions() {

            try {

                for (int i = 0; i < sa.length; i++) {

                    int actionLength = sa[i][0].numOfActions;

                    for (int j = 1; j <= actionLength; j++) {

                        int ship = sa[i][j].ship;
                        int action = sa[i][j].action;
                        int actionType = getActionType(sa[i][j].b.getText());

                        int catapultDir = -1;

                        if (sa[i][j].rb.isSelected()) {

                            for (int k = 0; k < direction[i].dir.length; k++) {

                                if (direction[i].dir[k].rb.isSelected()) {
                                    catapultDir = direction[i].dir[k].direction;
                                }
                            }

                        }

                        t.setShipAction(ship, --action, actionType, catapultDir);
                         
                    }
                }
                save.setEnabled(false);
                disableShipActions();

            } catch (Exception e) {
                displayErrorMessage(e.getMessage());
                e.printStackTrace();
            }
        }

        protected void updateActionButtons() {
            List<Ship> playerShips = new ArrayList<>();
            playerShips = trieris.getPlayerShips();
            for (int i = 0; i < playerShips.size(); i++) {
                Ship ship = playerShips.get(i);
                int health = ship.getLife();
                sa[i][0].numOfActions = health;

                for (int j = 4; j > health && j >= 0; j--) {
                    sa[i][j].b.setEnabled(false);
                    sa[i][j].b.setText("X");
                    sa[i][j].b.setFont(new Font("TimesRoman", Font.ITALIC, 30));
                }

            }
        }

        protected void updateShipActionLabels() {

            for (int i = 0; i < sa.length; i++) {
                for (int j = 0; j < sa[i].length; j++) {

                    if (sa[i][j].b != null) {
                        sa[i][j].b.setBackground(new Color(179, 198, 255));
                        sa[i][j].b.setText("Action " + j);
                        sa[i][j].b.setFont(new Font("TimesRoman", Font.ITALIC, 14));
                    }
                }
            }

        }

        private void displayErrorMessage(String msg) {
            JOptionPane.showMessageDialog(SwingUtilities.getWindowAncestor(this), msg, "Error",
                    JOptionPane.ERROR_MESSAGE);
        }

        private int getActionType(String text) {

            int actionType = 0;

            if (text.equals("Straight")) {
                actionType = 1;
            } else if (text.equals("Port")) {
                actionType = 2;
            } else if (text.equals("Starboard")) {
                actionType = 3;
            } else if (text.equals("Hold")) {
                actionType = 4;
            } else if (text.equals("Reverse")) {
                actionType = 5;
            } else {
                actionType = -1;
            }

            return actionType;

        }

    }

    class ClickedButton {

        protected JButton b;
        protected int ship;
        protected int action;
        protected boolean catapult;

        ClickedButton() {

        }

    }

    class ShipAction {

        JButton b;
        JRadioButton rb;
        int ship;
        int action;
        int numOfActions;

        private int buttonIndexX, buttonIndexY;
        private ActionListener listener = new ActionListener() {

            @Override
            public void actionPerformed(ActionEvent e) {

                clearBtns();

                b.setBackground(Color.YELLOW);

                click.b = b;
                click.ship = ship;
                click.action = action;

                testingIndexX = ShipAction.this.buttonIndexX;
                testingIndexY = ShipAction.this.buttonIndexY;

            }

        };

        ShipAction(int ship, int action, int x, int y) {

            this.buttonIndexX = x;
            this.buttonIndexY = y;

            b = new JButton();
            b.setPreferredSize(new Dimension(50, 100));

            rb = new JRadioButton();
            this.ship = ship;
            this.action = action;
            numOfActions = 4;

            initListeners();
        }

        public void initListeners() {

            b.addActionListener(listener);

        }

        protected void clearBtns() {

            for (int i = 0; i < sa.length; i++) {
                for (int j = 0; j < sa[i].length; j++) {

                    if (sa[i][j].b != null) {
                        sa[i][j].b.setBackground(new Color(179, 198, 255));
                    }
                }
            }

        }

    }

    class ShipCatapultDirection {

        ButtonGroup group = new ButtonGroup();
        Directions[] dir = new Directions[9];

        public ShipCatapultDirection() {

            dir[0] = new Directions(7);
            dir[1] = new Directions(0);
            dir[2] = new Directions(1);

            dir[3] = new Directions(6);
            dir[4] = new Directions(8);
            dir[5] = new Directions(2);

            dir[6] = new Directions(5);
            dir[7] = new Directions(4);
            dir[8] = new Directions(3);

            for (int i = 0; i < dir.length; i++) {
                group.add(dir[i].rb);
            }

        }

    }

    class Directions {

        JRadioButton rb;
        int direction;

        Directions(int direction) {

            this.direction = direction;
            rb = new JRadioButton();
            rb.setOpaque(false);

        }

    }

    class ShipDescription {

        JLabel label;
        JLabel btnLabel;
        JButton b;
        int id;
        
        ShipDescription(String shipID, int id) {

            label = new JLabel(shipID, SwingConstants.CENTER);
            label.setFont(new Font("TimesRoman", Font.BOLD + Font.ITALIC, 18));
            
            
            btnLabel = new JLabel("<html>"+ "Reset Catapult" +"</html>");
            btnLabel.setFont(new Font("TimesRoman", Font.ITALIC, 16));
            
            
            b = new JButton();
            b.add(btnLabel);
            
            this.id = id;

            b.addActionListener(new ActionListener() {

                @Override
                public void actionPerformed(ActionEvent e) {

                    ships[id].clearSelection();
                    direction[id].group.clearSelection();

                }
            });

        }

    }
}

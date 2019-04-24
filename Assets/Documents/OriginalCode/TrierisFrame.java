package trierisUI;

import java.awt.Graphics;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.geom.AffineTransform;
import java.awt.image.AffineTransformOp;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.net.URL;

import javax.imageio.ImageIO;
import javax.swing.BorderFactory;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JPanel;

import trieris.NoTrierisInterfaceException;
import trieris.Trieris;
import trieris.TrierisUIFrame;

@SuppressWarnings("serial")
public class TrierisFrame extends JFrame implements TrierisUIFrame {

    private final InputStream backgroundFile = getClass().getResourceAsStream("background.jpg");
    private final InputStream logoFile = getClass().getResourceAsStream("logo.png");
    private final InputStream creditsFile = getClass().getResourceAsStream("creditsInfo.png");
    
    private BufferedImage background;
    private BufferedImage logoImage;
    private BufferedImage creditsImage;
    
	private JLabel logoLabel;
	private JLabel creditsLabel;
	
	private JButton newGameButton;
	private JButton exitGameButton;
	private JButton creditsButton;
	private JButton backButton;
	
	private Trieris currentGame;
	private JFrame currentGameFrame;
	private TrierisGamePanel currentGamePanel;
	

	
    public static void main(String[] args) throws IOException {
        
        final TrierisFrame frame = new TrierisFrame();

    }
    
	public TrierisFrame() throws IOException {
		
		setTitle("Trieris");
		setSize(1200, 651);
		setLocation(400, 200);
		setVisible(true);
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		
		MenuPanel menuPanel = new MenuPanel();
		setContentPane(menuPanel);
		
		repaint();
		revalidate();
	}

	@Override
    public void newGame() {
        setVisible(false);
        
        currentGameFrame = new JFrame();
        currentGameFrame.setTitle("Trieris");
        currentGameFrame.setSize(1400, 800);
        currentGameFrame.setLocation(400, 200);
        currentGameFrame.setExtendedState(JFrame.MAXIMIZED_BOTH); 
        currentGameFrame.setVisible(true);
        currentGameFrame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        
        JMenuBar menubar = new JMenuBar();
        JMenu menu = new JMenu("Menu");
        JMenuItem exit = new JMenuItem("Exit");
        exit.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent arg0) {
                endGame();
            }
        });
        menu.add(exit);
        menubar.add(menu);
        currentGameFrame.setJMenuBar(menubar);
        
        currentGame = new Trieris();
        currentGamePanel = new TrierisGamePanel(currentGame);
        currentGame.setUI(currentGamePanel);
        currentGameFrame.setContentPane(currentGamePanel);
        
        try {
            currentGame.start();
        } catch (NoTrierisInterfaceException e) {
            e.printStackTrace();
        }
        
    }
	

    @Override
    public void endGame() {
        if (currentGame != null && currentGameFrame != null) {
            currentGame = null;
            currentGameFrame.dispose();
            setVisible(true);
        }
    }

    @Override
    public void exit() {
        System.exit(0);
    }

    public class MenuPanel extends JPanel {

		public MenuPanel() {
		    setLayout(null);
            setImages();
            setLogo();
            setButtons();
            setCredits();
            add(logoLabel);
            add(newGameButton);
            add(exitGameButton);
            add(creditsButton);
            add(backButton);
            add(creditsLabel);
		}

		public void paintComponent(Graphics g) {
		    super.paintComponent(g);
	        g.drawImage(background, 0, 0, background.getWidth(null),
	                background.getHeight(null), null);
	        drawLogo(g);
		}
		
		private void setButtons() {
			setNewGameButton();
			setExitGameButton();
			setCreditsButton();
			setBackButton();
		}
		
		private void setImages() {
		    try {
                background = ImageIO.read(backgroundFile);
                logoImage = ImageIO.read(logoFile);
                creditsImage = ImageIO.read(creditsFile);
            } catch (IOException e) {
                e.printStackTrace();
            }
		}
		
		public void drawLogo(Graphics g) {
			int y = background.getHeight() - logoImage.getHeight();
			int x = background.getWidth() - logoImage.getWidth();
			g.drawImage(logoImage, x - 20, y - 45, null);
		}
		
		private void setLogo() {
			BufferedImage logo = null;
			try {
				logo = ImageIO.read(getClass().getResourceAsStream("Trieris_Small_light.png"));
			} catch (IOException e) {
				e.printStackTrace();
			}
			ImageIcon icon = new ImageIcon(resizeImage(logo, 80));
			logoLabel = new JLabel();
			logoLabel.setIcon(icon);
			logoLabel.setLocation(230, 10);
			logoLabel.setSize(850, 170);
		}
		
		private void setCredits() {
			ImageIcon icon = new ImageIcon(creditsImage);
			creditsLabel = new JLabel();
			creditsLabel.setIcon(icon);
			creditsLabel.setLocation(1, 1);
			creditsLabel.setSize(1185, 616);
			creditsLabel.setVisible(false);
		}

		private void setNewGameButton() {
			BufferedImage buttonIcon = null;
			try {
				buttonIcon = ImageIO.read(getClass().getResourceAsStream("newGame.png"));
			} catch (IOException e) {
				e.printStackTrace();
			}
			newGameButton = new JButton(new ImageIcon(buttonIcon));
			newGameButton.setBorder(BorderFactory.createEmptyBorder());
			newGameButton.setContentAreaFilled(false);
			newGameButton.setBounds(160, 270, 260, 113);
			
			newGameButton.addActionListener(new ActionListener() {

				@Override
				public void actionPerformed(ActionEvent e) {
				    newGame();
				}
			});
		}

		private void setExitGameButton() {
			BufferedImage buttonIcon = null;
			try {
				buttonIcon = ImageIO.read(getClass().getResourceAsStream("exitGame.png"));
			} catch (IOException e) {
				e.printStackTrace();
			}
			exitGameButton = new JButton(new ImageIcon(buttonIcon));
			exitGameButton.setBorder(BorderFactory.createEmptyBorder());
			exitGameButton.setContentAreaFilled(false);
			exitGameButton.setBounds(760, 272, 260, 113);
			exitGameButton.addActionListener(new ActionListener() {
				@Override
				public void actionPerformed(ActionEvent e) {
			        exit();
				}
			});
		}
		
		private void setCreditsButton() {
			BufferedImage buttonIcon = null;
			try {
				buttonIcon = ImageIO.read(getClass().getResourceAsStream("credits.png"));
			} catch (IOException e) {
				e.printStackTrace();
			}
			creditsButton = new JButton(new ImageIcon(buttonIcon));
			creditsButton.setBorder(BorderFactory.createEmptyBorder());
			creditsButton.setContentAreaFilled(false);
			creditsButton.setBounds(460, 275, 260, 113);
			creditsButton.addActionListener(new ActionListener() {
				@Override
				public void actionPerformed(ActionEvent e) {
					newGameButton.setVisible(false);
					exitGameButton.setVisible(false);
					creditsButton.setVisible(false);
					backButton.setVisible(true);
					creditsLabel.setVisible(true);
				}
			});
		}
		
		private void setBackButton() {
			BufferedImage buttonIcon = null;
			try {
				buttonIcon = ImageIO.read(getClass().getResourceAsStream("backButton.png"));
			} catch (IOException e) {
				e.printStackTrace();
			}
			backButton = new JButton(new ImageIcon(resizeImage(buttonIcon, 80)));
			backButton.setBorder(BorderFactory.createEmptyBorder());
			backButton.setContentAreaFilled(false);
			backButton.setBounds(465, 500, 260, 113);
			backButton.setVisible(false);
			backButton.addActionListener(new ActionListener(){
				@Override
				public void actionPerformed(ActionEvent e) {
					newGameButton.setVisible(true);
					exitGameButton.setVisible(true);
					creditsButton.setVisible(true);
					backButton.setVisible(false);
					creditsLabel.setVisible(false);
				}
			});
		}
	}
	
    public static BufferedImage resizeImage(BufferedImage image, int percent) {
		double scale = percent / 100.0;
		AffineTransform resize = AffineTransform.getScaleInstance(scale, scale);
		AffineTransformOp op = new AffineTransformOp(resize, AffineTransformOp.TYPE_BICUBIC);
		return op.filter(image, null);
	}

}

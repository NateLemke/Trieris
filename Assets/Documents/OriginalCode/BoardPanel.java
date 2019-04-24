package trierisUI;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.event.MouseWheelEvent;
import java.awt.event.MouseWheelListener;
import java.awt.geom.AffineTransform;
import java.awt.geom.NoninvertibleTransformException;
import java.awt.geom.Point2D;
import java.awt.image.AffineTransformOp;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashSet;

import javax.imageio.ImageIO;

import trieris.Board;
import trieris.Node;
import trieris.Port;
import trieris.Ship;
import trieris.TeamColor;

@SuppressWarnings("serial")
public class BoardPanel extends DragPanel {

	private static final double QUARTER_RADIANS = Math.PI / 4;
	private static final int FONT_SIZE = 20;

	AffineTransform tx = new AffineTransform();
	
    private InputStream backgroundFile = getClass().getResourceAsStream("board.jpg");
    private final InputStream blackShipFile = getClass().getResourceAsStream("blackShip.png");
    private final InputStream blueShipFile = getClass().getResourceAsStream("blueShip.png");
    private final InputStream greenShipFile = getClass().getResourceAsStream("greenShip.png");
    private final InputStream orangeShipFile = getClass().getResourceAsStream("orangeShip.png");
    private final InputStream redShipFile = getClass().getResourceAsStream("redShip.png");
    private final InputStream yellowShipFile = getClass().getResourceAsStream("yellowShip.png");

    private final InputStream blackCapitalFile = getClass().getResourceAsStream("blackCap.png");
    private final InputStream blueCapitalFile = getClass().getResourceAsStream("blueCap.png");
    private final InputStream greenCapitalFile = getClass().getResourceAsStream("greenCap.png");
    private final InputStream orangeCapitalFile = getClass().getResourceAsStream("orangeCap.png");
    private final InputStream redCapitalFile = getClass().getResourceAsStream("redCap.png");
    private final InputStream yellowCapitalFile = getClass().getResourceAsStream("yellowCap.png");

    private final InputStream blackPortFile = getClass().getResourceAsStream("blackPort.png");
    private final InputStream bluePortFile = getClass().getResourceAsStream("bluePort.png");
    private final InputStream greenPortFile = getClass().getResourceAsStream("greenPort.png");
    private final InputStream orangePortFile = getClass().getResourceAsStream("orangePort.png");
    private final InputStream redPortFile = getClass().getResourceAsStream("redPort.png");
    private final InputStream yellowPortFile = getClass().getResourceAsStream("yellowPort.png");

    private final InputStream healthPointFile = getClass().getResourceAsStream("hitPoint.png");
	

	private BufferedImage background;
	// private BufferedImage screen;

	private BufferedImage blackShipImage;
	private BufferedImage blueShipImage;
	private BufferedImage greenShipImage;
	private BufferedImage orangeShipImage;
	private BufferedImage redShipImage;
	private BufferedImage yellowShipImage;

	private BufferedImage blackCapitalImage;
	private BufferedImage blueCapitalImage;
	private BufferedImage greenCapitalImage;
	private BufferedImage orangeCapitalImage;
	private BufferedImage redCapitalImage;
	private BufferedImage yellowCapitalImage;

	private BufferedImage blackPortImage;
	private BufferedImage bluePortImage;
	private BufferedImage greenPortImage;
	private BufferedImage orangePortImage;
	private BufferedImage redPortImage;
	private BufferedImage yellowPortImage;

	private BufferedImage healthPointImage;
	
	private Board board;

	private boolean fullyZoom;

	public BoardPanel(Board board) {
//		this.addMouseWheelListener(new ZoomHandler());
		this.board = board;
		this.setPreferredSize(new Dimension(1500, 1000));
		setBackground();
		fullyZoom = false;
        setImages();
		repaint();
		// screen = getScreen();
	}

	@Override
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		Graphics2D g2d = (Graphics2D) g.create();

		if (fullyZoom) {
			g2d.drawImage(background, 0, 0, this);
			tx = new AffineTransform();
			fullyZoom = false;
		} else {
			g2d.drawImage(background, tx, this);
		}

		g2d.dispose();
		// drawNode(g);
		drawPorts(g);
		drawShips(g);

	}

	public void drawNode(Graphics g) {

		int radius;
		((Graphics2D) g).setStroke(new BasicStroke(2));
		for (Node node : board.getAllNodes()) {
			radius = 12;
			g.setColor(node.getColor());
			int x = node.getX() * 60 + 25;
			int y = node.getY() * 60 + 25;
			if (node.getPort() != null && node.getPort().getCapital() == true) {
				radius = 20;
				x = x - 5;
				y = y - 5;
			} else if (node.isIsland()) {
				continue;
			}
			g.fillOval(y, x, radius, radius);

			for (int i = 0; i <= 7; i++) {
				Node adjacentNode = node.getAdjacentNode(i);
				if (adjacentNode != null) {
					g.setColor(Color.BLACK);
					int adjacentX = adjacentNode.getX() * 60 + 31;
					int adJacentY = adjacentNode.getY() * 60 + 31;
					g.drawLine(y + 6, x + 6, adJacentY, adjacentX);
				}
			}
		}

	}

	public void drawShips(Graphics g) {

		HashSet<Node> uniqueNodes = new HashSet<Node>();

		for (Ship ship : board.getAllShips()) {
			uniqueNodes.add(ship.getNode());
		}

		for (Node node : uniqueNodes) {
			if (node.getShips().size() > 1) {

				drawMultipleShips(node, g);

			} else if (node.getShips().size() == 1) {

				Ship ship = node.getShips().get(0);

				drawShip(getShipImage(ship), g, ship);

			}
		}
	}

	public void drawPorts(Graphics g) {

		for (Node node : board.getAllPortNodes()) {
			
			Port port = node.getPort();
			
			if(port.getCapital()) {
				g.drawImage(getCapitalImage(port), node.getY() * 60 + 16, node.getX() * 60 + 18, null);
			} else {
				g.drawImage(getPortImage(port), node.getY() * 60 + 16, node.getX() * 60 + 18, null);
			}
		}
	}
	
	public void setImages() {
		try {
			blackShipImage = ImageIO.read(blackShipFile);
			blueShipImage = ImageIO.read(blueShipFile);
			greenShipImage = ImageIO.read(greenShipFile);
			orangeShipImage = ImageIO.read(orangeShipFile);
			redShipImage = ImageIO.read(redShipFile);
			yellowShipImage = ImageIO.read(yellowShipFile);

			healthPointImage = ImageIO.read(healthPointFile);

			blackCapitalImage = ImageIO.read(blackCapitalFile);
			blueCapitalImage = ImageIO.read(blueCapitalFile);
			greenCapitalImage = ImageIO.read(greenCapitalFile);
			orangeCapitalImage = ImageIO.read(orangeCapitalFile);
			redCapitalImage = ImageIO.read(redCapitalFile);
			yellowCapitalImage = ImageIO.read(yellowCapitalFile);

			blackPortImage = ImageIO.read(blackPortFile);
			bluePortImage = ImageIO.read(bluePortFile);
			greenPortImage = ImageIO.read(greenPortFile);
			orangePortImage = ImageIO.read(orangePortFile);
			redPortImage = ImageIO.read(redPortFile);
			yellowPortImage = ImageIO.read(yellowPortFile);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void setBackground() {
		try {
			background = ImageIO.read(backgroundFile);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

//	private class ZoomHandler implements MouseWheelListener {
//
//		double scale = 1.0;
//
//		public void mouseWheelMoved(MouseWheelEvent e) {
//			if (e.getScrollType() == MouseWheelEvent.WHEEL_UNIT_SCROLL) {
//				if (e.getWheelRotation() == 1 && scale <= 1) {
//					fullyZoom = true;
//					BoardPanel.this.revalidate();
//					BoardPanel.this.repaint();
//					return;
//				}
//				Point2D p1 = e.getPoint();
//				Point2D p2 = null;
//				try {
//					p2 = tx.inverseTransform(p1, null);
//				} catch (NoninvertibleTransformException ex) {
//					ex.printStackTrace();
//					return;
//				}
//
//				scale -= (0.1 * e.getWheelRotation());
//				scale = Math.max(0.1, scale);
//
//				tx.setToIdentity();
//				tx.translate(p1.getX(), p1.getY());
//				tx.scale(scale, scale);
//				tx.translate(-p2.getX(), -p2.getY());
//
//				BoardPanel.this.revalidate();
//				BoardPanel.this.repaint();
//			}
//		}
//	}

	private BufferedImage getShipImage(Ship ship) {

		TeamColor color = ship.getTeamColor();

		switch (color) {
		case BLACK:
			return blackShipImage;
		case BLUE:
			return blueShipImage;
		case GREEN:
			return greenShipImage;
		case ORANGE:
			return orangeShipImage;
		case RED:
			return redShipImage;
		case YELLOW:
			return yellowShipImage;
		default:
			return null;
		}
	}

	private BufferedImage getPortImage(Port port) {

		TeamColor color = port.getTeamColor();

		switch (color) {
		case BLACK:
			return blackPortImage;
		case BLUE:
			return bluePortImage;
		case GREEN:
			return greenPortImage;
		case ORANGE:
			return orangePortImage;
		case RED:
			return redPortImage;
		case YELLOW:
			return yellowPortImage;
		default:
			return null;
		}
	}

	private BufferedImage getCapitalImage(Port port) {
		
		TeamColor color = port.getTeamColor();

		switch (color) {
		case BLACK:
			return blackCapitalImage;
		case BLUE:
			return blueCapitalImage;
		case GREEN:
			return greenCapitalImage;
		case ORANGE:
			return orangeCapitalImage;
		case RED:
			return redCapitalImage;
		case YELLOW:
			return yellowCapitalImage;
		default:
			return null;
		}
	}
	
	private void drawShip(BufferedImage image, Graphics g, Ship ship) {
		AffineTransform transform = new AffineTransform();
		transform.rotate(ship.getFront() * QUARTER_RADIANS, image.getWidth() / 2, image.getHeight() / 2);
		AffineTransformOp op = new AffineTransformOp(transform, AffineTransformOp.TYPE_BILINEAR);
		image = op.filter(image, null);
		g.drawImage(image, ship.getNode().getY() * 60, ship.getNode().getX() * 60, null);
		numberShips(g, ship);
		drawHealth(g, ship);
	}

	private void drawMultipleShips(Node node, Graphics g) {

		ArrayList<ShipImage> shipsInNode = new ArrayList<ShipImage>();
		BufferedImage image;
		int scale = 15;
		int percent = 100;
		double fontSize = FONT_SIZE;
		for(int i = 1; i < node.getShips().size(); i++) {
			percent = percent * (100 - scale) / 100;
		}
		fontSize = fontSize * ((double) percent / 100);
		for (int i = 0; i < node.getShips().size(); i++) {
			AffineTransform transform = new AffineTransform();
			Ship ship = node.getShips().get(i);
			image = resizeImage(getShipImage(ship), percent);
			transform.rotate(ship.getFront() * QUARTER_RADIANS, image.getWidth() / 2, image.getHeight() / 2);
			AffineTransformOp op = new AffineTransformOp(transform, AffineTransformOp.TYPE_BILINEAR);
			image = op.filter(image, null);
			int x = ship.getNode().getY() * 60 + ((scale * percent / 100) * i);
			int y = ship.getNode().getX() * 60 + 10;
			g.drawImage(image, x, y, null);
			shipsInNode.add(new ShipImage(x, y, ship.getID(), ship.getLife()));
		}
		addMultipleShipDetails(g, shipsInNode, (int) fontSize);
	}

	private void numberShips(Graphics g, Ship ship) {
		g.setColor(Color.BLACK);
		g.setFont(new Font("Sans-serif", Font.BOLD, FONT_SIZE));
		g.drawString("" + ship.getID(), ship.getNode().getY() * 60 + 45, ship.getNode().getX() * 60 + 55);
	}

	private void drawHealth(Graphics g, Ship ship) {
		g.setColor(Color.RED);
		g.setFont(new Font("Sans-serif", Font.BOLD, FONT_SIZE));
		g.drawString("" + ship.getLife(), ship.getNode().getY() * 60 + 10, ship.getNode().getX() * 60 + 20);
	}
	
	private void addMultipleShipDetails(Graphics g, ArrayList<ShipImage> ships, int fontSize) {
		g.setFont(new Font("Sans-serif", Font.BOLD, fontSize));
		for(ShipImage ship : ships) {
			g.setColor(Color.BLACK);
			g.drawString("" + ship.getId(), ship.getX() + 45 - (ships.size() * 5), ship.getY() + 50);
			g.setColor(Color.RED);
			g.drawString("" + ship.getHealth(), ship.getX(), ship.getY());
		}
	}
	
	private BufferedImage getScreen() {
		BufferedImage screen = new BufferedImage(1264, 957, BufferedImage.TYPE_INT_ARGB);
		this.paint(screen.getGraphics());
		return screen;
	}

	public static BufferedImage resizeImage(BufferedImage image, int percent) {
		double scale = percent / 100.0;
		AffineTransform resize = AffineTransform.getScaleInstance(scale, scale);
		AffineTransformOp op = new AffineTransformOp(resize, AffineTransformOp.TYPE_BICUBIC);
		return op.filter(image, null);
	}
}
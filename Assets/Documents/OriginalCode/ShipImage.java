package trierisUI;

public class ShipImage {

	private int x;
	private int y;
	private int xForLife;
	private int yForLife;
	private int xForId;
	private int yForId;
	private int id;
	private int health;

	public ShipImage() {

	}

	public ShipImage(int x, int y, int id, int health) {
		this.x = x;
		this.y = y;
		this.id = id;
		this.health = health;
	}

	public ShipImage(int xLife, int yLife, int xId, int yId, int id, int health) {
		xForLife = xLife;
		yForLife = yLife;
		xForId = xId;
		yForId = yId;
		this.id = id;
		this.health = health;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public int getXLife() {
		return xForLife;
	}

	public int getYLife() {
		return yForLife;
	}

	public int getXId() {
		return xForId;
	}
	
	public int getYId() {
		return yForId;
	}

	public int getId() {
		return id;
	}

	public int getHealth() {
		return health;
	}
}

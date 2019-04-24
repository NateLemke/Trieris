package trieris;

import java.awt.Color;

public enum TeamColor {
    ORANGE(Color.ORANGE, "orangePorts.txt"),
    BLACK(Color.GRAY, "blackPorts.txt"),
    BLUE(Color.BLUE, "bluePorts.txt"),
    YELLOW(Color.YELLOW, "yellowPorts.txt"),
    GREEN(Color.GREEN, "greenPorts.txt"),
    RED(Color.RED, "redPorts.txt");
	
	public final Color color;
	final String portFile;
	
	public Color getColor() {
	    return color;
	}
	
	TeamColor(Color color, String portFile) {
		this.color = color;
		this.portFile = portFile;
	}
}

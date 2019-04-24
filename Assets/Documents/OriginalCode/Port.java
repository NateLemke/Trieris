package trieris;

import java.awt.Color;

public class Port {

    private boolean capital;
    private TeamColor tColor;
    
    private Color color;
    
    public Port() {
    }
    
    public Port(boolean isCapital, TeamColor teamColor) {
        setCapital(isCapital);
        setTeamColor(teamColor);
        setColor(teamColor.color);
    }
    
    public boolean getCapital() {
        return capital;
    }
    
    public TeamColor getTeamColor() {
        return tColor;
    }
    
    public Color getColor() {
        return color;
    }
    
    public void setCapital(boolean isCapital) {
        capital = isCapital;
    }
    
    public void setTeamColor(TeamColor teamColor) {
        tColor = teamColor;
    }
    
    public void setColor(Color color) {
        this.color = color;
    }

}

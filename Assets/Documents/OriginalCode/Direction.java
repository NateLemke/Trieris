package trieris;

public enum Direction {
    
    NORTH  (0, -1, 0),      //
    NORTHEAST  (1, -1, 1), //
    EAST   (2, 0, 1),       //
    SOUTHEAST  (3, 1, 1), //
    SOUTH   (4, 1, 0),      //
    SOUTHWEST  (5, 1, -1), //
    WEST   (6, 0, -1),       //
    NORTHWEST  (7, -1, -1); //
    
    public final int index;
    final int dx, dy;
    
    Direction(int index, int dx, int dy) {
        this.index = index;
        this.dx = dx;
        this.dy = dy;
    }
    
    @Override
    public String toString() {
        return this.name();
    }
}

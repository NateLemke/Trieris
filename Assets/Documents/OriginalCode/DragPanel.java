package trierisUI;

import java.awt.Point;
import java.awt.Rectangle;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JViewport;

public class DragPanel extends JPanel implements MouseListener, MouseMotionListener {
    private JScrollPane scroll;
    private JViewport viewport;
    private Point viewportPoint;
    private int mouseStartX;
    private int mouseStartY;
    
    public DragPanel() {
        this.addMouseListener(this);
        this.addMouseMotionListener(this);
        
    }
    
    public void setScrollPane(JScrollPane scroll) {
        this.scroll = scroll;
        this.scroll.getVerticalScrollBar().setUnitIncrement(16);
    }

    @Override
    public void mouseDragged(MouseEvent e) {
        try {
            viewport = scroll.getViewport();
            viewportPoint = viewport.getViewPosition();
            viewportPoint.translate(mouseStartX - e.getX(), mouseStartY - e.getY());
            scrollRectToVisible(new Rectangle(viewportPoint, viewport.getSize()));
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        
    }

    @Override
    public void mousePressed(MouseEvent e) {
        mouseStartX = e.getX();
        mouseStartY = e.getY();
        
    }

    @Override
    public void mouseMoved(MouseEvent e) { }

    @Override
    public void mouseClicked(MouseEvent e) { }

    @Override
    public void mouseEntered(MouseEvent e) { }

    @Override
    public void mouseExited(MouseEvent e) { }

    @Override
    public void mouseReleased(MouseEvent e) { }
}

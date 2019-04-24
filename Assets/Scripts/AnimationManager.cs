using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // stack for animations to be processed
    // stack for non-combat animation
    // stack for combat animations


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if combat animations.count > 0 AND animation playing == false
            // being next combat animation
    }

    public void playingAnimation() {
        // foreach ship
            // if playinganimation == true
                // return true
        // return false
    }

    public void prepareAnmations() {

        // also need to assign animation destinations 

        // foreach animation in animations to be processed
            // if animation.ship has taken or delt damage
                // add to combat animation stack
            // else
                // add to non-combat animation stack
                // sort combat animations by location??
                // from left to right??
    }

    public void runAnimations() {
        prepareAnmations();
        // foreach animation in non-combat animations
            // start animation
        // if combat animations.count > 0
            // start first combat animation
    }

    private void drawMultipleShips(Node node,Graphics g) {

        List<Ship> shipsInNode = new List<Ship>();
        //BufferedImage image;
        int scale = 15;
        int percent = 100;
        //double fontSize = FONT_SIZE;
        for (int i = 1; i < node.getShips().Count; i++) {
            percent = percent * (100 - scale) / 100;
        }
        //fontSize = fontSize * ((double)percent / 100);
        for (int i = 0; i < node.getShips().Count; i++) {
            //AffineTransform transform = new AffineTransform();
            Ship ship = node.getShips()[i];
            //image = resizeImage(getShipImage(ship),percent);
            //transform.rotate(ship.getFront() * QUARTER_RADIANS,image.getWidth() / 2,image.getHeight() / 2);
            //AffineTransformOp op = new AffineTransformOp(transform,AffineTransformOp.TYPE_BILINEAR);
            //image = op.filter(image,null);
            int x = ship.getNode().getY() * 60 + ((scale * percent / 100) * i);
            int y = ship.getNode().getX() * 60 + 10;
            //g.drawImage(image,x,y,null);
           // shipsInNode.add(new ShipImage(x,y,ship.getID(),ship.getLife()));
        }
        //addMultipleShipDetails(g,shipsInNode,(int)fontSize);
    }





}

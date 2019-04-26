﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimationManager 
{
    public static Dictionary<Ship,Animation> actionAnimations = new Dictionary<Ship,Animation>();
    //public static List<Animation> shipAnimations = new List<Animation>();
    static List<CombatResolution> rammingResolutions = new List<CombatResolution>();

    //public static AnimationManager main;

    //public static void Awake() {
    //    main = this;
    //}

    public static bool playingAnimation = false;


    public static IEnumerator playAnimations() {
        playingAnimation = true;
        yield return playCombatAnims();
        yield return playNonCombatAnims();
        playingAnimation = false;
        actionAnimations.Clear();
        rammingResolutions.Clear();
        yield return null;
    }

    static IEnumerator playCombatAnims() {

        // sort list

        for (int i = 0; i < rammingResolutions.Count;i++) {
            yield return rammingResolutions[i].resolve();            
        }

        yield return null;
    }

    static IEnumerator playNonCombatAnims() {
        List<Animation> anims = actionAnimations.Values.ToList();
        for (int i = 0; i < anims.Count; i++) {
            yield return anims[i].playAnimation();
        }
        yield return null;
    }

    public static void addCombat() {

    }

    private static void drawMultipleShips(Node node,Graphics g) {

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

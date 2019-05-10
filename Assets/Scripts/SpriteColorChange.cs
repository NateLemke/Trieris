using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChange : MonoBehaviour
{
    public Color c;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        Sprite s = sr.sprite;
        Texture2D t = new Texture2D(s.texture.width,s.texture.height);

        //Sprite2 = 
        
        for(int y = 0; y < t.height; y++) {
            for(int x = 0; x < t.width; x++) {
                //if( y > 10 && y < 200) {
                //    t.SetPixel(x,y,Color.red);
                //} else {
                //    t.SetPixel(x,y,s.texture.GetPixel(x,y));
                //}
                if(s.texture.GetPixel(x,y) == Color.magenta) {
                    t.SetPixel(x,y,c);
                } else {
                    t.SetPixel(x,y,s.texture.GetPixel(x,y));
                }
            }
        }
        t.Apply();
        sr.sprite = Sprite.Create(t,new Rect(0f,0f,t.width,t.height), Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

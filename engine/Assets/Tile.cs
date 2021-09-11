using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    
        private int x;
        private int y;

        public Sprite blank;
        public Sprite red;
        public Sprite black;

        public Image self;

        Game b;
        public void Init(int y, int x){   
            b =  GameObject.Find("Board").GetComponent<Game>();         
            this.y = y;
            this.x = x;
            self.sprite = blank;
        }
        public int GetX(){
            return x;
        }
        public int GetY(){
            return y;
        }

    public void onClick()
    {
        if (b.winner != ((int)Game.color.BLANK))
        {
            return;
        }
        if (b.board[y, x] != ((int)Game.color.BLANK))
        {
            return;
        }

        if (b.currentMoveColor == ((int)Game.color.RED))
        {
            //set sprite to red
            self.sprite = red;
        }
        else
        {
            self.sprite = black;
        }
        b.move(y, x);

    }

}

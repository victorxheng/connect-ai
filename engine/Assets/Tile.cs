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

    Game game;
    Minimax ai;
    public void Init(int y, int x)
    {
        game = GameObject.Find("Board").GetComponent<Game>();
        ai = GameObject.Find("Board").GetComponent<Minimax>();
        this.y = y;
        this.x = x;
        self.sprite = blank;
    }
    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }

    public void onClick()
    {
        if (game.winner != ((int)Game.color.BLANK))
        {
            return;
        }
        if (game.b.board[y, x] != ((int)Game.color.BLANK))
        {
            return;
        }

        if (game.b.moveColor == ((int)Game.color.RED))
        {
            //set sprite to red
            self.sprite = red;
        }
        else
        {
            self.sprite = black;
        }
        game.b = game.b.Move(new Point(y, x));

        //max score of board: 100,000,000 (3^16)
        if (Mathf.Abs(game.b.score) > 100000000)
        {
            Debug.Log("Player won: " + game.b.score);
            game.winner = game.b.score > 0 ? (int)Game.color.BLACK : (int)Game.color.RED;
        }

        game.b = ai.ComputeMove(game.b);
    }

}

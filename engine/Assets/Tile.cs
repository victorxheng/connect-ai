using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    private int x;
    private int y;

    public Image self;

    Game game;
    Minimax ai;
    public void Init(int y, int x)
    {
        game = GameObject.Find("Board").GetComponent<Game>();
        ai = GameObject.Find("Board").GetComponent<Minimax>();
        this.y = y;
        this.x = x;
        self.sprite = game.blank;
    }

    public void onClick()
    {
        if (game.AiThinking)
        {
            return;
        }
        if (game.winner != ((int)Game.color.BLANK))
        {
            return;
        }
        if (game.b.board[y, x] != ((int)Game.color.BLANK))
        {
            return;
        }
        game.PlayerClick(self, y, x);
    }

}

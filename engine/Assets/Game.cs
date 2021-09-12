using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public const int rows = 20;
    public const int cols = 20;
    public const int winCount = 5;

    public Board b;

    private bool showBoard = true;

    public GameObject horizontalLayoutGroup;
    public GameObject tile;
    public Transform verticalLayoutGroup;


    public int winner = ((int)color.BLANK);

    public Minimax ai;

    public Dictionary<(int,int), Tile> Layout;


    public Sprite blank;
    public Sprite red;
    public Sprite black;

    public enum color
    { //blank is 0
        BLANK = 0,
        RED = 1,
        BLACK = 2
    }
    void Start()
    {
        newBoard();
        buildBoard();
    }
    private void newBoard()
    {
        b = new Board(new int[rows, cols], new HashSet<Point>(), 0, (int) color.RED);
    }

    private void buildBoard() // renders the board on screen. clears anything already there
    {
        if (!showBoard)
        {
            return;
        }
        Layout = new Dictionary<(int,int), Tile>();

        foreach (GameObject g in verticalLayoutGroup)
        {//clear board
            Destroy(g);
        }
        //populate layout with buttons
        for (int i = 0; i < rows; i++)
        {
            var group = GameObject.Instantiate(horizontalLayoutGroup, verticalLayoutGroup);
            Transform t = group.GetComponent<Transform>();
            for (int j = 0; j < cols; j++)
            {
                var square = GameObject.Instantiate(tile, t);
                square.name = i+","+j;
                Tile v = square.GetComponent<Tile>();
                v.Init(i, j);
                Layout.Add((i,j), v);
            }
        }
    }
    public bool AiThinking = false;
    public void PlayerClick(Image self, int y, int x)
    {
        if (b.moveColor == ((int)Game.color.RED))
        {
            self.sprite = red;
        }
        else
        {
            self.sprite = black;
        }

        b = b.Move(new Point(y, x));

        //max score of board: 100,000,000 (3^16)
        if (Mathf.Abs(b.score) > 100000000)
        {
            Debug.Log("Player won: " + b.score);
            winner = b.score > 0 ? (int)Game.color.BLACK : (int)Game.color.RED;
        }

        AiThinking = true;
        b = ai.ComputeMove(b);
        AiThinking = false;

        //max score of board: 100,000,000 (3^16)
        if (Mathf.Abs(b.score) > 100000000)
        {
            Debug.Log("Player won: " + b.score);
            winner = b.score > 0 ? (int)Game.color.BLACK : (int)Game.color.RED;
        }
        b.PrintBoard();
    }

    public void AiMove(Point p)
    {
        Tile t = Layout[(p.y,p.x)];
        if (b.moveColor == ((int)Game.color.RED))
        {
            t.self.sprite = red;
        }
        else
        {
            t.self.sprite = black;
        }
    }
}


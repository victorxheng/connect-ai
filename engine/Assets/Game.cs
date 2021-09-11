using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Minimax minimax;

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
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public const int rows = 20;
    public const int cols = 20;
    public const int winCount = 5;

    public int[,] board; // [x] [y]

    public int blackScore;
    public int redScore;

    public HashSet<Point> blackPieces;
    public HashSet<Point> redPieces;
    private bool showBoard = true;
    public int currentMoveColor;

    public GameObject horizontalLayoutGroup;
    public GameObject tile;
    public Transform verticalLayoutGroup;


    public Sprite blankSprite;
    public Sprite redSprite;
    public Sprite blackSprite;
    public Sprite blueSprite;

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
        currentMoveColor = ((int)color.RED);//starting color
        newBoard();
        buildBoard();
    }
    private void newBoard()
    {
        board = new int[rows, cols];
        blackPieces = new HashSet<Point>();
        redPieces = new HashSet<Point>();
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
    public void move(int y, int x)
    {
        minimax.clearValidMoves();
        if (currentMoveColor == ((int)color.RED)){
            board[y, x] = ((int)color.RED);//move red
            redPieces.Add(new Point(y,x));
        }
        else{
            board[y, x] = ((int)color.BLACK);//move black
            blackPieces.Add(new Point(y,x));
        }
        bool winningMove = evaluateForWin(y, x, currentMoveColor);
        if (winningMove)
        {
            winner = currentMoveColor;
            Debug.Log("Player Won!");
        }
        //switch turns
        currentMoveColor = currentMoveColor == ((int)color.RED) ? ((int)color.BLACK) : ((int)color.RED);
        //minimax.showValidMoves();
    }

    private bool evaluateForWin(int y, int x, int moveColor)
    {//CAN BE CLEANED UP TO NOT BRUTE FORCE
        if (checkDirection(y, x, moveColor, 0, 1)) return true;
        if (checkDirection(y, x, moveColor, 1, 0)) return true;
        if (checkDirection(y, x, moveColor, 1, 1)) return true;
        if (checkDirection(y, x, moveColor, -1, 1)) return true;
        //Debug.Log($"Red Score: {redScore}; Black Score: {blackScore}");
        return false;
    }
    private bool checkDirection(int y, int x, int moveColor, int yChange, int xChange)
    {
        int count = 1;
        int yPointer = y;
        int xPointer = x;
        while (yPointer + yChange >= 0 && yPointer + yChange < rows && xPointer + xChange >= 0 && xPointer + xChange < cols && board[yPointer + yChange, xPointer + xChange] == moveColor)
        {
            count++;
            yPointer += yChange;
            xPointer += xChange;
        }
        yPointer = y;
        xPointer = x;
        while (yPointer - yChange >= 0 && yPointer - yChange < rows && xPointer - xChange >= 0 && xPointer - xChange < cols && board[yPointer - yChange, xPointer - xChange] == moveColor)
        {
            count++;
            yPointer -= yChange;
            xPointer -= xChange;
        }
        if(moveColor == (int)color.RED)
        {
            redScore += (int)Mathf.Pow(3,count - 1)-1;

            if (count >= winCount)
            {
                redScore += 1000;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            blackScore += (int)Mathf.Pow(3, count - 1)-1;

            if (count >= winCount)
            {
                blackScore += 1000;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimax : MonoBehaviour
{
    public Game game;


    private void ComputeMove(){
        //loop through all numbers and choose the max num
    }

    //params: board, black pieces, red pieces, black score, red score, depth
    private int MiniMax(Board b, int depth){//MINIMAX RECURSION
        //check depth: base case
        if(depth == 1)
        {
           return b.score;
        }
        //create list of all valid moves for given board
        //loop through each valid move 
        //develop a new board and new heuristic and passing it down
        //check for largest/smallest value and return that value

        //alpha beta pruning
        //check value above and compare it based on min or max: prune
        return 0;
    }

    private HashSet<Point> validMoves(int[,] b, HashSet<Point> pieces)
    {
        HashSet<Point> o = new HashSet<Point>(); 
        foreach (Point p in pieces)
        {
            checkPoint(o, b, p, 1, 1);
            checkPoint(o, b, p, 0, 1);
            checkPoint(o, b, p, 1, 0);
            checkPoint(o, b, p, -1, -1);
            checkPoint(o, b, p, 0, -1);
            checkPoint(o, b, p, -1, 0);
            checkPoint(o, b, p, -1, 1);
            checkPoint(o, b, p, 1, -1);
        }
        return o;
    }
    private void checkPoint(HashSet<Point> o, int[,] b, Point p, int yChange, int xChange)
    {
        //checks if position is a valid move (touching another piece)
        if (p.y + yChange < Game.rows && p.x + xChange < Game.cols &&
            p.y + yChange >= 0 && p.x + xChange >= 0 &&
            b[p.y + yChange, p.x + xChange] == ((int)Game.color.BLANK))
            o.Add(new Point(p.y + yChange, p.x + xChange));
    }

    /*
    //FOR TESTING PURPOSES
    HashSet<Point> moves;
    public void showValidMoves()
    {
        moves = validMoves(game.board, game.blackPieces, game.redPieces);
        foreach (Point p in moves)
        {
            GameObject g = GameObject.Find(p.y + "," + p.x);
            Image img = g.GetComponent<Image>();
            img.sprite = game.blueSprite;
        }
    }
    public void clearValidMoves()
    {
        if (moves != null)
        {
            foreach (Point p in moves)
            {
                GameObject g = GameObject.Find(p.y + "," + p.x);
                Image img = g.GetComponent<Image>();
                img.sprite = game.blankSprite;
            }
        }
    }*/
}
public class Point
{
    public int x;
    public int y;
    public Point(int y, int x)
    {
        this.y = y;
        this.x = x;
    }
}

public class Board
{
    public int[,] board; // [x] [y]

    public HashSet<Point> pieces;
    public int score; //black = positive, red = negative
    public int moveColor;

    public Board(int[,] board, HashSet<Point> pieces, int score, int moveColor)
    {
        this.board = board;
        this.pieces = pieces;
        this.score = score;
        this.moveColor = moveColor;
    }

    public Board Move(Point p)
    {
        Board newBoard = (Board) this.MemberwiseClone(); //new board instance
        newBoard.board[p.y, p.x] = moveColor; //add piece to board
        newBoard.pieces.Add(new Point(p.y, p.x)); //add piece to list
        newBoard.score += AddToScore(p.y, p.x); //change score
        newBoard.moveColor = moveColor == ((int) Game.color.RED) ? ((int)Game.color.BLACK) : ((int)Game.color.RED); //change move color
        return newBoard;
    }

    private int AddToScore(int y, int x)
    {
        int delta = 0;
        delta += CheckDirection(y, x, moveColor, 0, 1);
        delta += CheckDirection(y, x, moveColor, 1, 0);
        delta += CheckDirection(y, x, moveColor, 1, 1);
        delta += CheckDirection(y, x, moveColor, -1, 1);
        return delta;
    }
    private int CheckDirection(int y, int x, int moveColor, int yChange, int xChange)
    {
        int count = 1;
        int yPointer = y;
        int xPointer = x;
        while (yPointer + yChange >= 0 && yPointer + yChange < Game.rows && xPointer + xChange >= 0 && xPointer + xChange < Game.cols && board[yPointer + yChange, xPointer + xChange] == moveColor)
        {
            count++;
            yPointer += yChange;
            xPointer += xChange;
        }
        yPointer = y;
        xPointer = x;
        while (yPointer - yChange >= 0 && yPointer - yChange < Game.rows && xPointer - xChange >= 0 && xPointer - xChange < Game.cols && board[yPointer - yChange, xPointer - xChange] == moveColor)
        {
            count++;
            yPointer -= yChange;
            xPointer -= xChange;
        }

        int delta = 0;
        if (moveColor == (int) Game.color.RED)
        { 
            delta -= (int)Mathf.Pow(3, count - 1) - 1;
            if (count >= Game.winCount) delta -= 1000000000;
        }
        else
        { 
            delta += (int)Mathf.Pow(3, count - 1) - 1;
            if (count >= Game.winCount) delta += 1000000000;
        }
        return delta;
    }
}
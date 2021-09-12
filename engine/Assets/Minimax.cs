using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimax : MonoBehaviour
{
    public Game game;

    private int depth = 3;
    public Board ComputeMove(Board b)
    {
        HashSet<Point> moves = ValidMoves(b.board, b.pieces);

        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;
        Point movePoint = new Point(0, 0);
        foreach (Point p in moves)
        {
            int num = Evaluate(b.Move(p), depth, value);
            if (b.moveColor == (int)Game.color.BLACK && num > value) { value = num; movePoint = p; }
            else if (b.moveColor == (int)Game.color.RED && num < value) { value = num; movePoint = p; }
        }
        game.AiMove(movePoint);
        return b.Move(movePoint);
    }

    //MINIMAX ALGORITHM RECURSION HERE
    private int Evaluate(Board b, int depth, int valueAbove)
    {//MINIMAX RECURSION
        //check depth: base case
        if (depth == 1)
        {
            return b.score;
        }
        //create list of all valid moves for given board
        HashSet<Point> moves = ValidMoves(b.board, b.pieces);

        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;

        foreach (Point p in moves)
        {
            int num = Evaluate(b.Move(p), depth - 1, value);
            if (b.moveColor == (int)Game.color.BLACK)
            {
                if (num > value) value = num;
                if (value > valueAbove) return value;
            }
            else 
            {
                if (num < value) value = num;
                if (value < valueAbove) return value;
            }
        }

        //check value above and compare it based on min or max: prune
        return value;
    }

    private HashSet<Point> ValidMoves(int[,] b, HashSet<Point> pieces)
    {
        HashSet<Point> o = new HashSet<Point>();
        foreach (Point p in pieces)
        {
            CheckPoint(o, b, p, 1, 1);
            CheckPoint(o, b, p, 0, 1);
            CheckPoint(o, b, p, 1, 0);
            CheckPoint(o, b, p, -1, -1);
            CheckPoint(o, b, p, 0, -1);
            CheckPoint(o, b, p, -1, 0);
            CheckPoint(o, b, p, -1, 1);
            CheckPoint(o, b, p, 1, -1);
        }
        return o;
    }
    private void CheckPoint(HashSet<Point> o, int[,] b, Point p, int yChange, int xChange)
    {
        //checks if position is a valid move (touching another piece)
        if (p.y + yChange < Game.rows && p.x + xChange < Game.cols &&
            p.y + yChange >= 0 && p.x + xChange >= 0 &&
            b[p.y + yChange, p.x + xChange] == ((int)Game.color.BLANK))
            o.Add(new Point(p.y + yChange, p.x + xChange));
    }

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
    public override bool Equals(System.Object obj)
    {
        Point p = (Point) obj;
        return  (y == p.y) && (x == p.x);        
    }

    public override string ToString() { return $"({x},{y})"; }
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
        Board newBoard = new Board((int[,])board.Clone(), new HashSet<Point>(pieces), score, moveColor); //new board instance
        newBoard.board[p.y, p.x] = moveColor; //add piece to board
        newBoard.pieces.Add(new Point(p.y, p.x)); //add piece to list
        newBoard.score += AddToScore(p.y, p.x); //change score
        newBoard.moveColor = moveColor == ((int)Game.color.RED) ? ((int)Game.color.BLACK) : ((int)Game.color.RED); //change move color
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
        if (moveColor == (int)Game.color.RED)
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

    public void PrintBoard()
    {
        string s = "\n";
        for(int i = 0; i < Game.rows; i++)
        {
            for(int j = 0; j<Game.cols; j++)
            {
                s += board[i, j]+" ";
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}
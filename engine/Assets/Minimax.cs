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
        HashSet<(int,int)> moves = ValidMoves(b.board, b.pieces);

        if (b.pieces.Count == 0) return b.Move(new Point(Game.rows/2,Game.cols/2));

        if (moves.Count == 0) { b.tieGame = true; return b; }//TIE GAME

        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;
        Point movePoint = new Point(0, 0);
        foreach ((int,int) p in moves)
        {
            int num = Evaluate(b.Move(new Point(p.Item1,p.Item2)), depth, value);
            if (b.moveColor == (int)Game.color.BLACK && num > value) { value = num; movePoint = new Point(p.Item1,p.Item2); }
            else if (b.moveColor == (int)Game.color.RED && num < value) { value = num; movePoint = new Point(p.Item1, p.Item2); }
        }        
        if(Game.showBoard) game.AiMove(movePoint);
        return b.Move(movePoint);
    }

    //MINIMAX ALGORITHM RECURSION HERE
    private int Evaluate(Board b, int depth, int valueAbove)
    {
        if (depth == 1|| Mathf.Abs(b.score) > 100000000)
        {
            return b.score;
        }

        //create list of all valid moves for given board
        HashSet<(int, int)> moves = ValidMoves(b.board, b.pieces);

        //tie game
        if (moves.Count == 0) return 0;

        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;

        foreach ((int, int) p in moves)
        {
            int num = Evaluate(b.Move(new Point(p.Item1,p.Item2)), depth - 1, value);
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

        return value;
    }

    private HashSet<(int,int)> ValidMoves(int[,] b, HashSet<(int,int)> pieces)
    {
        HashSet<(int,int)> o = new HashSet<(int,int)>();
        foreach ((int,int) p in pieces)
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
    private void CheckPoint(HashSet<(int,int)> o, int[,] b, (int,int) p, int yChange, int xChange)
    {
        //checks if position is a valid move (touching another piece)
        if (p.Item1 + yChange < Game.rows && p.Item2 + xChange < Game.cols &&
            p.Item1 + yChange >= 0 && p.Item2 + xChange >= 0 &&
            b[p.Item1 + yChange, p.Item2 + xChange] == ((int)Game.color.BLANK))
            o.Add((p.Item1 + yChange, p.Item2 + xChange));
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
    public override string ToString() { return $"({x},{y})"; }
}

public class Board
{
    public int[,] board; // [x] [y]

    public HashSet<(int,int)> pieces;
    public int score; //black = positive, red = negative
    public int moveColor;

    public Boolean tieGame = false;


    public Board(int[,] board, HashSet<(int,int)> pieces, int score, int moveColor)
    {
        this.board = board;
        this.pieces = pieces;
        this.score = score;
        this.moveColor = moveColor;
    }

    public Board Move(Point p)
    {
        Board newBoard = new Board((int[,])board.Clone(), new HashSet<(int,int)>(pieces), score, moveColor); //new board instance
        newBoard.board[p.y, p.x] = moveColor; //add piece to board
        newBoard.pieces.Add((p.y, p.x)); //add piece to list
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
        int blankCount = 0;
        int yPointer = y;
        int xPointer = x;
        while (yPointer + yChange >= 0 && yPointer + yChange < Game.rows && xPointer + xChange >= 0 && xPointer + xChange < Game.cols )
        {
            if(board[yPointer + yChange, xPointer + xChange] == moveColor)
            {
                count++;
                yPointer += yChange;
                xPointer += xChange;
            }
            else
            {
                if(board[yPointer + yChange, xPointer + xChange] == (int)Game.color.BLANK) blankCount++;
                break;
            }
        }
        yPointer = y;
        xPointer = x;
        while (yPointer - yChange >= 0 && yPointer - yChange < Game.rows && xPointer - xChange >= 0 && xPointer - xChange < Game.cols && board[yPointer - yChange, xPointer - xChange] == moveColor)
        {
            if (board[yPointer - yChange, xPointer - xChange] == moveColor)
            {
                count++;
                yPointer -= yChange;
                xPointer -= xChange;
            }
            else
            {
                if (board[yPointer - yChange, xPointer - xChange] == (int)Game.color.BLANK) blankCount++;
                break;
            }
        }

        int delta = 0;
        if (moveColor == (int)Game.color.RED)
        {
            delta -= ((int)Mathf.Pow(3, count-1) + (int)Mathf.Pow(2, blankCount) - 2);
            if (count >= Game.winCount) delta -= 1000000000;
        }
        else
        {
            delta += ((int)Mathf.Pow(3, count-1) + (int)Mathf.Pow(2, blankCount) - 2);
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
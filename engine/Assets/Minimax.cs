using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimax : MonoBehaviour
{
    public Game game;

    private int maxEndNodes = 100000;
    int endNodes = 0;
    int prunedNodes = 0;

    Dictionary<string, int> enumeratedBoards = new Dictionary<string,int>();

    public Board ComputeMove(Board b)
    {
        //PrintScores(b);

        HashSet<(int,int)> moves = ValidMoves(b.board, b.pieces);

        if (b.pieces.Count == 0) return b.Move(new Point(Game.rows/2,Game.cols/2));

        if (moves.Count == 0) { b.tieGame = true; return b; }//TIE GAME

        int currentDepth = 6;
        endNodes = 0;
        Point limitedMovePoint = new Point(0, 0);

       // while (endNodes <= maxEndNodes)
        //{
            endNodes = 0;
        prunedNodes = 0;
        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;

            Point movePoint = new Point(0, 0);
         enumeratedBoards = new Dictionary<string, int>();

        bool foundWinningMove = false;
        foreach ((int, int) p in moves)
        {
            if (Mathf.Abs(b.Move(new Point(p.Item1, p.Item2)).score) > 10000000) { movePoint = new Point(p.Item1, p.Item2); foundWinningMove = true; break; }
        }

        if (!foundWinningMove)
        {
            foreach ((int, int) p in moves)
            {
                int num = Evaluate(b.Move(new Point(p.Item1, p.Item2)), currentDepth, value);
                if (b.moveColor == (int)Game.color.BLACK && num > value) { value = num; movePoint = new Point(p.Item1, p.Item2); }
                else if (b.moveColor == (int)Game.color.RED && num < value) { value = num; movePoint = new Point(p.Item1, p.Item2); }
            }
        }
        limitedMovePoint = new Point(movePoint.y, movePoint.x);

      //  Debug.Log($"Depth: {currentDepth}, Nodes: {endNodes}");
       //   if (endNodes <= maxEndNodes) limitedMovePoint = new Point(movePoint.y, movePoint.x);
        //  else break;

       //   if (currentDepth > 10) break;
       //   currentDepth++;
       // }

        //Debug.Log(endNodes);
        //Debug.Log("prunedNodes:" + prunedNodes);

        if (Game.showBoard) game.AiMove(limitedMovePoint);
        return b.Move(limitedMovePoint);
    }

    //MINIMAX ALGORITHM RECURSION HERE
    private int Evaluate(Board b, int depth, int valueAbove)
    {
        if (depth == 1|| Mathf.Abs(b.score) > 10000000)
        {
            endNodes++;
            return b.score;
        }

        
        //ENCODE
        List<(int, int, int)> encodedPoints = new List<(int, int, int)>();
        foreach((int,int) p in b.pieces)
        {
            encodedPoints.Add((p.Item1, p.Item2, b.board[p.Item1, p.Item2]));
        }
        encodedPoints.Sort(ComparePoints);

        string s = "";
        foreach((int,int,int) p in encodedPoints)
        {
            s += p.Item1.ToString("00") + p.Item2.ToString("00") + p.Item3;
        }
        if (enumeratedBoards.ContainsKey(s))
        {
            enumeratedBoards.TryGetValue(s, out int output);
            prunedNodes++;
            return output;
        }
        else
        {
            //create list of all valid moves for given board
            HashSet<(int, int)> moves = ValidMoves(b.board, b.pieces);

            //tie game
            if (moves.Count == 0) return 0;

            int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;

            foreach ((int, int) p in moves)
            {
                if (Mathf.Abs(b.Move(new Point(p.Item1, p.Item2)).score) > 10000000) { enumeratedBoards.Add(s, b.Move(new Point(p.Item1, p.Item2)).score); return b.Move(new Point(p.Item1, p.Item2)).score; }
            }
            
            foreach ((int, int) p in moves)
            {
                int num = Evaluate(b.Move(new Point(p.Item1, p.Item2)), depth - 1, value);

                if (b.moveColor == (int)Game.color.BLACK)
                {
                    if (num >10000000) { value = num; enumeratedBoards.Add(s, value); return value; }
                    if (num > value) value = num;
                    if (value > valueAbove){enumeratedBoards.Add(s, value); return value; }
                }
                else
                {
                    if(num<-10000000){value = num; enumeratedBoards.Add(s, value); return value;}
                    if (num < value) value = num;
                    if (value < valueAbove){enumeratedBoards.Add(s, value); return value; }
                }
            }

            enumeratedBoards.Add(s, value);
            return value;
        }


    }


    public void PrintScores(Board b)
    {
        HashSet<(int, int)> moves = ValidMoves(b.board, b.pieces);

        int[,] boardScores = (int[,])b.board.Clone();

        foreach ((int, int) p in moves)
        {
            boardScores[p.Item1, p.Item2] = b.AddToScore(p.Item1,p.Item2);
        }

        string s = "\n";
        for (int i = 0; i < Game.rows; i++)
        {
            for (int j = 0; j < Game.cols; j++)
            {
                s += boardScores[i, j] + " ";
            }
            s += "\n";
        }
        Debug.Log(s);
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
    private int ComparePoints((int, int, int) a, (int, int, int) b)
    {
        if(a.Item1!=b.Item1)
            return a.Item1.CompareTo(b.Item1);
        else
            return a.Item2.CompareTo(b.Item2);
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
    int connectPower = 3;
    int blankPower = 2;

    int blockPower = 3;
    int blockBlank = 2;



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
        //if (Math.Abs(newBoard.score) > 10000000) newBoard.score = newBoard.score > 0 ? 1000000000 : -1000000000;
        newBoard.moveColor = moveColor == ((int)Game.color.RED) ? ((int)Game.color.BLACK) : ((int)Game.color.RED); //change move color
        return newBoard;
    }

    public int AddToScore(int y, int x)
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

        int yPointer = y + yChange;
        int xPointer = x + xChange;


        while (checkBounds(yPointer, xPointer))
        {
            if (board[yPointer, xPointer] == moveColor) { count++; yPointer += yChange; xPointer += xChange; }
            else{if (board[yPointer, xPointer] == (int)Game.color.BLANK) blankCount++;break;}
        }

        yPointer = y - yChange;
        xPointer = x - xChange;

        while (checkBounds(yPointer, xPointer))
        {
            if (board[yPointer, xPointer] == moveColor) { count++; yPointer -= yChange; xPointer -= xChange; }
            else { if (board[yPointer, xPointer] == (int)Game.color.BLANK) blankCount++; break; }
        }

        int newColor = moveColor == ((int)Game.color.RED) ? ((int)Game.color.BLACK) : ((int)Game.color.RED);
        int oppositeCount = 1;
        int oppositeBlanks = 0;
        yPointer = y;
        xPointer = x;

        int delta = 0;
        if (moveColor == (int)Game.color.RED)
        {
            delta -= (int)(Mathf.Pow(connectPower, count) + Mathf.Pow(blankPower, blankCount));//+ Mathf.Pow(blockPower, oppositeCount) + Mathf.Pow(blockBlank, oppositeBlanks));
            if (count >= Game.winCount)delta -= 1000000000;            
            if (oppositeCount >= Game.winCount)delta -= 10000;
            
        }
        else
        {
            delta += (int)(Mathf.Pow(connectPower, count) + Mathf.Pow(blankPower, blankCount)); //+ Mathf.Pow(blockPower, oppositeCount) + Mathf.Pow(blockBlank, oppositeBlanks));
            if (count >= Game.winCount) delta += 1000000000;
            if (oppositeCount >= Game.winCount) delta += 10000;
        }
        return delta;

    }
    private bool checkBounds(int y, int x)
    {
        return y >= 0 && y < Game.rows && x >= 0 && x < Game.cols;
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
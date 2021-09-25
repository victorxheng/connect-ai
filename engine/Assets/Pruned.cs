using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pruned : MonoBehaviour
{
    public Game game;

    private int depth = 3;

    public Board ComputeMove(Board b)
    {
        HashSet<(int, int)> moves = ValidMoves(b.board, b.pieces);

        if (b.pieces.Count == 0) return b.Move(new Point(Game.rows / 2, Game.cols / 2));

        if (moves.Count == 0) { b.tieGame = true; return b; }//TIE GAME

        int value = b.moveColor == (int)Game.color.BLACK ? Int32.MinValue : Int32.MaxValue;
        Point movePoint = new Point(0, 0);
        foreach ((int, int) p in moves)
        {
            int num = Evaluate(b.Move(new Point(p.Item1, p.Item2)), depth, value);
            if (b.moveColor == (int)Game.color.BLACK && num > value) { value = num; movePoint = new Point(p.Item1, p.Item2); }
            else if (b.moveColor == (int)Game.color.RED && num < value) { value = num; movePoint = new Point(p.Item1, p.Item2); }
        }
        if (Game.showBoard) game.AiMove(movePoint);
        return b.Move(movePoint);
    }

    //MINIMAX ALGORITHM RECURSION HERE
    private int Evaluate(Board b, int depth, int valueAbove)
    {
        if (depth == 1 || Mathf.Abs(b.score) > 100000000)
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
            int num = Evaluate(b.Move(new Point(p.Item1, p.Item2)), depth - 1, value);
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

    private HashSet<(int, int)> ValidMoves(int[,] b, HashSet<(int, int)> pieces)
    {
        HashSet<(int, int)> o = new HashSet<(int, int)>();
        foreach ((int, int) p in pieces)
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
    private void CheckPoint(HashSet<(int, int)> o, int[,] b, (int, int) p, int yChange, int xChange)
    {
        //checks if position is a valid move (touching another piece)
        if (p.Item1 + yChange < Game.rows && p.Item2 + xChange < Game.cols &&
            p.Item1 + yChange >= 0 && p.Item2 + xChange >= 0 &&
            b[p.Item1 + yChange, p.Item2 + xChange] == ((int)Game.color.BLANK))
            o.Add((p.Item1 + yChange, p.Item2 + xChange));
    }

}
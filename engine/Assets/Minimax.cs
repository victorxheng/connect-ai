using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimax : MonoBehaviour
{
    public Game game;


    private void ComputeMove(){
        
    }
    private int Project(){//MINIMAX RECURSION
        return 0;
    }
    

    private HashSet<Point> validMoves(int[,] b, HashSet<Point> blackPieces, HashSet<Point> redPieces)
    {
        HashSet<Point> o = new HashSet<Point>();
        checkSurroundingPiecesFromSet(o,blackPieces,b);
        checkSurroundingPiecesFromSet(o,redPieces,b);
        return o;

    }
    private void checkSurroundingPiecesFromSet(HashSet<Point> o, HashSet<Point> pieces, int[,] b){
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
    }
    private void checkPoint(HashSet<Point> o, int[,] b, Point p, int yChange, int xChange)
    {
        if (p.y + yChange < Game.rows && p.x + xChange < Game.cols &&
            p.y + yChange >= 0 && p.x + xChange >= 0 &&
            b[p.y + yChange, p.x + xChange] == ((int)Game.color.BLANK))
            o.Add(new Point(p.y + yChange, p.x + xChange));
    }


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
}

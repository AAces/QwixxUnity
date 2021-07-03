using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{


    Row[] rows;
    public int player;
    int penalties;

    public Card(int p)
    {
        this.rows = new Row[] { new Row(0, true), new Row(1, true), new Row(2, false), new Row(3, false) };
        player = p;
        penalties = 0;
    }
    public int printScore()
    {
        int score = calcScore();

        return score;
    }
    int calcScore()
    {
        int score = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j < rows[i].getMarkedCount() + 1; j++)
            {
                score += j;
            }
        }
        score -= penalties * 3;
        return score;
    }
    string scoreToPts(int s)
    {
        return (s < 10 ? s.ToString() + "pts  " : (s < 100 ? s.ToString() + "pts " : s.ToString() + "pts"));
    }
    string xToPts(int r)
    {
        int pts = 0;
        int x = rows[r].getMarkedCount();
        for (int i = 1; i < x + 1; i++)
        {
            pts += i;
        }
        return (pts < 10 ? pts.ToString() + "pts " : pts.ToString() + "pts");
    }
    string pToPts()
    {
        int pts = 5 * penalties;
        return (pts < 10 ? pts.ToString() + "pts " : pts.ToString() + "pts");
    }
    string xToString(int r)
    {
        return (rows[r].getMarkedCount().ToString() + "x" + (rows[r].getMarkedCount() < 10 ? " " : ""));
    }
    public void updateCard()
    {

    }
    public Row[] getRows()
    {
        return rows;
    }
    public void addPenalty()
    {
        penalties++;
    }
    public int getPenalties()
    {
        return penalties;
    }
}
public class Row
{
    int color;
    bool startsSmall;
    List<int> markedNumbers;

    public Row(int c, bool s)
    {
        this.color = c;
        this.startsSmall = s;
        this.markedNumbers = new List<int>();
    }

    public bool addMarkedNumber(int n)
    {
        if (markedNumbers.Contains(n) || (startsSmall ? n < getFurthestRight() : n > getFurthestRight()) || (markedNumbers.Count < 5 && (startsSmall ? n == 12 : n == 2)))
        {
            return false;
        }
        else
        {
            markedNumbers.Add(n);
            return true;
        }
    }

    public bool getStartsSmall()
    {
        return startsSmall;
    }

    public int getMarkedCount()
    {
        return markedNumbers.Count;
    }

    public int getFurthestRight()
    {
        if (startsSmall)
        {
            switch (markedNumbers.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return markedNumbers[0];
                default:
                    int max = markedNumbers[0];
                    for (int i = 1; i < markedNumbers.Count; i++)
                    {
                        if (markedNumbers[i] > max)
                        {
                            max = markedNumbers[i];
                        }
                    }
                    return max;
            }
        }
        else
        {
            switch (markedNumbers.Count)
            {
                case 0:
                    return 13;
                case 1:
                    return markedNumbers[0];
                default:
                    int min = markedNumbers[0];
                    for (int i = 1; i < markedNumbers.Count; i++)
                    {
                        if (markedNumbers[i] < min)
                        {
                            min = markedNumbers[i];
                        }
                    }
                    return min;
            }
        }
    }

    public string printRow()
    {
        string r = "";
        if (startsSmall)
        {
            for (int i = 2; i < 10; i++)
            {
                if (markedNumbers.Contains(i))
                {
                    r += " █ ";
                }
                else
                {
                    r += " " + i + " ";
                }
            }
            for (int i = 10; i < 12; i++)
            {
                if (markedNumbers.Contains(i))
                {
                    r += " █ ";
                }
                else
                {
                    r += " " + i;
                }
            }
            r += "   " + (markedNumbers.Contains(12) ? "█  X" : "12 O");
        }
        else
        {
            for (int i = 12; i > 9; i--)
            {
                if (markedNumbers.Contains(i))
                {
                    r += " █ ";
                }
                else
                {
                    r += " " + i;
                }
            }
            for (int i = 9; i > 2; i--)
            {
                if (markedNumbers.Contains(i))
                {
                    r += " █ ";
                }
                else
                {
                    r += " " + i + " ";
                }
            }
            r += "   " + (markedNumbers.Contains(2) ? "█  X" : "2  O");
        }
        return r;
    }
}

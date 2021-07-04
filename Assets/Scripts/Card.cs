using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    Row[] rows = new Row[] { new Row(0, true), new Row(1, true), new Row(2, false), new Row(3, false) };
    public int player;
    int penalties = 0;
    public GameObject[] penaltiesObj, lockObj;

    public Card()
    {
        penalties = 0;
    }
    public void markLock(int row)
    {
        lockObj[row].SetActive(true);
    }
    public void hideLocks()
    {
        foreach(GameObject o in lockObj)
        {
            o.SetActive(false);
        }
    }
    public void updatePenalties()
    {
        foreach(GameObject o in penaltiesObj)
        {
            o.SetActive(false);
        }
        if (penalties > 0) {
            for (int i = 0; i < penalties; i++)
            {
                penaltiesObj[i].SetActive(true);
            }
        }
                
    }
    public int getTotalScore()
    {
        int score = 0;
        for (int i = 0; i < 4; i++)
        {
            score += getRowScore(i);
        }
        score -= getPenaltyScore();
        return score;
    }
    public int getRowScore(int i)
    {
        int score = 0;
        for (int j = 1; j < rows[i].getMarkedCount() + 1; j++)
        {
            score += j;
        }
        return score;
    }
    public int getPenaltyScore()
    {
        return penalties * 5;
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
    bool startsSmall;
    List<int> markedNumbers = new List<int>();

    public Row(int c, bool s)
    {
        this.startsSmall = s;
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

    public List<int> getMarkedNumbers()
    {
        return markedNumbers;
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
}
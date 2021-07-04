using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    int red, blue, yellow, green, white1, white2, players, activePlayer, whiteSum;
    int[] dice;
    string[] diceNames = new string[] { "White: ", "White: ", "Red: ", "Yellow: ", "Green: ", "Blue: " };
    public Card card1, card2, card3, card4;
    Card[] cards;
    List<int> pRed = new List<int>(), pYellow = new List<int>(), pGreen = new List<int>(), pBlue = new List<int>();
    HashSet<int> toBeLocked = new HashSet<int>(), lockedRows = new HashSet<int>();
    public Button bOne, bTwo, bThree, bFour, nextRoll;
    Button[] buttons;
    public Button[] c1r1, c1r2, c1r3, c1r4, c2r1, c2r2, c2r3, c2r4, c3r1, c3r2, c3r3, c3r4, c4r1, c4r2, c4r3, c4r4;
    Button[][] card1Buttons, card2Buttons, card3Buttons, card4Buttons;
    Button[][][] allNumButtons;
    public GameObject canvasObj, t1, t2, nextButtonCanvas;
    public GameObject[] whiteDice1, whiteDice2, redDice, yellowDice, greenDice, blueDice, borders, scoreCards;
    public Text[] scoreCard1Text, scoreCard2Text, scoreCard3Text, scoreCard4Text;
    Text[][] scoreCardsText;
    GameObject[][] diceObj;
    bool[] whitePressed = new bool[] { false, false, false, false };
    List<int>[] lists;
    bool activePlayerPressed = false, activePlayerPressed2 = false;

    void Start()
    {
        initGame();
        addListeners();
    }

    void addListeners()
    {
        bOne.onClick.AddListener(delegate { press(1); });
        bTwo.onClick.AddListener(delegate { press(2); });
        bThree.onClick.AddListener(delegate { press(3); });
        bFour.onClick.AddListener(delegate { press(4); });
        nextRoll.onClick.AddListener(delegate { handleNextPress(); });
        for (int c = 0; c < 4; c++)
        {
            for (int r = 0; r < 4; r++)
            {
                for (int n = 2; n < 13; n++)
                {
                    int x = c;
                    int y = r;
                    int z = n;
                    allNumButtons[x][y][z - 2].onClick.AddListener(delegate { handlePress(x, y, z); });
                }
            }
        }
        
    }

    void resetWhitePressed()
    {
        for(int i = 0; i<4; i++)
        {
            whitePressed[i] = false;
        }
    }

    void hideCards()
    { 
        foreach(Card c in cards)
        {
            c.gameObject.SetActive(false);
        }
    }

    void updateCrossedNumbers(int c)
    {
        for(int i = 0; i<4; i++)
        {
            int r = i;
            for (int j = 2; j<13; j++)
            {
                int n = j;
                if (cards[c].getRows()[r].getMarkedNumbers().Contains(n))
                {
                    allNumButtons[c][r][n-2].transform.GetChild(0).gameObject.SetActive(true);
                } else
                {
                    allNumButtons[c][r][n-2].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            
            if (cards[c].getRows()[r].getMarkedNumbers().Contains(13) || cards[c].getRows()[r].getMarkedNumbers().Contains(1))
            {
                cards[c].markLock(r);
            }
        }
        if (!activePlayerPressed)
        {
            ColorBlock cb = nextRoll.colors;
            cb.normalColor = Color.red;
            nextRoll.colors = cb;
        }
        else
        {
            ColorBlock cb = nextRoll.colors;
            cb.normalColor = Color.green;
            nextRoll.colors = cb;
        }
    }

    void updatePenalties()
    {
        foreach(Card c in cards)
        {
            c.updatePenalties();
        }
    }

    void trySecondPress(int c, int r, int n)
    {
        if (r > 3)
        {
            Debug.Log("Somehow a button in row '" + (r + 1) + "' was pressed?");
            return;
        }
        if (lists[r].Contains(n))
        {
            if (lockedRows.Contains(r))
            {
                Debug.Log("That row has been locked!");
                return;
            }
            if (cards[c].getRows()[r].addMarkedNumber(n))
            {
                Debug.Log("The number " + n + " was marked off of row " + (r + 1) + " of card " + (c + 1));
                activePlayerPressed = true;
                if (cards[c].getRows()[r].getStartsSmall())
                {
                    if (n == 12)
                    {
                        cards[c].getRows()[r].addMarkedNumber(13);
                        toBeLocked.Add(r);
                    }
                }
                else
                {
                    if (n == 2)
                    {
                        cards[activePlayer - 1].getRows()[r].addMarkedNumber(1);
                        toBeLocked.Add(r);
                    }
                }
                updateCrossedNumbers(c);
                activePlayerPressed2 = true;
            }
        } else
        {
            if (lists[r].Count > 1) 
            {
                Debug.Log("Cant mark off " + n + " from row " + (r + 1) + ", can only mark off " + lists[r][0] + " or " + lists[r][1]);
            } else
            {
                Debug.Log("Cant mark off " + n + " from row " + (r + 1) + ", can only mark off " + lists[r][0]);
            }

        }
    }

    void handlePress(int card, int row, int num)
    {
        Debug.Log("Button pressed: Card " + (card + 1) + ", row " + (row + 1) + ", number " + num + ".");
        bool active = card == activePlayer - 1;
        if (lockedRows.Contains(row))
        {
            return;
        }
        if(active && activePlayerPressed2)
        {
            return;
        }
        
        if (num == whiteSum)
        {
            if (!whitePressed[card])
            {
                if (active && activePlayerPressed)
                {
                    return;
                }
                if (cards[card].getRows()[row].addMarkedNumber(num))
                {
                    Debug.Log("The number " + whiteSum + " was marked off of row " + (row + 1) + " of card " + (card+1));
                    whitePressed[card] = true;
                    if (active) { activePlayerPressed = true; }
                    if (cards[card].getRows()[row].getStartsSmall())
                    {
                        if (whiteSum == 12)
                        {
                            cards[card].getRows()[row].addMarkedNumber(13);
                            toBeLocked.Add(row);
                        }
                    }
                    else
                    {
                        if (whiteSum == 2)
                        {
                            cards[card].getRows()[row].addMarkedNumber(1);
                            toBeLocked.Add(row);
                        }
                    }
                    updateCrossedNumbers(card);
                }
            } else
            {
                if (active)
                {
                    trySecondPress(card, row, num);
                }
            }
        } else
        {
            if (active)
            {
                trySecondPress(card, row, num);
            }
        }
    }

    void handleNextPress()
    {
        if (!activePlayerPressed)
        {
            cards[activePlayer - 1].addPenalty();
            cards[activePlayer - 1].updatePenalties();
        }

        activePlayerPressed = false;
        activePlayerPressed2 = false;
        ColorBlock cb = nextRoll.colors;
        cb.normalColor = Color.red;
        nextRoll.colors = cb;

        if (toBeLocked.Count > 0)
        {
            foreach (int i in toBeLocked)
            {
                lockedRows.Add(i);

            }
            if (lockedRows.Count > 1)
            {
                end(2);
                return;
            }
        }
        if (cards[activePlayer-1].getPenalties() > 3)
        {
            end(1);
            return;
        }
        activePlayer++;
        if (activePlayer > players)
        {
            activePlayer = 1;
        }
        refreshBorders();
        resetWhitePressed();
        roll();
    }

    void showCards()
    {
        for(int i=0; i<players; i++)
        {
            cards[i].gameObject.SetActive(true);
        }
    }

    void showScoreCards()
    {
        for (int i = 0; i < players; i++)
        {
            scoreCards[i].SetActive(true);
        }
    }

    void updateScoreCards()
    {
        for (int i = 0; i < players; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                scoreCardsText[i][j].text = cards[i].getRowScore(j).ToString();
            }
            scoreCardsText[i][4].text = cards[i].getPenaltyScore().ToString();
            scoreCardsText[i][5].text = cards[i].getTotalScore().ToString();            
        }
    }

    private IEnumerator hideButtons(int b)
    {
        yield return new WaitForSeconds(0.1f);
        for(int i=0; i<4; i++)
        {
            if (i == b-1) continue;
            for (int j = 0; j < 10; j++)
            {
                buttons[i].gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0f);
                yield return new WaitForSeconds(0.01f);
            }
            for (int j = 0; j < 110; j++)
            {
                buttons[i].gameObject.transform.localScale += new Vector3(-0.01f, -0.01f, 0f);
                yield return new WaitForSeconds(0.001f);
            }
        }
        float f = buttons[b - 1].gameObject.transform.localPosition.x;
        for (int j=0; j<200; j++)
        {
            buttons[b - 1].gameObject.transform.localPosition += new Vector3((0-f)/200,0,0);
            yield return new WaitForSeconds(0.00001f*Mathf.Abs(f));
        }
        for (int j = 0; j < 10; j++)
        {
            buttons[b-1].gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        for (int j = 0; j < 110; j++)
        {
            buttons[b-1].gameObject.transform.localScale += new Vector3(-0.01f, -0.01f, 0f);
            yield return new WaitForSeconds(0.001f);
        }
        for (int j = 0; j < 100; j++)
        {
            t1.transform.localPosition += new Vector3(10f, 0f, 0f);
            t2.transform.localPosition += new Vector3(10f, 0f, 0f);
            yield return new WaitForSeconds(0.001f);
        }
        canvasObj.SetActive(false);
        start();
    }

    void initGame()
    {
        cards = new Card[] { card1, card2, card3, card4 };
        buttons = new Button[] { bOne, bTwo, bThree, bFour };
        diceObj = new GameObject[][] { whiteDice1, whiteDice2, redDice, yellowDice, greenDice, blueDice };
        scoreCardsText = new Text[][] { scoreCard1Text, scoreCard2Text, scoreCard3Text, scoreCard4Text };
        card1Buttons = new Button[][] { c1r1, c1r2, c1r3, c1r4 };
        card2Buttons = new Button[][] { c2r1, c2r2, c2r3, c2r4 };
        card3Buttons = new Button[][] { c3r1, c3r2, c3r3, c3r4 };
        card4Buttons = new Button[][] { c4r1, c4r2, c4r3, c4r4 };
        allNumButtons = new Button[][][] { card1Buttons, card2Buttons, card3Buttons, card4Buttons };
        activePlayer = 1;
        for(int i = 0; i<4; i++)
        {
            updateCrossedNumbers(i);
            cards[i].hideLocks();
        }
        updatePenalties();
        canvasObj.SetActive(true);
        nextButtonCanvas.SetActive(false);
        hideCards();
        hideScoreCards();
        hideDice();
    }

    void hideDice()
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                diceObj[i][j].SetActive(false);
            }
        }
    }

    void hideScoreCards()
    {
        foreach(GameObject o in scoreCards)
        {
            o.SetActive(false);
        }
    }

    void press(int p)
    {
        players = p;
        StartCoroutine(hideButtons(p));
    }

    void start()
    {
        showCards();
        refreshBorders();
        nextButtonCanvas.SetActive(true);
        Debug.Log("Starting game with " + players + " players!");
        roll();
    }

    void refreshBorders()
    {
        foreach(GameObject o in borders)
        {
            o.SetActive(false);
        }
        borders[activePlayer - 1].SetActive(true);
    }

    void end(int how)
    {
        switch (how)
        {
            case 1:
                Debug.Log("The game ended! One player got 4 penalties.");
                break;
            case 2:
                Debug.Log("The game ended! Two rows have been locked!");
                break;
            default:
                break;
        }
        hideCards();
        nextButtonCanvas.SetActive(false);
        hideDice();
        showScoreCards();
        updateScoreCards();
    }

    void roll()
    {
        red = Random.Range(1, 7);
        blue = Random.Range(1, 7);
        yellow = Random.Range(1, 7);
        green = Random.Range(1, 7);
        white1 = Random.Range(1, 7);
        white2 = Random.Range(1, 7);
        dice = new int[] { white1, white2, red, yellow, green, blue };
        whiteSum = white1 + white2;
        pRed.Add(white1 + red);
        pRed.Add(white2 + red);
        pYellow.Add(white1 + yellow);
        pYellow.Add(white2 + yellow);
        pGreen.Add(white1 + green);
        pGreen.Add(white2 + green);
        pBlue.Add(white1 + blue);
        pBlue.Add(white2 + blue);
        lists = new List<int>[] { pRed, pYellow, pGreen, pBlue };
        StartCoroutine(printDice());
    }
   
    private IEnumerator printDice()
    {
        foreach(GameObject[] o in diceObj)
        {
            foreach(GameObject b in o)
            {
                b.SetActive(false);
            }
        }
        for (int i = 0; i < 6; i++)
        {
            Debug.Log(diceNames[i] + dice[i]);
            if (lockedRows.Contains(i - 2))
            {
                continue;
            }         
            diceObj[i][dice[i] - 1].transform.localScale = new Vector3(0, 0, 0);
            diceObj[i][dice[i] - 1].SetActive(true);
            for (int j = 0; j < 10; j++)
            {
                diceObj[i][dice[i] - 1].transform.localScale += new Vector3(0.1f,0.1f,0.1f);
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}